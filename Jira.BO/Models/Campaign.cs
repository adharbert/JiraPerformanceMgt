using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Jira.BO.Models {
    public class Campaign {
        [Key]
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string Abbrev { get; set; }
        public bool IsActive { get; set; }

    }
}
