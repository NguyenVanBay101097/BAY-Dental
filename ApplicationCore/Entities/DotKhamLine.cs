using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    //Tuong tu mrp production
    public class DotKhamLine: BaseEntity
    {
        public DotKhamLine()
        {
            State = "draft";
        }

        public Guid DotKhamId { get; set; }
        public DotKham DotKham { get; set; }

        public int? Sequence { get; set; }

        public string NameStep { get; set; }

        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        /// <summary>
        /// dinh muc
        /// </summary>
        //public Guid? RoutingId { get; set; }
        //public Routing Routing { get; set; }

        /// <summary>
        /// Nguoi chiu trach nhiem
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }


        public Guid? SaleOrderLineId { get; set; }
        public SaleOrderLine SaleOrderLine { get; set; }

        /// <summary>
        /// Trạng thái: chưa tiến hành, đang tiến hành, hoàn thành
        /// draft: chưa tiến hành
        /// progress: đang tiến hành
        /// done: hoàn thành
        /// </summary>
        public string State { get; set; }

        //public ICollection<DotKhamLineOperation> Operations { get; set; } = new List<DotKhamLineOperation>();

        //public DateTime? DateStart { get; set; }

        //public DateTime? DateFinished { get; set; }


        public string Note { get; set; }

        public DateTime? Date { get; set; }

        public Guid? ToothCategoryId { get; set; }
        public ToothCategory ToothCategory { get; set; }

        /// <summary>
        /// răng
        /// </summary>
        public ICollection<SaleOrderLineToothRel> SaleOrderLineToothRels { get; set; } = new List<SaleOrderLineToothRel>();

    }
}
