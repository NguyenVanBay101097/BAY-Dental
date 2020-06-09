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

        public IEnumerable<ProductStepSimple> StepList { get; set; } = new List<ProductStepSimple>();
    }
}
