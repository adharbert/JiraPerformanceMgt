using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jira.BO.Models;
using System.Data.Entity.Validation;

namespace Jira.BO.Services {
    public class ItemService {

        private MainContext _db = null;
        public ItemService() {
            _db = new MainContext();
        }
        public ItemService(MainContext db) {
            _db = db;
        }


        public void Save(dynamic item, int ProjectRestId) {
            int RestID = int.Parse(item.id.Value);
            string Abbr = item.key.Value;
            var ite = _db.Items.Where(i => i.RestID == RestID && i.Abbr == Abbr).SingleOrDefault();
            int ProjectID = _db.Projects.Where(p => p.RestID == ProjectRestId).Select(p => p.ProjectID).First();
            if (ite == null) {
                ite = new Item();
                ite.ProjectID = ProjectID;
                ite.Abbr = Abbr;
                ite.RestID = RestID;
                ite.CreateDt = DateTime.Now;
                _db.Items.Add(ite);
            }
            
            int itType = int.Parse(item.fields.issuetype.id.Value);
            ite.ItemType = (ItemType)itType;
            int itStatus = int.Parse(item.fields.status.id.Value);            
            ite.ItemStatus = (ItemStatus)itStatus;
            long originalEstimate = (item.fields.timetracking.originalEstimateSeconds != null) ? item.fields.timetracking.originalEstimateSeconds.Value : 0;
            long remaintingTime = (item.fields.timetracking.remainingEstimateSeconds != null) ? item.fields.timetracking.remainingEstimateSeconds.Value : 0;
            long timeSpent = (item.fields.progress.Value != null) ? item.fields.progress.Value : 0;
            int percentDone = (item.fields.progress.percent != null) ? (int)item.fields.progress.percent.Value : 0;
            DateTime? dueDt = null;
            if (item.fields.duedate != null) {
                string[] dd = item.fields.duedate.Value.Split('-'); // date format is yyyy-mm-dd
                dueDt = Convert.ToDateTime(string.Format("{0}/{1}/{2}", dd[1], dd[2], dd[0]));
            }

            //Make sure User is in Database and get their UserId.
            UserService us = new UserService();
            us.Save(item.fields.assignee);
            int UserID = us.UserId;

            try {

                if ((ite.Title == null && ite.Description == null) ||
                (ite.SelfUrl != item.self.ToString() && ite.Title != item.fields.summary.ToString() && ite.AssignedUserID != UserID && ite.Description != item.fields.description.ToString()) ||
                (ite.OriginalEstimateTimeInSeconds != originalEstimate && ite.RemainingTimeInSeconds != remaintingTime && ite.PercentDone != percentDone) ||
                (ite.AssignedUserID != UserID)) {
                    ite.Title = item.fields.summary.ToString();
                    ite.Description = item.fields.description.ToString();
                    ite.AssignedUserID = UserID;
                    ite.SelfUrl = item.self.ToString();
                    ite.OriginalEstimateTimeInSeconds = originalEstimate;
                    ite.RemainingTimeInSeconds = remaintingTime;
                    ite.PercentDone = percentDone;
                    ite.DueDt = dueDt;
                    ite.ModifyDt = DateTime.Now;
                }
                _db.SaveChanges();



                //Add assigned User to Project User database.
                ProjectUserService pus = new ProjectUserService(_db);
                pus.Save(UserID, ProjectID);


                //Add Worklog items for each user.
                //item.fields.worklog.worklogs.Count
                ItemUserWorkLogService iuwl = new ItemUserWorkLogService(_db);
                for (int i = 0; i < item.fields.worklog.worklogs.Count; i++) {
                    iuwl.Save(item.fields.worklog.worklogs[i], ite.ItemID);
                }

            } catch (DbEntityValidationException ex) {
                foreach (var eve in ex.EntityValidationErrors) {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors: ", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors) {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
	            }
                
            }
            
        }

        public IEnumerable<Item> GetListAll() {
            return _db.Items.ToList();
        }

        public IEnumerable<Item> GetListByProjectId(int projectId) {
            return _db.Items.Where(i => i.ProjectID == projectId).ToList();
        }

    }
}
