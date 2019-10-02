using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Đơn vị tính
    /// </summary>
    public class UoM: BaseEntity
    {
        public UoM()
        {
            Active = true;
            Rounding = 0.01M;
            Factor = 1;
            UOMType = "reference";
        }

        public string Name { get; set; }

        public decimal Rounding { get; set; }

        public bool Active { get; set; }

        public decimal Factor { get; set; }

        public string UOMType { get; set; }

        public Guid CategoryId { get; set; }

        public UoMCategory Category { get; set; }

        public string MeasureType { get; set; }
    }
}
