using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jira.BO.Models;

namespace Jira.BO.Services {
    public class ProjectService {

        private MainContext _db = null;

        public ProjectService(MainContext db) {
            _db = db;
        }

        public ProjectService() {
            _db = new MainContext();
            
        }

        public void Save(dynamic project) {
            int RestID = int.Parse(project.id.Value);
            string Abbr = project.key.Value;
            var proj = _db.Projects.Where(p => p.RestID == RestID && p.Abbr == Abbr).SingleOrDefault();
            if (proj == null) {
                proj = new Project();
                proj.Abbr = Abbr;
                proj.RestID = RestID;
                proj.CreateDt = DateTime.Now;
                _db.Projects.Add(proj);
            }
            if (proj.Name != project.name.ToString() && proj.SelfUrl != project.self.ToString() && proj.Description != project.description.ToString()) {
                proj.Name = project.name.ToString();
                proj.SelfUrl = project.self.ToString();
                proj.ModifyDt = DateTime.Now;
                proj.Description = project.description.ToString();
                _db.SaveChanges();
            }

        }



        public IEnumerable<Project> GetList() {
            return _db.Projects.ToList();
        }

        public Project GetById(int Id) {
            return _db.Projects.Where(p => p.ProjectID == Id).FirstOrDefault();
        }

    }
}
