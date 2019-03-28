using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jira.BO.HttpUtils;
using cfm = System.Configuration.ConfigurationManager;

namespace Jira.BO.Services {
    public class JiraService {
        
        private string _baseurl = cfm.AppSettings["RestURL"];
        public string RestURL {
            get { return _baseurl; }
            set { _baseurl = value; }
        }
       
        private RestClient restClient = null;

        public JiraService(RestClient restClient) {
            this.restClient = restClient;
        }

        public JiraService() {
            this.restClient = new RestClient("");
        }


#region " ***** project details ****** "

        public Task<string> GetLatestProjectDataJSONAsync() {
            Task<string> latestProjectTask = Task<string>.Factory.StartNew(() => {
                return GetLatestProjectDataJSON();
            });
            return latestProjectTask;
        }

        public string GetLatestProjectDataJSON() {
            return restClient.MakeRequestFromURI(RestURL + "latest/project");
        }

        public Task<string> GetProjectDetailsAsync(string projectID) {
            Task<string> projectDetailsTask = Task<string>.Factory.StartNew(() => {
                return GetProjectDetails(projectID);
            });
            return projectDetailsTask;
        }

        public string GetProjectDetails(string projectID) {   
            return restClient.MakeRequestFromURI(RestURL + "2/project/" + projectID);
        }

#endregion


#region " ***** item details ****** "

        public Task<string> GetIssueListAsync(string projectAbbr) {
            Task<string> issueListTask = Task<string>.Factory.StartNew(() => {
                return GetIssueList(projectAbbr);
            });
            return issueListTask;
        }
        
        public string GetIssueList(string projectAbbr) {
            return restClient.MakeRequestFromURI(RestURL + string.Format("2/search?jql=project={0}", projectAbbr));     
            //return restClient.MakeRequestFromURI(RestURL + string.Format("2/project/{0}/properties", projectAbbr));     
        }


        public Task<string> GetIssueDetailsAsync(string issueId) {
            Task<string> issueDetailListTask = Task<string>.Factory.StartNew(() => {
                return GetIssueDetails(issueId);
            });
            return issueDetailListTask;
        }

        public string GetIssueDetails(string issueId) {
            return restClient.MakeRequestFromURI(RestURL + string.Format("2/issue/{0}", issueId));     
        }

#endregion


#region " ***** user details ****** "

    

#endregion



        public string GetSelfResults(string self) {
            return restClient.MakeRequestFromURI(self);
        }





    }
}
