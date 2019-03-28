using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Jira.BO.Models {
    public partial class ProjectUser {

        [ForeignKey("ProjectID")]
        public virtual Project Project { get; set; }   
        [Key, Column(Order=0)]
        public int ProjectID { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        [Key, Column(Order=1)]
        public int UserID { get; set; }

    }
}
