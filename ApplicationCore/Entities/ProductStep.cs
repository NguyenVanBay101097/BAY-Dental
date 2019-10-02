using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ProductStep : BaseEntity
    {
        public ProductStep()
        {
            Active = true;
            Default = true;
        }
        /// <summary>
        /// Tên 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Thứ tự
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Tham chiếu Product.type Service
        /// </summary>
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Mặc định 
        /// </summary>
        public bool Default { get; set; }

    }
}
