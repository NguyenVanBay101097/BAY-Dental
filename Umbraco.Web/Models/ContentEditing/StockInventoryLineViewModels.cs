using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class StockInventoryLineOnChangeCreateLine
    {
        public Guid ProductId { get; set; }

        public Guid LocationId { get; set; }
    }
}
