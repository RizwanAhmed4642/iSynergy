using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Inventory.Models
{
    /// <summary>
    /// This clas represents hardware items like 
    /// keyboard, mouse, hard drive, pen drive, LCD and netowrking devices etc...
    /// </summary>
    public class Device : Asset
    {
        [Required(ErrorMessage = "Please provide serial number")]
        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
    }
}