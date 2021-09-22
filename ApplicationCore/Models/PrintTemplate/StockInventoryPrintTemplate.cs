using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models.PrintTemplate
{
    public class StockInventoryPrintTemplate
    {
        public string Name { get; set; }

        /// <summary>
        /// Ngày kiểm kho
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Ghi Chú
        /// </summary>
        public string Note { get; set; }

        public string CreatedById { get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// Dòng điều chỉnh
        /// </summary>
        public IEnumerable<StockInventoryLinePrintTemplate> Lines { get; set; } = new List<StockInventoryLinePrintTemplate>();



        public Guid CompanyId { get; set; }
        public CompanyPrintTemplate Company { get; set; }


        public string CreatedByName { get; set; }
    }

    public class StockInventoryLinePrintTemplate
    {
        public UoMBasicPrintTemplate ProductUOM { get; set; }

        public ProductSimplePrintTemplate Product { get; set; }

        /// <summary>
        /// Số lượng chính thực tế
        /// </summary>
        public decimal? ProductQty { get; set; }

        /// <summary>
        /// Số lượng chính lý thuyết
        /// </summary>
        public decimal? TheoreticalQty { get; set; }

        public int? Sequence { get; set; }
    }
}
