using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class DotKhamLineOperation: BaseEntity
    {
        public DotKhamLineOperation()
        {
            State = "draft";
        }

        public Guid? LineId { get; set; }
        public DotKhamLine Line { get; set; }

        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        public int? Sequence { get; set; }

        /// <summary>
        /// Trạng thái: chưa tiến hành, đang tiến hành, hoàn thành
        /// draft: chưa tiến hành
        /// progress: đang tiến hành
        /// done: hoàn thành
        /// </summary>
        public string State { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateFinished { get; set; }
    }
}
