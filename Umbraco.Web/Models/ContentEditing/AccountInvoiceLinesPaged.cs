using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountInvoiceLinesPaged
    {
        public AccountInvoiceLinesPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }
        public int Limit { get; set; }

        public Guid? InvoiceId { get; set; }
        public AccountInvoiceBasic Invoice { get; set; }

        public Guid? ProductId { get; set; }
        public ProductBasic Product { get; set; }

        public Guid AccountId { get; set; }
        public AccountAccountSimple Account { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid? PartnerId { get; set; }
        public PartnerBasic Partner { get; set; }
        

        public Guid? ToothCategoryId { get; set; }
        public ToothCategoryBasic ToothCategory { get; set; }
        
        /// <summary>
        /// Chẩn đoán
        /// </summary>
        public string Diagnostic { get; set; }
    }
}
