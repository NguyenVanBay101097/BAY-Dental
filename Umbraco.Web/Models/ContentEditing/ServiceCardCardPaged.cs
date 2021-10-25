using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ServiceCardCardPaged
    {
        public ServiceCardCardPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

        public Guid? OrderId { get; set; }
        public DateTime? ActivatedDate { get; set; }
        public DateTime? ActivatedDateFrom { get; set; }
        public DateTime? ActivatedDateTo { get; set; }
        public DateTime? ExpiredDateFrom { get; set; }
        public DateTime? ExpiredDateTo { get; set; }
        public string state { get; set; }
    }
}
