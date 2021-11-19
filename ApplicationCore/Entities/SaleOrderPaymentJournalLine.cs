﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleOrderPaymentJournalLine : BaseEntity
    {
        public Guid JournalId { get; set; }
        public AccountJournal Journal { get; set; }

        public Guid SaleOrderPaymentId { get; set; }
        public SaleOrderPayment SaleOrderPayment { get; set; }

        /// <summary>
        /// insurance : bảo hiểm
        /// </summary>
        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        public decimal Amount { get; set; }
    }
}
