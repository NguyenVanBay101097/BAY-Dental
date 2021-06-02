using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsConfigProductCategoryRel
    {
        public Guid SmsConfigId { get; set; }
        public SmsConfig SmsConfig { get; set; }
        public Guid ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }
    }
}
