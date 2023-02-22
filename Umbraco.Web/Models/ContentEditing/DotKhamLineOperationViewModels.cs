using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class DotKhamLineOperationDisplay
    {
        public Guid Id { get; set; }

        public Guid? ProductId { get; set; }
        public ProductSimple Product { get; set; }

        /// <summary>
        /// Trạng thái: chưa tiến hành, đang tiến hành, hoàn thành
        /// draft: chưa tiến hành
        /// progress: đang tiến hành
        /// done: hoàn thành
        /// </summary>
        public string State { get; set; }

        public int? Sequence { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateFinished { get; set; }
    }
}
