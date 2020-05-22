using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class TCareProperty: BaseEntity
    {
        public Guid RuleId { get; set; }
        public TCareRule Rule { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Text, Integer, DateTime, Decimal, Double, Many2One
        /// </summary>
        public string Type { get; set; }

        public string ValueText { get; set; }

        public int? ValueInteger { get; set; }

        public DateTime? ValueDateTime { get; set; }

        public decimal? ValueDecimal { get; set; }

        public double? ValueDouble { get; set; }

        public string ValueReference { get; set; }
    }
}
