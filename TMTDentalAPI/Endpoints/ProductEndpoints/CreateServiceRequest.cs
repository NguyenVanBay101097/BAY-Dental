using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.Endpoints.ProductEndpoints
{
    public class CreateServiceRequest
    {
        public string Name { get; set; }

        public Guid? CategId { get; set; }

        public Guid UOMId { get; set; }

        public string DefaultCode { get; set; }

        public decimal? StandardPrice { get; set; }

        public decimal? LaboPrice { get; set; }

        public decimal? ListPrice { get; set; }

        public bool IsLabo { get; set; }

        public string Firm { get; set; }

        public IEnumerable<CreateServiceRequestStep> StepList { get; set; } = new List<CreateServiceRequestStep>();

        /// <summary>
        /// danh sách định mức vật tư
        /// </summary>
        public IEnumerable<CreateServiceRequestBom> Boms { get; set; } = new List<CreateServiceRequestBom>();
    }

    public class CreateServiceRequestStep
    {
        public string Name { get; set; }
    }

    public class CreateServiceRequestBom
    {
        public Guid? MaterialProductId { get; set; }

        public decimal Quantity { get; set; }
    }
}
