using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class WorkEntryType : BaseEntity
    {
        public WorkEntryType()
        {
            Active = true;
            RoundDays = "NO";
        }

        /// <summary>
        /// Ten 
        /// </summary>
        public string Name { get; set; }

        public string Code { get; set; }

        public int? Sequence { get; set; }

        public bool Active { get; set; }


        /// <summary>
        /// là có chấm công hay không, ví dụ: type workfromhome là có công
        /// </summary>
        public bool IsHasTimeKeeping { get; set; }

        /// <summary>
        /// màu của type
        /// </summary>
        public string Color { get; set; }

        public ICollection<ChamCong> ChamCongs { get; set; }

        /// <summary>
        /// Làm tròn
        /// NO: Không làm tròn
        /// HALF: Nữa ngày
        /// FULL: Ngày
        /// </summary>
        public string RoundDays { get; set; }

        /// <summary>
        /// Loại làm tròn
        /// HALF-UP: gần nhất
        /// UP: lên
        /// DOWN: xuống
        /// </summary>
        public string RoundDaysType { get; set; }
    }
}
