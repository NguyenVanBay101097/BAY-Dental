using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AdvisoryProductRel
    {
        public Guid AdvisoryId { get; set; }
        public Advisory Advisory { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
