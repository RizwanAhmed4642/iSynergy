using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.DMS.Models
{
    public class AttributeDataListItem
    {
        [Key]
        public int AttributeDataListItemId { get; set; }

        public string Value { get; set; }
        public string Text { get; set; }


        public int AttributeDataListId { get; set; }

        [ForeignKey("AttributeDataListId")]
        public virtual AttributeDataList AttributeDataList { get; set; }
    }
}