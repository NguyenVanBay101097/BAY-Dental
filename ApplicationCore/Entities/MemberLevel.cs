using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class MemberLevel:BaseEntity
    {
        public MemberLevel()
        {
            
        }
        public string Name { get; set; }
        public decimal Point { get; set; }
        public string Color { get; set; }
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
