using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Endpoints.ProductEndpoints
{
    public class GetServiceResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ProductCategorySimple Categ { get; set; }

        public UoMSimple UOM { get; set; }

        public string DefaultCode { get; set; }

        public decimal? StandardPrice { get; set; }

        public decimal? LaboPrice { get; set; }

        public decimal? ListPrice { get; set; }

        public bool IsLabo { get; set; }

        public string Firm { get; set; }

        public IEnumerable<ProductStepSimple> Steps { get; set; } = new List<ProductStepSimple>();

        /// <summary>
        /// danh sách định mức vật tư
        /// </summary>
        public IEnumerable<ProductBomBasic> Boms { get; set; } = new List<ProductBomBasic>();
    }
}
