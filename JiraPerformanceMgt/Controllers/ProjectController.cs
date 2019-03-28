using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Jira.BO.Services;
using Jira.BO.Models;

namespace JiraPerformanceMgt.Controllers {
    public class ProjectController : Controller {

        private ProjectService _projectservice;
        private ItemService _itemservice;
                

        public ActionResult Index() {
            return RedirectToAction("List");
        }

        public ActionResult List() {
            _projectservice = new ProjectService();
            var projects = _projectservice.GetList();
            return View("List", projects);
        }

        public ActionResult ProjectDetail(int Id) {
            _projectservice = new ProjectService();
            _itemservice = new ItemService();
            var project = _projectservice.GetById(Id);
            if (project != null) {
                var items = _itemservice.GetListByProjectId(Id).OrderByDescending(i => i.DueDt).ToList();
                ViewBag.Title = "Project Detail " + project.Abbr;
                return View("ProjectDetail", items);
            } else {
                ViewBag.Error = "No project found with this ID. Please try again.";
                return List();
            }
        }


        public ActionResult WorkLog(int Id) {
            return List();
        }



    }
}
