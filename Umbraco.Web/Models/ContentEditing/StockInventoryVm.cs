using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class StockInventoryBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string State { get; set; }
    }

    public class StockInventoryPaged
    {
        public StockInventoryPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string State { get; set; }

        public string Search { get; set; }
    }

    public class StockInventorySave
    {

        /// <summary>
        /// Ngày kiểm kho
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Ghi Chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Dòng điều chỉnh
        /// </summary>
        public IEnumerable<StockInventoryLineSave> Lines { get; set; } = new List<StockInventoryLineSave>();

        /// <summary>
        /// Dịch chuyển kho được tạo ra
        /// </summary>
        //public IEnumerable<StockMoveSave> Moves { get; set; }

        /// <summary>
        /// draft : nháp
        /// confirmed : đang xử lý
        /// done : hoàn thành
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Địa điểm kiểm kho
        /// </summary>
        public Guid LocationId { get; set; }

        public Guid? ProductId { get; set; }

        public Guid? CategoryId { get; set; }

        public Guid? CriteriaId { get; set; }

        /// <summary>
        /// none : tất cả sản phẩm
        /// category : nhóm sản phẩm
        /// partial : chọn sản phẩm thủ công
        /// </summary>
        public string Filter { get; set; }

        public Guid CompanyId { get; set; }

        /// <summary>
        /// Bao gồm những sản phẩm mà tồn <= 0
        /// </summary>
        public bool? Exhausted { get; set; }
    }

    public class StockInventoryDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Ngày kiểm kho
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Ghi Chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Dòng điều chỉnh
        /// </summary>
        public IEnumerable<StockInventoryLineDisplay> Lines { get; set; } = new List<StockInventoryLineDisplay>();

        /// <summary>
        /// Dịch chuyển kho được tạo ra
        /// </summary>
        public IEnumerable<StockMoveDisplay> Moves { get; set; } = new List<StockMoveDisplay>();

        /// <summary>
        /// draft : nháp
        /// confirmed : đang xử lý
        /// done : hoàn thành
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Địa điểm kiểm kho
        /// </summary>
        public Guid LocationId { get; set; }
        public StockLocationSimple Location { get; set; }

        public Guid? ProductId { get; set; }
        public ProductSimple Product { get; set; }

        public Guid? CategoryId { get; set; }
        public ProductCategoryBasic Category { get; set; }

        public Guid? CriteriaId { get; set; }
        public StockInventoryCriteriaBasic Criteria { get; set; }

        /// <summary>
        /// none : tất cả sản phẩm
        /// category : nhóm sản phẩm
        /// partial : chọn sản phẩm thủ công
        /// </summary>
        public string Filter { get; set; }

        public Guid CompanyId { get; set; }

        /// <summary>
        /// Bao gồm những sản phẩm mà tồn <= 0
        /// </summary>
        public bool? Exhausted { get; set; }
    }

    public class StockInventoryDefaultGet
    {
        public Guid? CompanyId { get; set; }
    }

    public class ProductStockInventory
    {
        public Guid Id { get; set; }
        public Guid LocationId { get; set; }

        public Guid ProductId { get; set; }

        public ProductDisplay Product { get; set; }

        public Guid ProductUOMId { get; set; }
        public UoMBasic ProductUOM { get; set; }
        public decimal? ProductQty { get; set; }
        public decimal? TheoreticalQty { get; set; }
    }


}
