using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Jira.BO.Models {
    public partial class Project {

        [Key]
        public int ProjectID { get; set; }
        public int RestID { get; set; }
        [Required, MaxLength(8, ErrorMessage="Key must be 8 characters or less"), MinLength(1)]
        public string Abbr { get; set; }
        [Required, MaxLength(60, ErrorMessage="Name must be between 3 to 60 characters."), MinLength(3)]
        public string Name { get; set; }
        [MaxLength(500, ErrorMessage = "Self must be 500 characters or less"), MinLength(10)] 
        public string SelfUrl { get; set; }
        [MaxLength(ErrorMessage="Description too large.")]
        public string Description { get; set; }
        public string SmallDetail { get; set; }
        public DateTime CreateDt { get; set; }
        public DateTime ModifyDt { get; set; }
        public virtual ICollection<Item> Items { get; set; }

    }

}
