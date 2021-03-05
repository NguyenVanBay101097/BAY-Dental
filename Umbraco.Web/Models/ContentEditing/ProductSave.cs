using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductSave
    {
        public ProductSave()
        {
            Type = "consu";
            SaleOK = true;
            PurchaseOK = true;
            LaboPrice = 0;
            PurchasePrice = 0;
        }

        public string Name { get; set; }

        public Guid? CategId { get; set; }

        public Guid UOMId { get; set; }

        public Guid UOMPOId { get; set; }

        public string Type { get; set; }

        public string DefaultCode { get; set; }

        public string Type2 { get; set; }

        public double StandardPrice { get; set; }

        public decimal? PurchasePrice { get; set; }

        public decimal? LaboPrice { get; set; }

        public bool SaleOK { get; set; }

        public bool PurchaseOK { get; set; }

        public bool KeToaOk { get; set; }

        public bool IsLabo { get; set; }

        public Guid? CompanyId { get; set; }

        public decimal ListPrice { get; set; }

        public IEnumerable<ProductStepDisplay> StepList { get; set; } = new List<ProductStepDisplay>();
        
        public string Firm { get; set; }

        public IEnumerable<ProductStockInventoryCriteriaRel> ProductStockInventoryCriteriaRels { get; set; } = new LinkedList<ProductStockInventoryCriteriaRel>();
    }
}
