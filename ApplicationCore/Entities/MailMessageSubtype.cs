using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class MailMessageSubtype : BaseEntity
    {
        /// <summary>
        /// Name : Subtype get in irmodeldata 
        /// Phiếu điều trị : subtype_sale_order
        /// Dịch vụ : subtype_sale_order_line
        /// Thanh toán: subtype_sale_order_payment
        /// Lịch hẹn: subtype_appointment
        /// Tiếp nhận: subtype_receive
        /// Đợt khám: subtype_dotkham
        /// Ghi chú: subtype_comment
        /// </summary>
        public string Name { get; set; }
    }
}
