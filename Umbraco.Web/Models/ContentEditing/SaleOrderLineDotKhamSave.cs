using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderLineDotKhamSave
    {
        public Guid Id { get; set; }
        public IList<DotKhamStepSave> Steps { get; set; } = new List<DotKhamStepSave>();
        public bool Default { get; set; }

    }
}
