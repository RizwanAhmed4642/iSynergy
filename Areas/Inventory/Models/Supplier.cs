using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Inventory.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        [Required(ErrorMessage = "Please provide supplier name")]
        [Display(Name = "Supplier")]
        public string Name { get; set; }
        public string Address { get; set; }
        [Display(Name = "Land Line")]
        public string LandLine { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }
}