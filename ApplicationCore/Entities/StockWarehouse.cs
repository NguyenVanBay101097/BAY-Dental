using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class StockWarehouse: BaseEntity
    {
        public StockWarehouse()
        {
            ReceptionSteps = "one_step";
            DeliverySteps = "ship_only";
            Active = true;
        }

        public string Name { get; set; }

        public bool Active { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        public Guid ViewLocationId { get; set; }
        public StockLocation ViewLocation { get; set; }

        //lot_stock_id
        public Guid LocationId { get; set; }
        public StockLocation Location { get; set; }

        /// <summary>
        /// Short name
        /// </summary>
        public string Code { get; set; }

        public Guid? InTypeId { get; set; }
        public StockPickingType InType { get; set; }
        public Guid? OutTypeId { get; set; }
        public StockPickingType OutType { get; set; }

        public string ReceptionSteps { get; set; }
        public string DeliverySteps { get; set; }
    }
}
