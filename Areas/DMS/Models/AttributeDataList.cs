using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.DMS.Models
{
    public class AttributeDataList
    {
        [Key]
        [Display(Name = "List Id")]
        public int AttributeDataListId { get; set; }

        [Required(ErrorMessage ="Please provide a unique name for the list.")]
        [Display(Name = "List Name")]
        public string Name { get; set; }

    }
}