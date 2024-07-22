using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.DMS.Models
{
    public class DocumentMetaData
    {
        [Key]
        public int DocumentMetaDataId { get; set; }

        [Required(ErrorMessage = "Please select a document.")]
        public int DocumentId { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }
        public int DocumentAttributeId { get; set; }
        public string AttributeValue { get; set; }
    }
}