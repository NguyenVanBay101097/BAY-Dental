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


        /// <summary>
        /// 
        /// </summary>
        public string MeasureType { get; set; }
    }
}
