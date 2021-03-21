using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class StockMove: BaseEntity
    {
        public StockMove()
        {
            Date = DateTime.Now;
            State = "draft";
        }

        public StockMove(StockMove move)
        {
            Origin = move.Origin;
            State = "draft";
            Note = move.Note;
            Name = move.Name;
            DateExpected = move.DateExpected;
            PickingId = move.PickingId;
            PartnerId = move.PartnerId;
            ProductUOMId = move.ProductUOMId;
            ProductUOMQty = move.ProductUOMQty;
            ProductQty = move.ProductQty;
            ProductId = move.ProductId;
            LocationId = move.LocationId;
            LocationDestId = move.LocationDestId;
            PickingTypeId = move.PickingTypeId;
            PurchaseLineId = move.PurchaseLineId;
            PurchaseLine = move.PurchaseLine;
            Date = move.Date;
            PriceUnit = move.PriceUnit;
            WarehouseId = move.WarehouseId;
            CompanyId = move.CompanyId;
        }

        /// <summary>
        /// mô tả cho dịch chuyển
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// trạng thái
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// tên của dịch chuyển kho
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ngày dự kiến
        /// </summary>
        public DateTime? DateExpected { get; set; }
        /// <summary>
        /// Loại hoạt động (bảng hoạt động)
        /// </summary>
        public Guid? PickingTypeId { get; set; }

        public StockPickingType PickingType { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        public decimal ProductUOMQty { get; set; }

        public decimal? ProductQty { get; set; }

        public Guid? ProductUOMId { get; set; }
        public UoM ProductUOM { get; set; }

        /// <summary>
        /// sản phẩm di chuyển
        /// </summary>
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        /// <summary>
        /// địa điểm di chuyển
        /// </summary>
        public Guid LocationId { get; set; }
        public StockLocation Location { get; set; }

        /// <summary>
        /// địa điểm đến
        /// </summary>
        public Guid LocationDestId { get; set; }
        public StockLocation LocationDest { get; set; }

        /// <summary>
        /// kho hàng
        /// </summary>
        public Guid? WarehouseId { get; set; }
        public StockWarehouse Warehouse { get; set; }

        public Guid? PickingId { get; set; }

        public StockPicking Picking { get; set; }

        /// <summary>
        /// Date Creation
        /// </summary>
        public DateTime Date { get; set; }

        public double? PriceUnit { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public int Sequence { get; set; }

        public string Origin { get; set; }

        public Guid? PurchaseLineId { get; set; }
        public PurchaseOrderLine PurchaseLine { get; set; }

        public Guid? InventoryId { get; set; }
        public StockInventory Inventory { get; set; }

        public bool IsIn()
        {
            if (!Location.ShouldBeValued() && LocationDest.ShouldBeValued())
                return true;
            return false;
        }
    }
}
