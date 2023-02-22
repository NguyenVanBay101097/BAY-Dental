using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class CardHistory: BaseEntity
    {
        public Guid CardId { get; set; }
        public CardCard Card { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? PointInPeriod { get; set; }
        public decimal? TotalPoint { get; set; }
        public Guid TypeId { get; set; }
        public CardType Type { get; set; }
    }
}
