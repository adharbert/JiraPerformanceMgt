using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Jira.BO.Models {
    public partial class UserItemWorklog {

        [Key]
        public int UserItemWorklogID{ get; set; }

        public int WorkLogID { get; set; }
        //log.id.Value

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        public int UserID { get; set; }

        [ForeignKey("ItemID")]
        public virtual Item Item { get; set; }
        public int ItemID { get; set; }

        public long TimeSpentInSeconds { get; set; }
        //log.timeSpentSeconds.Value

        public DateTime CreateDt { get; set; }

    }
}
