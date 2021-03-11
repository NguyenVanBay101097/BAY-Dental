using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountPaymentBasic
    {
        public Guid Id { get; set; }

        public string PartnerName { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PartnerType { get; set; }

        public string PaymentType { get; set; }

        public string JournalName { get; set; }

        public string State { get; set; }

        public string Name { get; set; }

        public decimal Amount { get; set; }

        public string Communication { get; set; }

        public string DisplayPaymentType
        {
            get
            {
                switch (PaymentType)
                {
                    case "inbound":
                        return "Phiếu thu";
                    case "outbound":
                        return "Phiếu chi";
                    default:
                        return "";
                }
            }
        }

        public decimal AmountSigned
        {
            get
            {
                var sign = PaymentType == "outbound" ? -1 : 1;
                return Amount * sign;
            }
        }

        public string DisplayState
        {
            get
            {
                switch (State)
                {
                    case "posted":
                        return "Đã xác nhận";
                    case "cancel":
                        return "Đã hủy";
                    default:
                        return "Nháp";
                }
            }
        }
    }
}
