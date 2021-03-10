using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class StockPickingBasic
    {
        public Guid Id { get; set; }

        public Guid? PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        public string State { get; set; }

        /// <summary>
        /// Creation Date
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Reference
        /// </summary>
        public string Name { get; set; }
    }

    public class StockPickingPaged
    {
        public StockPickingPaged()
        {
            Limit = 20;
        }

        public int Limit { get; set; }

        public int Offset { get; set; }

        public Guid? PickingTypeId { get; set; }

        public string Search { get; set; }

        public string Type { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }

    public class StockPickingDisplay
    {
        public StockPickingDisplay()
        {
            State = "draft";
            Date = DateTime.Now;
            Name = "/";
        }

        public Guid Id { get; set; }

        public Guid? PartnerId { get; set; }

        public PartnerSimple Partner { get; set; }

        public Guid PickingTypeId { get; set; }

        public string Note { get; set; }

        public string State { get; set; }

        /// <summary>
        /// Creation Date
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Reference
        /// </summary>
        public string Name { get; set; }

        public IEnumerable<StockMoveDisplay> MoveLines { get; set; } = new List<StockMoveDisplay>();

        public Guid CompanyId { get; set; }

        /// <summary>
        /// Source Location Zone
        /// </summary>
        public Guid LocationId { get; set; }

        /// <summary>
        /// Destination Location Zone
        /// </summary>
        public Guid LocationDestId { get; set; }
    }

    public class StockPickingDefaultGet
    {
        public Guid? DefaultPickingTypeId { get; set; }
    }

    public class StockPickingOnChangePickingType
    {
        public Guid? PickingTypeId { get; set; }
    }

    public class StockPickingOnChangePickingTypeResult
    {
        public Guid? LocationId { get; set; }

        public Guid? LocationDestId { get; set; }
    }

    public class StockPickingSimple
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
