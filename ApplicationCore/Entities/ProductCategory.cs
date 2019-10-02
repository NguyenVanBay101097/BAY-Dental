using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Nhóm dịch vụ
    /// </summary>
    public class ProductCategory: BaseEntity
    {
        public string Name { get; set; }

        public string CompleteName { get; set; }

        public Guid? ParentId { get; set; }
        public ProductCategory Parent { get; set; }

        public ICollection<ProductCategory> Childs { get; set; }

        public int? ParentLeft { get; set; }

        public int? ParentRight { get; set; }

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

        /// <summary>
        /// Nhóm công đoạn
        /// </summary>
        public bool StepCateg { get; set; }
    }
}
