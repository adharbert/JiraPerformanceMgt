using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jira.BO.HttpUtils;
using Jira.BO.Models;
using Jira.BO.Services;
using Newtonsoft.Json;

namespace TestApp {
    class Program {
        /*

            https://nttdata.atlassian.net/rest/api/2/search?jql=project=IPT            

        */
        static void Main(string[] args) {
            
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<MainContext>());
            //Database.SetInitializer(new CreateDatabaseIfNotExists<MainContext>());
            Database.SetInitializer(new MainContext.Initializer());
            ImportServices ise = new ImportServices();
            ise.Start();
           
        }
    }
}
