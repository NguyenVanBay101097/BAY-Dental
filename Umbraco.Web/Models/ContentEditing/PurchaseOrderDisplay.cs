﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PurchaseOrderDisplay
    {
        public PurchaseOrderDisplay()
        {
            DateOrder = DateTime.Now;
            State = "draft";
            Type = "order";
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string State { get; set; }

        public Guid PickingTypeId { get; set; }
        public StockPickingTypeBasic PickingType { get; set; }

        public Guid? JournalId { get; set; }
        public AccountJournalSimple Journal { get; set; }

        public string UserId { get; set; }
        public ApplicationUserSimple User { get; set; }

        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public Guid PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        public DateTime DateOrder { get; set; }

        public decimal AmountTotal { get; set; }

        public decimal? AmountPayment { get; set; }

        public IEnumerable<PurchaseOrderLineDisplay> OrderLines { get; set; } = new List<PurchaseOrderLineDisplay>();

        public decimal AmountResidual { get; set; }

        public string Note { get; set; }
    }
}
