using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SamplePrescriptionLine : BaseEntity
    {
        /// <summary>
        /// Toa thuốc mẫu
        /// </summary>
        public Guid PrescriptionId { get; set; }
        public SamplePrescription Prescription { get; set; }

        /// <summary>
        /// Thuốc
        /// </summary>
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public Guid? ProductUoMId { get; set; }
        public UoM ProductUoM { get; set; }

        /// <summary>
        /// STT
        /// </summary>
        public int? Sequence { get; set; }

        /// <summary>
        /// Số lần uống 1 ngày
        /// </summary>
        public int NumberOfTimes { get; set; }

        /// <summary>
        /// Số ngày
        /// </summary>
        public int NumberOfDays { get; set; }

        /// <summary>
        /// Số lượng uống 1 lần
        /// </summary>
        public decimal AmountOfTimes { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Dùng lúc
        /// Sau khi ăn: after_meal
        /// Trước khi ăn: before_meal
        /// Trong khi ăn: in_meal
        /// Sau khi dậy: after_wakeup
        /// Trước khi đi ngủ: before_sleep
        /// Khác: other 
        /// </summary>
        public string UseAt { get; set; }

        public string Note { get; set; }
    }
}
