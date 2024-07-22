using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.DMS.Models
{
    public class DocumentAttributeType
    {
        [Key]
        [Display(Name = "Data Type Id")]
        public int DocumentAttributeTypeId { get; set; }

        [Required(ErrorMessage = "Please select a data type")]
        [Display(Name = "Data Type")]
        public DocumentAttributeDataType DataType { get; set; }
    }
    public enum DocumentAttributeDataType
    {
        Number,
        Text,
        [Display(Name = "Large Text")]
        TextArea,
        Date,
        Time,
        Currency,
        Boolean,
    }
}
