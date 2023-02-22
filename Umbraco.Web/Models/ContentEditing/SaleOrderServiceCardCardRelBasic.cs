using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderServiceCardCardRelBasic
    {
        public string CardName { get; set; }

        public string SaleOrderName { get; set; }

        public string SaleOrderId { get; set; }

        public decimal? Amount { get; set; }

        public DateTime? DateCreated { get; set; }
    }
}
