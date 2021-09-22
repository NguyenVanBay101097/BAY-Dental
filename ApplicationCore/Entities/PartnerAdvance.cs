using ApplicationCore.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Entities
{
    public class PartnerAdvance : BaseEntity
    {
        public PartnerAdvance()
        {
            Date = DateTime.Now;
            State = "draft";       
        }

        /// <summary>
        /// mã tự sinh : TUKH/{yyyy}/STT
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ngày tạm ứng
        /// </summary>
        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// khách hàng
        /// </summary>
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }

        /// <summary>
        /// phương thức
        /// </summary>
        public Guid? JournalId { get; set; }
        public AccountJournal Journal { get; set; }

        /// <summary>
        /// chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// advance : đóng tạm ứng
        /// refund : hoàn tiền tạm ứng
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// draft : nháp
        /// confirmed : đã xác nhận
        /// </summary>
        public string State { get; set; }

        public string Note { get; set; }
        /// <summary>
        /// Số tiền bằng chữ
        /// </summary>
        [NotMapped]
        public string AmountText
        {
            get
            {
                return AmountToText.amount_to_text(Amount);
            }
            set { }
        }
    }
}
