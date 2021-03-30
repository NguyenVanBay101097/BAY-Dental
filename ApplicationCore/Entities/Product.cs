using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Dịch vụ
    /// </summary>
    public class Product: BaseEntity
    {
        public Product()
        {
            Active = true;
            SaleOK = true;
            PurchaseOK = true;
            Type = "consu";
        }
        public string Name { get; set; }

        public string NameNoSign { get; set; }

      
        /// <summary>
        /// Nhóm sản phẩm
        /// </summary>
        public Guid? CategId { get; set; }
        public ProductCategory Categ { get; set; }

        /// <summary>
        /// Giá niêm yết, chỗ này nên xử lý nhiều chi nhánh?? 
        /// Nếu các chi nhánh khác biệt giá bán sản phẩm thì nên có danh sách sản phẩm riêng cho từng chi nhánh
        /// </summary>
        public decimal ListPrice { get; set; }

        /// <summary>
        /// Can be sold
        /// </summary>
        public bool SaleOK { get; set; }

        /// <summary>
        /// Là thuốc
        /// </summary>
        public bool KeToaOK { get; set; }

        public bool Active { get; set; }

        /// <summary>
        /// Đơn vị mua hàng
        /// uomPo là đơn vị mua
        /// uom đơn vị mặc định
        /// </summary>
        public Guid UOMPOId { get; set; }
        public UoM UOMPO { get; set; }

        /// <summary>
        /// Đơn vị chính của sản phẩm
        /// </summary>
        public Guid UOMId { get; set; }
        public UoM UOM { get; set; }

        /// <summary>
        /// Danh sách UoMs được chọn trong việc mua hàng
        /// </summary>
        public ICollection<ProductUoMRel> ProductUoMRels { get; set; } = new List<ProductUoMRel>();

        /// <summary>
        /// Loại sản phẩm
        /// ('consu', 'Consumable') không quản lý tồn kho
        /// product Có quản lý tồn kho
        /// service A service is a non-material product you provide.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Can be Puschase
        /// </summary>
        public bool PurchaseOK { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Ghi hướng dẫn sử dụng khi tạo toa thuốc
        /// </summary>
        public string KeToaNote { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public string DefaultCode { get; set; }

        /// <summary>
        /// Cột này không có sử dụng, sẽ bị xóa trong tương lai
        /// </summary>
        public string NameGet { get; set; }

        public ICollection<ProductCompanyRel> ProductCompanyRels { get; set; }

        public ICollection<StockQuant> StockQuants { get; set; }

        public ICollection<ProductPriceHistory> PriceHistories { get; set; } = new List<ProductPriceHistory>();

        public ICollection<ProductStep> Steps { get; set; } = new List<ProductStep>();

        /// <summary>
        /// danh sách định mức vật tư
        /// </summary>
        public ICollection<ProductBom> Boms { get; set; } = new List<ProductBom>();

        public bool IsLabo { get; set; }
 
        /// <summary>
        /// service: Dich vu
        /// product: Vat tu
        /// medicine: Thuoc
        /// labo: vật liệu Labo
        /// labo_attach: gửu kèm labo
        /// </summary>
        public string Type2 { get; set; }

        /// <summary>
        /// Giá mua
        /// </summary>
        public decimal? PurchasePrice { get; set; }

        /// <summary>
        /// Giá đặt mua labo
        /// </summary>
        public decimal? LaboPrice { get; set; }


        /// <summary>
        /// hãng:  nếu là labo thì hãng nào ?
        /// </summary>
        public string Firm { get; set; }

        public ICollection<ProductStockInventoryCriteriaRel> ProductStockInventoryCriteriaRels { get; set; } = new List<ProductStockInventoryCriteriaRel>();

        public ICollection<AdvisoryProductRel> AdvisoryProductRels { get; set; } = new List<AdvisoryProductRel>();
    }
}
