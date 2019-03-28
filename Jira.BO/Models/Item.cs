using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Jira.BO.Models {
    public partial class Item {

        [Key]
        public int ItemID { get; set; }

        public int RestID { get; set; }        

        [ForeignKey("ProjectID")]
        public virtual Project Project { get; set; }       
        public int ProjectID { get; set; }
        
        [Required, MaxLength(12, ErrorMessage = "Key must be 8 characters or less")]
        public string Abbr { get; set; }
        
        [MaxLength(500, ErrorMessage = "Self must be 500 characters or less")] 
        public string SelfUrl { get; set; }
        
        [MaxLength(500, ErrorMessage = "Summary must be 500 characters or less")] 
        public string Title { get; set; }
        
        [MaxLength(4000, ErrorMessage = "Summary must be 2000 characters or less")]
        public string Description { get; set; }
                
        public ItemType ItemType { get; set; }
        
        public ItemStatus ItemStatus { get; set; }

        [ForeignKey("AssignedUserID")]        
        public virtual User User { get; set; }        
        public int AssignedUserID { get; set; }
        
        public long OriginalEstimateTimeInSeconds { get; set; }        
        
        public long RemainingTimeInSeconds { get; set; }        
        
        public long TimeSpentInSeconds { get; set; }
        
        public int PercentDone { get; set; }        

        public DateTime CreateDt { get; set; }        

        public DateTime ModifyDt { get; set; }        

        public DateTime? DueDt { get; set; }

        

        // Virtual properties not added to database.
        public virtual ICollection<UserItemWorklog> WorkLogs { get; set; }

        public virtual string FormattedDueDt {
            get {
                if (DueDt != null) {
                    return DueDt.Value.ToString("MM/dd/yyyy");
                } else {
                    return "";
                }

            }
        }

        public virtual double OrigianlEstimageInHours {
            get {
                return (OriginalEstimateTimeInSeconds > 0) ? (OriginalEstimateTimeInSeconds / 60) / 60 : 0;
            }
        }

        public virtual double RemainingTimeInHours {
            get {
                return (RemainingTimeInSeconds > 0) ? (RemainingTimeInSeconds / 60) / 60 : 0;
            }
        }

        public virtual string ItemTypeName {
            get {
                return ((ItemType)ItemType).ToString();
            }
        }


    }
}
