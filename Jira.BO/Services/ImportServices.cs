using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Jira.BO.Services {

    public enum ObjectType {
        Project = 1
        , Item = 2
        , User = 3
    }
    

    public class WorkerPayload {
        public object ProjectData { get; set; }
    }

    public class SmallProject {

        public SmallProject() { }

        public SmallProject(string jsonObj) {
            JObject o = JObject.Parse(jsonObj);
            ProjectID = o["id"].ToString();
            Key = o["key"].ToString();
        }
        public string ProjectID { get; set; }
        public string Key { get; set; }
    }
    
    public class ImportWorker {
        ManualResetEvent ev;
        int id;
        public string ObjectId { get; set; }
        public WorkerPayload Payload { get; set; }
        public string ForignKeyId { get; set; }
        public string Self { get; set; }

        public ImportWorker(ManualResetEvent mrEvent, int index, string objectId) {
            id = index;
            ev = mrEvent;
            ObjectId = objectId;
        }
        public ImportWorker(ManualResetEvent mrEvent, int index, string objectId, string forignkeyId) {
            id = index;
            ev = mrEvent;
            ObjectId = objectId;
            ForignKeyId = forignkeyId;
        }
        public ImportWorker(ManualResetEvent mrEvent, int index, string objectId, string forignkeyId, string self) {
            id = index;
            ev = mrEvent;
            ObjectId = objectId;
            ForignKeyId = forignkeyId;
            Self = self;
        }

        public void DoWorkProject() {
            var jiraService = new JiraService();
            var projectService = new ProjectService();
            var jsonProject = JsonConvert.DeserializeObject(jiraService.GetProjectDetails(ObjectId));
            Payload = new WorkerPayload() {
                ProjectData = jsonProject
            };
            projectService.Save(jsonProject);
            ev.Set();
        }

        public void DoWorkItemUser() {
            var jiraService = new JiraService();
            var itemService = new ItemService();
            var jsonProject = JsonConvert.DeserializeObject(jiraService.GetSelfResults(Self));
            Payload = new WorkerPayload() {
                ProjectData = jsonProject
            };
            itemService.Save(jsonProject, int.Parse(ForignKeyId));
            ev.Set();
        }

    }



    public class ImportServices {

        private JiraService jser = null;
        private ProjectService pser = null;
        private int MaxPool = Utilities.MaxPool;    // For threading, set a max pool amount. In case there are a lot of items.

        public ImportServices() {
            jser = new JiraService();
            pser = new ProjectService();
        }


        public void Start() {
            var jsonProjects = Phase1();
            foreach (var projItem in jsonProjects) {
                WorkerPayload wp = (WorkerPayload)projItem;
                //SmallProject sp = new SmallProject() { Key = wp["key"], ProjectID = wp["id"] };
                SmallProject sp = new SmallProject(wp.ProjectData.ToString());
                //SmallProject sp = (SmallProject)projItem;
                Phase2(sp);
            }
        }


        private IEnumerable<object> Phase1() {
            var jiraService = new JiraService();

            ManualResetEvent[] events = Enumerable.Range(1, 10).Select(e => new ManualResetEvent(true)).ToArray();
            ImportWorker[] workers = new ImportWorker[10];
            Thread[] threads = new Thread[10];

            var projectDataJson = jiraService.GetLatestProjectDataJSON();
            dynamic obj = JsonConvert.DeserializeObject(projectDataJson);
            int total = obj.Count;
            int current = 0;
            var countToAssign = total > MaxPool ? MaxPool : total;

            List<object> jsonProjects = new List<object>();

            for (int i = 0; i < countToAssign; i++) {
                StartProjectWorker(events, workers, threads, i, obj[current].id.Value);
                current++;
            }

            while (current < total) {
                int index = WaitHandle.WaitAny(events);
                jsonProjects.Add(workers[index].Payload);
                StartProjectWorker(events, workers, threads, index, obj[current].id.Value);
                current++;
            }

            WaitHandle.WaitAll(events);
            for (var i = 0; i < workers.Length; i++) {
                if (workers[i] != null && workers[i].Payload != null) {
                    dynamic pj = workers[i].Payload.ProjectData;
                    jsonProjects.Add(new SmallProject() { Key = pj.key.Value, ProjectID = pj.id.Value });
                }
            }
            // Getting project list to start phase 2, to capture issue, user and work log information.
            return jsonProjects;
        }



        private void Phase2(SmallProject sp) {
            var jiraService = new JiraService();

            ManualResetEvent[] events = Enumerable.Range(1, 10).Select(e => new ManualResetEvent(true)).ToArray();
            ImportWorker[] workers = new ImportWorker[10];
            Thread[] threads = new Thread[10];

            // Get All Issues for project.
            //var itemDataJson = jiraService.GetIssueList(sp.ProjectID);
            var itemDataJson = jiraService.GetIssueList(sp.Key);
            dynamic obj = JsonConvert.DeserializeObject(itemDataJson);
            int current = 0;
            int total = obj.issues.Count;
            var countToAssign = total > MaxPool ? MaxPool : total;

            // if there are no issues, just skip through.
            if (obj.issues.Count > 0) {
                for (int i = 0; i < total; i++) {
                    dynamic jsonProject = JsonConvert.DeserializeObject(jiraService.GetSelfResults(obj.issues[i].self.Value));
                    var itemService = new ItemService();
                    itemService.Save(jsonProject, int.Parse(sp.ProjectID));
                }




                //List<object> jsonProjects = new List<object>();
                
                //// Return specific details for each issue                
                //for (int i = 0; i < countToAssign; i++) {
                //    StartIssueWorker(events, workers, threads, i, obj.issues[i].id.Value, sp.ProjectID, obj.issues[i].self.Value);
                //    current++;
                //}

                //while (current < total) {
                //    int index = WaitHandle.WaitAny(events);
                //    jsonProjects.Add(workers[index].Payload);
                //    StartIssueWorker(events, workers, threads, index, obj.issues[index].id.Value, sp.ProjectID, obj.issues[index].self.Value);
                //    current++;
                //}
                //WaitHandle.WaitAll(events);
                //for (var i = 0; i < workers.Length; i++) {
                //    if (workers[i] != null && workers[i].Payload != null) {
                //        jsonProjects.Add(workers[i].Payload);
                //    }
                //}     
            }
        }



        private static void StartProjectWorker(ManualResetEvent[] events, ImportWorker[] workers, Thread[] threads, int i, string projectId) {
            events[i] = new ManualResetEvent(false);
            workers[i] = new ImportWorker(events[i], i, projectId);
            threads[i] = new Thread(new ThreadStart(workers[i].DoWorkProject));
            threads[i].Start();
        }

        private static void StartIssueWorker(ManualResetEvent[] events, ImportWorker[] workers, Thread[] threads, int i, string issueId, string projectId, string self) {
            events[i] = new ManualResetEvent(false);
            workers[i] = new ImportWorker(events[i], i, issueId, projectId, self);            
            threads[i] = new Thread(new ThreadStart(workers[i].DoWorkItemUser));
            threads[i].Start();
        }


    }
}
