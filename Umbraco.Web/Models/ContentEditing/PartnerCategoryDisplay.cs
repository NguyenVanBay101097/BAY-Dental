using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerCategoryDisplay
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public Guid? ParentId { get; set; }
        public PartnerCategoryBasic Parent { get; set; }

        public string Color { get; set; }
    }
}
