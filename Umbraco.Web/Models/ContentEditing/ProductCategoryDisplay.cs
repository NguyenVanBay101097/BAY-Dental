using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductCategoryDisplay
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public Guid? ParentId { get; set; }
        public ProductCategorySimple Parent { get; set; }

        public int? Sequence { get; set; }

        /// <summary>
        /// Là nhóm dịch vụ
        /// </summary>
        public bool ServiceCateg { get; set; }

        /// <summary>
        /// Là nhóm thuốc
        /// </summary>
        public bool MedicineCateg { get; set; }

        /// <summary>
        /// Là nhóm labo
        /// </summary>
        public bool LaboCateg { get; set; }

        /// <summary>
        /// Nhóm hàng hóa vật tư
        /// </summary>
        public bool ProductCateg { get; set; }

        public string CompleteName { get; set; }
    }

    public class ProductCategoryFilter
    {
        public string Search { get; set; }
    }
}
