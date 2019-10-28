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
        public string Name { get; set; }

        public string NameNoSign { get; set; }

        /// <summary>
        /// Đơn vị chính của sản phẩm
        /// </summary>
        public Guid UOMId { get; set; }
        public UoM UOM { get; set; }

        /// <summary>
        /// Nhóm sản phẩm
        /// </summary>
        public Guid CategId { get; set; }
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
        /// </summary>
        public Guid UOMPOId { get; set; }
        public UoM UOMPO { get; set; }

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

        public string NameGet { get; set; }

        public ICollection<ProductCompanyRel> ProductCompanyRels { get; set; }

        public ICollection<StockQuant> StockQuants { get; set; }

        public bool IsLabo { get; set; }

        /// <summary>
        /// service: Dich vu
        /// product: Vat tu
        /// medicine: Thuoc
        /// </summary>
        public string Type2 { get; set; }
    }
}
