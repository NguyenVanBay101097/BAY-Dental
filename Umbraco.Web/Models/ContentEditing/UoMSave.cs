using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class UoMSave
    {
        public UoMSave()
        {
            Active = true;
            Rounding = 0.01M;
            Factor = 1;
            UOMType = "reference";
        }
        public string Name { get; set; }

        //làm tròn
        public decimal Rounding { get; set; }

        public bool Active { get; set; }

        //ti lệ
        public decimal Factor { get; set; }

        //loại
        public string UOMType { get; set; }

        public Guid CategoryId { get; set; }

        public string MeasureType { get; set; }
    }
}
