using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Jira.BO.Models {
    public partial class User {

        [Key]
        public int UserID { get; set; }
        [Required, MaxLength(50, ErrorMessage="Name must be 50 characters or less.")]
        public string Name { get; set; }
        [Required, MaxLength(150, ErrorMessage="Email must be 150 characters or less.")]
        public string EmailAddress { get; set; }
        [MaxLength(100, ErrorMessage="Display Name must be 100 characters or less.")]
        public string DisplayName { get; set; }
        public string SelfUrl { get; set; }

        public DateTime CreateDt { get; set; }
        public DateTime ModifyDt { get; set; }

    }
}
