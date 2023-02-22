using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class UoMDisplay
    {
        public string Name { get; set; }

        public decimal Rounding { get; set; }

        public bool Active { get; set; }

        public double Factor { get; set; }

        public double? FactorInv { get; set; }

        public string UOMType { get; set; }

        public Guid CategoryId { get; set; }

        public UoMCategorySimple Category { get; set; }

        public string MeasureType { get; set; }
    }
}
