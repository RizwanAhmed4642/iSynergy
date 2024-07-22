using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Inventory.Models
{
    /// <summary>
    /// This class represents hardware items like 
    /// laptops, desktops and servers.
    /// </summary>
    public class Computer : Device
    {
        [Required(ErrorMessage = "Please provide processor details")]
        [Display(Name = "Processor")]
        public string Processor { get; set; }

        [Required(ErrorMessage = "Please provide RAM details")]
        [Display(Name = "RAM")]
        public string RAM { get; set; }

        [Required(ErrorMessage = "Please provide hard disk details")]
        [Display(Name = "Hard Disk")]
        public string HardDisk { get; set; }

    }
}