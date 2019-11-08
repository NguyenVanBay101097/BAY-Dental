using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Đơn vị chính của sản phẩm
        /// </summary>
        public Guid UOMId { get; set; }
        public UoMSimple UOM { get; set; }

        /// <summary>
        /// Nhóm sản phẩm
        /// </summary>
        public Guid CategId { get; set; }
        public ProductCategoryBasic Categ { get; set; }

        /// <summary>
        /// Giá niêm yết, chỗ này nên xử lý nhiều chi nhánh?? 
        /// Nếu các chi nhánh khác biệt giá bán sản phẩm thì nên có danh sách sản phẩm riêng cho từng chi nhánh
        /// </summary>
        public decimal ListPrice { get; set; }

        /// <summary>
        /// Can be sold
        /// </summary>
        public bool SaleOK { get; set; }

        public bool Active { get; set; }

        /// <summary>
        /// Đơn vị mua hàng
        /// </summary>
        public Guid UOMPOId { get; set; }
        public UoMSimple UOMPO { get; set; }

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

        public string DefaultCode { get; set; }

        public string NameGet { get; set; }

        public Guid? CompanyId { get; set; }

        public decimal QtyAvailable { get; set; }
    }

    public class ProductBasic2
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string NameNoSign { get; set; }

        public string CategName { get; set; }

        /// <summary>
        /// Giá niêm yết, chỗ này nên xử lý nhiều chi nhánh?? 
        /// Nếu các chi nhánh khác biệt giá bán sản phẩm thì nên có danh sách sản phẩm riêng cho từng chi nhánh
        /// </summary>
        public decimal ListPrice { get; set; }

        /// <summary>
        /// Loại sản phẩm
        /// ('consu', 'Consumable') không quản lý tồn kho
        /// product Có quản lý tồn kho
        /// service A service is a non-material product you provide.
        /// </summary>
        public string Type { get; set; }

        public string DefaultCode { get; set; }

        public decimal QtyAvailable { get; set; }

        public Guid CategId { get; set; }

        public bool SaleOK { get; set; }

        public bool PurchaseOK { get; set; }

        public bool KeToaOK { get; set; }

        public bool IsLabo { get; set; }

        public string Type2 { get; set; }

        public decimal? PurchasePrice { get; set; }
    }

    public class ProductPaged
    {
        public ProductPaged()
        {
            Limit = 20;
        }

        public string Search { get; set; }

        public bool? KeToaOK { get; set; }

        public bool? SaleOK { get; set; }

        public bool? PurchaseOK { get; set; }

        public bool? IsLabo { get; set; }

        public int Limit { get; set; }

        public int Offset { get; set; }

        public string Type { get; set; }

        public Guid? CategId { get; set; }

        public string Type2 { get; set; }
    }

    public class ProductImportExcelViewModel
    {
        public string FileBase64 { get; set; }
    }

    public class ProductImportExcelRow
    {
        public string Name { get; set; }

        public bool SaleOK { get; set; }

        public bool KeToaOK { get; set; }

        public string Type { get; set; }

        public string CategName { get; set; }

        public string DefaultCode { get; set; }

        public decimal ListPrice { get; set; }

        public decimal StandardPrice { get; set; }
    }
}
