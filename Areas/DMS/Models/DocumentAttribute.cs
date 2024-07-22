using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.DMS.Models
{
    public class DocumentAttribute
    {

        [Key]
        [Display(Name = "Attribute Id")]
        public int DocumentAttributeId { get; set; }


        [Required(ErrorMessage = "Please provide a name for the attribute")]
        [Display(Name = "Attribute Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        [Display(Name = "Category")]
        public int DocumentCategoryId { get; set; }


        [ForeignKey("DocumentCategoryId")]
        [Display(Name = "Category")]
        public virtual DocumentCategory DocumentCategory { get; set; }

        [Display(Name = "Data Type")]
        public int DocumentAttributeTypeId { get; set; }

        [ForeignKey("DocumentAttributeTypeId")]
        [Display(Name = "Data Type")]
        public virtual DocumentAttributeType DocumentAttributeType { get; set; }

        [Display(Name = "Requires List")]
        public bool isMappedToDataList { get; set; }


        [Display(Name = "List")]
        public int? AttributeDataListId { get; set; }

        [ForeignKey("AttributeDataListId")]
        [Display(Name = "List")]
        public virtual AttributeDataList AttributeDataList { get; set; }

        public DocumentAttribute()
        {
            isMappedToDataList = false;
        }
    }
}