using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TCareReports
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        public int Items { get; set; }
        public int MessageTotal { get; set; }
        public int DeliveryTotal { get; set; }
        public int ReadTotal { get; set; }
    }

    public class TCareScenarioFilterReport
    {
        public Guid? TCareScenarioId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class TCareReportsItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int MessageTotal { get; set; }
        public int DeliveryTotal { get; set; }
        public int ReadTotal { get; set; }
        public bool Active { get; set; }
    }
}
