using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jira.BO.Models;

namespace Jira.BO.Services {
    public class ItemUserWorkLogService {

        private MainContext _db = null;        
      
        public ItemUserWorkLogService() {
            _db = new MainContext();
        }
        public ItemUserWorkLogService(MainContext db) {
            _db = db;
        }


        public void Save(dynamic log, int ItemId) {
            UserService us = new UserService();
            us.Save(log.author);
            int UserID = us.UserId;
            int workLogId = int.Parse(log.id.Value);
            var WLog = _db.UserItemWorklogs.Where(i => i.WorkLogID == workLogId).SingleOrDefault();
            if (WLog == null) {
                WLog = new UserItemWorklog();
                WLog.UserID = UserID;
                WLog.ItemID = ItemId;
                WLog.WorkLogID = workLogId;
                WLog.TimeSpentInSeconds = log.timeSpentSeconds.Value;
                WLog.CreateDt = DateTime.Now;
                _db.UserItemWorklogs.Add(WLog);
                _db.SaveChanges();
            }



        }

    }
}
