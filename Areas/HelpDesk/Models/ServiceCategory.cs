using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.HelpDesk.Models
{
    public class ServiceCategory
    {
        [Key]
        [Display(Name = "Service Category ID")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Service Category")]
        public string Name { get; set; }
    }
}