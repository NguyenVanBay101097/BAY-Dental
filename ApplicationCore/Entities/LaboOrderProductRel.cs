using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class LaboOrderProductRel
    {
        /// <summary>
        /// laboorder
        /// </summary>
        public Guid LaboOrderId { get; set; }
        public LaboOrder LaboOrder { get; set; }
        /// <summary>
        /// gửu kèm
        /// </summary>
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
