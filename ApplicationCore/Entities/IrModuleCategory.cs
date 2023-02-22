using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class IrModuleCategory: BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ParentId { get; set; }
        public IrModuleCategory Parent { get; set; }
        public int? Sequence { get; set; }
        public bool? Visible { get; set; }
        public bool? Exclusive { get; set; }
    }
}
