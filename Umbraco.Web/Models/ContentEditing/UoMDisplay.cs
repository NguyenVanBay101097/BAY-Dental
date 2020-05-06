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

        public decimal Factor { get; set; }

        public string UOMType { get; set; }

        public Guid CategoryId { get; set; }

        public UoMCategory Category { get; set; }

        public string MeasureType { get; set; }
    }
}
