using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class StockInventoryCriteriaPaged
    {
        public StockInventoryCriteriaPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

    }

    public class StockInventoryCriteriaBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }

    }

    public class StockInventoryCriteriaSave
    {
        public string Name { get; set; }
        public string Note { get; set; }

    }

    public class StockInventoryCriteriaDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }

    }

    public class StockInventoryCriteriaSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

    }


}
