using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NameNoSign { get; set; }
        public Guid CategId { get; set; }
        //public ProductCategory Categ { get; set; }
        public decimal ListPrice { get; set; }
        public bool SaleOK { get; set; }
        public bool KeToaOK { get; set; }
        public bool Active { get; set; }
        public Guid UOMPOId { get; set; }
        //public UoM UOMPO { get; set; }
        public Guid UOMId { get; set; }
        //public UoM UOM { get; set; }
        //public ICollection<ProductUoMRel> ProductUoMRels { get; set; } = new List<ProductUoMRel>();
        public string Type { get; set; }
        public bool PurchaseOK { get; set; }
        public string Description { get; set; }
        public string KeToaNote { get; set; }
        public Guid? CompanyId { get; set; }
        //public Company Company { get; set; }
        public string DefaultCode { get; set; }
        public string NameGet { get; set; }
        public bool IsLabo { get; set; }
        public string Type2 { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? LaboPrice { get; set; }
    }
}
