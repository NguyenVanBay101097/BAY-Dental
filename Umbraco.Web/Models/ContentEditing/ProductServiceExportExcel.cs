using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductServiceExportExcel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool? IsLabo { get; set; }
        /// <summary>
        /// tên nhóm dịch vụ
        /// </summary>
        public string CategName { get; set; }

        /// <summary>
        /// Mã dịch vụ
        /// </summary>
        public string DefaultCode { get; set; }

        /// <summary>
        /// Giá bán
        /// </summary>
        public decimal? ListPrice { get; set; }

        public string Type { get; set; }

        public decimal QtyAvailable { get; set; }
        /// <summary>
        /// Giá đặt labo
        /// </summary>
        public decimal? PurchasePrice { get; set; }

        public decimal? LaboPrice { get; set; }

        public IEnumerable<ProductStepSimple> StepList { get; set; } = new List<ProductStepSimple>();

        /// <summary>
        /// Hãng
        /// </summary>
        public string Firm { get; set; }

        /// <summary>
        /// Giá vốn
        /// </summary>
        public decimal? StandardPrice { get; set; }
    }

    public class ProductProductExportExcel
    {
        public string Name { get; set; }

        public string DefaultCode { get; set; }

        public string Type { get; set; }

        public string CategName { get; set; }

        public decimal? PurchasePrice { get; set; }

        public decimal? ListPrice { get; set; }

        public string UomName { get; set; }

        public string UomPoName { get; set; }

        public decimal? MinInventory { get; set; }
    }
}
