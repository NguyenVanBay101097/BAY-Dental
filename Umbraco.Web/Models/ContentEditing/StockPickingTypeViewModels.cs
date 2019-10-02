using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class StockPickingTypeBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class StockPickingTypePaged
    {
        public StockPickingTypePaged()
        {
            Limit = 20;
        }

        public int Limit { get; set; }

        public int Offset { get; set; }

        public string Search { get; set; }
    }
}
