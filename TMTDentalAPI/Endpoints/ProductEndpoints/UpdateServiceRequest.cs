using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.Endpoints.ProductEndpoints
{
    public class UpdateServiceRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? CategId { get; set; }

        public Guid UOMId { get; set; }

        public string DefaultCode { get; set; }

        public decimal? StandardPrice { get; set; }

        public decimal? LaboPrice { get; set; }

        public decimal? ListPrice { get; set; }

        public bool IsLabo { get; set; }

        public string Firm { get; set; }

        public IEnumerable<UpdateServiceRequestStep> Steps { get; set; } = new List<UpdateServiceRequestStep>();

        /// <summary>
        /// danh sách định mức vật tư
        /// </summary>
        public IEnumerable<UpdateServiceRequestBom> Boms { get; set; } = new List<UpdateServiceRequestBom>();
    }

    public class UpdateServiceRequestStep
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }
    }

    public class UpdateServiceRequestBom
    {
        public Guid? Id { get; set; }

        public Guid MaterialProductId { get; set; }

        public Guid ProductUomId { get; set; }

        public decimal Quantity { get; set; }
    }
}
