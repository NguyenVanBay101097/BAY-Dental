using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ToothCategoryViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public int? Sequence { get; set; }
    }
}
