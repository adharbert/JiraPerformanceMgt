using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jira.BO.Models;

namespace Jira.BO.Services {
    public class UserService {

        private MainContext _db = null;
        private User _user = null;
        public int UserId { get; set; }
        

        public UserService(MainContext db) {
            _db = db;
        }
        public UserService() {
            _db = new MainContext();
        }
                
        public void Save(dynamic user) {
            if (user != null) {
                string userEmail = user.emailAddress.Value;
                var userObj = _db.Users.Where(u => u.EmailAddress == userEmail).SingleOrDefault();
                if (userObj == null) {
                    userObj = new User();
                    userObj.EmailAddress = userEmail;
                    userObj.Name = user.name.ToString();
                    userObj.SelfUrl = user.self.ToString();
                    userObj.CreateDt = DateTime.Now;
                    _db.Users.Add(userObj);
                }
                if (user.displayName != userObj.DisplayName) {
                    userObj.DisplayName = user.displayName.ToString();
                    userObj.ModifyDt = DateTime.Now;
                }
                _db.SaveChanges();
                UserId = userObj.UserID;
            } else {
                UserId = 0;
            }
        }
        

        

    }
}
