using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class CustomerReceiptProductRel
    {
        public Guid CustomerReceiptId { get; set; }
        public CustomerReceipt CustomerReceipt { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }      
    }
}
