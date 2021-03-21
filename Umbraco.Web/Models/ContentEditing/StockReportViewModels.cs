using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class StockReportXuatNhapTonSearch
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? ProductCategId { get; set; }

        public Guid? ProductId { get; set; }

        /// <summary>
        /// Search ma hoac ten san pham
        /// </summary>
        public string Search { get; set; }
    }

    public class StockReportXuatNhapTonItem
    {
        public StockReportXuatNhapTonItem()
        {
            Begin = 0;
            Import = 0;
            Export = 0;
            End = 0;
        }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public decimal Begin { get; set; }

        public decimal Import { get; set; }

        public decimal Export { get; set; }

        public decimal End { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string ProductUomName { get; set; }

    }

    public class StockReportXuatNhapTonItemDetail
    {
        public DateTime? Date { get; set; }

        public decimal Begin { get; set; }

        public decimal Import { get; set; }

        public decimal Export { get; set; }

        public decimal End { get; set; }

        public string MovePickingName { get; set; }

        public Guid? MovePickingId { get; set; }

        public Guid? MovePickingTypeId { get; set; }

        public double? PriceUnitOnQuant { get; set; }
    }
}

