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
    }
}
