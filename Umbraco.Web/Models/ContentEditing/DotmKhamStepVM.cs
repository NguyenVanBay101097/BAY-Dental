using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class DotmKhamStepVM
    {
        public DotmKhamStepVM()
        {
            State = "draft";
        }
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        public int? Order { get; set; }

        public bool IsInclude { get; set; }

        public Guid ProductId { get; set; }
        public ProductSimple Product { get; set; }

        public Guid SaleLineId { get; set; }
        public SaleOrderLineBasic SaleLine { get; set; }

        public bool IsDone { get; set; }
    }
}
