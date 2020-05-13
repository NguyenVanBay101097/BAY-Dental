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
            Active = true;
            SaleOK = true;
            PurchaseOK = true;
            LaboPrice = 0;
            PurchasePrice = 0;
        }
        public string Name { get; set; }
        public Guid? CategId { get; set; }
        public Guid? UOMId { get; set; }
        public Guid? UOMPOId { get; set; }
        public string Type { get; set; }
        public string DefaultCode { get; set; }
        public string Type2 { get; set; }
        public double StandardPrice { get; set; }
        public string keToaNote { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? LaboPrice { get; set; }
        public bool SaleOK { get; set; }

        public bool PurchaseOK { get; set; }
        public bool KeToaOk { get; set; }
        public bool Active { get; set; }
        public bool IsLabo { get; set; }
        public IEnumerable<Guid> UoMIds { get; set; } = new List<Guid>();
        public Guid? CompanyId { get; set; }
        /// <summary>
        /// Giá niêm yết, chỗ này nên xử lý nhiều chi nhánh?? 
        /// Nếu các chi nhánh khác biệt giá bán sản phẩm thì nên có danh sách sản phẩm riêng cho từng chi nhánh
        /// </summary>
        public decimal ListPrice { get; set; }
        public IEnumerable<ProductStepDisplay> StepList { get; set; } = new List<ProductStepDisplay>();
    }
}
