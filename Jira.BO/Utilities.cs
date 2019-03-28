using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cfm = System.Configuration.ConfigurationManager;

namespace Jira.BO {
    public static class Utilities {

        public static int MaxPool {
            get { return int.Parse(cfm.AppSettings["MaxPool"]); }
        }

    }
}
