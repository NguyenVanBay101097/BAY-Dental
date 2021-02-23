using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Dùng để điều chỉnh tồn kho -> số lượng thực tến của sản phẩm
    /// </summary>
    public class StockInventory : BaseEntity
    {
        public StockInventory()
        {
            Date = DateTime.Now;
            State = "draft";
            Filter = "none";
            Lines = new List<StockInventoryLine>();
            Moves = new List<StockMove>();
        }

        /// <summary>
        /// Mã phiếu tự động
        /// </summary>
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
        public ICollection<StockInventoryLine> Lines { get; set; }

        /// <summary>
        /// Dịch chuyển kho được tạo ra
        /// </summary>
        public ICollection<StockMove> Moves { get; set; }

        public string State { get; set; }

        /// <summary>
        /// Địa điểm kiểm kho
        /// </summary>
        public Guid LocationId { get; set; }
        public StockLocation Location { get; set; }

        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        public Guid? CategoryId { get; set; }
        public ProductCategory Category { get; set; }

        /// <summary>
        /// If you do an entire inventory, you can choose 'All Products' and it will prefill the inventory with the current stock.  If you only do some products
        /// (e.g. Cycle Counting) you can choose 'Manual Selection of Products' and the system won't propose anything.  You can also let the
        /// system propose for a single product / lot /... 
        /// </summary>
        public string Filter { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// Bao gồm những sản phẩm mà tồn <= 0
        /// </summary>
        public bool? Exhausted { get; set; }
    }
}
