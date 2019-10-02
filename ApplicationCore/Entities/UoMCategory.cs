using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Nhóm đơn vị tính
    /// </summary>
    public class UoMCategory: BaseEntity
    {
        public string Name { get; set; }

        public string MeasureType { get; set; }
    }
}
