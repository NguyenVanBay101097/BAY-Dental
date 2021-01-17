using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// hoa don thuoc
    /// </summary>
    public class MedicineOrder : BaseEntity
    {
        public MedicineOrder()
        {
            State = "draft";
            Amount = 0;
        }

        /// <summary>
        /// HDTT{dd}{MM}{yy}-{sequence} refix 5
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// phương thức thanh toán
        /// </summary>
        public Guid JournalId { get; set; }
        public AccountJournal Journal { get; set; }

        /// <summary>
        /// khách hàng
        /// </summary>
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }

        public Guid ToathuocId { get; set; }
        public ToaThuoc ToaThuoc { get; set; }

        public  DateTime OrderDate { get; set; }

        /// <summary>
        /// bác sĩ
        /// </summary>
        public Guid? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// tổng tiền
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// darft , confirmed , cancel
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// nội dung
        /// </summary>
        public string Note { get; set; }

        public Guid? MoveId { get; set; }
        public AccountMove Move { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid? AccountPaymentId { get; set; }
        public AccountPayment AccountPayment { get; set; }


        /// <summary>
        /// phiếu xuất kho
        /// </summary>
        public Guid? StockPickingOutgoingId { get; set; }
        public StockPicking StockPickingOutgoing { get; set; }

        /// <summary>
        /// phiếu nhập kho
        /// </summary>
        public Guid? StockPickingIncomingId { get; set; }
        public StockPicking StockPickingIncoming { get; set; }

        ///line

        public ICollection<MedicineOrderLine> MedicineOrderLines { get; set; } = new List<MedicineOrderLine>();
    }
}
