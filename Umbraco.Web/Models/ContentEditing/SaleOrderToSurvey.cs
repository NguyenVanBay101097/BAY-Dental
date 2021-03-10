using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderToSurvey
    {
        public string PartnerRef { get; set; }

        public string PartnerName { get; set; }

        public string PartnerPhone { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime DateOrder { get; set; }

        public DateTime? DateDone { get; set; }
    }

    public class SaleOrderToSurveyFilter
    {
        public string Search { get; set; }

        public int Limit { get; set; }

        public int Offset { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }
}
