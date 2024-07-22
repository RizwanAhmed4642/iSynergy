using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Finance.Models
{
    public class Bank
    {
        [Required]
        public int BankId { get; set; }
        [Required]
        [Display(Name = "Bank")]
        public string Name { get; set; }
    }
}