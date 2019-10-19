using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    /// <summary>
    /// The product category model used for paging and listing users in the UI
    /// </summary>
    public class ProductCategoryBasic
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string CompleteName { get; set; }

        public Guid? ParentId { get; set; }
        public ProductCategorySimple Parent { get; set; }
    }

    public class ProductCategoryPaged
    {
        public ProductCategoryPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

        /// <summary>
        /// Là nhóm dịch vụ
        /// </summary>
        public bool? ServiceCateg { get; set; }

        /// <summary>
        /// Là nhóm thuốc
        /// </summary>
        public bool? MedicineCateg { get; set; }

        /// <summary>
        /// Là nhóm labo
        /// </summary>
        public bool? LaboCateg { get; set; }

        /// <summary>
        /// Nhóm hàng hóa vật tư
        /// </summary>
        public bool? ProductCateg { get; set; }
    }
}
