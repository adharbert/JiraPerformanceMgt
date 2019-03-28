using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jira.BO.Models;

namespace Jira.BO.Services {
    public class ProjectUserService {

        private MainContext _db = null;

        public ProjectUserService(MainContext db) {
            _db = db;
        }
        public ProjectUserService() {
            _db = new MainContext();
        }


        public void Save(int UserId, int ProjectId) {

            if (UserId > 0) {
                var pu = _db.ProjectUsers.Where(p => p.ProjectID == ProjectId && p.UserID == UserId).SingleOrDefault();
                if (pu == null) {
                    pu = new ProjectUser();
                    pu.UserID = UserId;
                    pu.ProjectID = ProjectId;
                    _db.ProjectUsers.Add(pu);
                    _db.SaveChanges();
                }
            }
        }


    }
}
