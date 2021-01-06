using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class MedicineOrderPaged
    {
        public MedicineOrderPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }

        public string State { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
    public class DefaultGet
    {     
        public Guid ToaThuocId { get; set; }
    }

    public class MedicineOrderBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Ngày thanh toán
        /// </summary>
        public DateTime DateOrder { get; set; }

        public string EmployeeName { get; set; }
   
        public string PartnerName { get; set; }

        public Guid ToaThuocId { get; set; }
        public string ToaThuocName { get; set; }

        public decimal Amount { get; set; }
        public string State { get; set; }
    }

    public class MedicineOrderSave
    {

        /// <summary>
        /// Ngày thanh toán
        /// </summary>
        public DateTime DateOrder { get; set; }

        public Guid journalId { get; set; }
        public Guid ToaThuocId { get; set; }

        public Guid EmployeeId { get; set; }
        public Guid PartnerId { get; set; }

        public Guid CompanyId { get; set; }



        public string Note { get; set; }

        public decimal Amount { get; set; }
        public string State { get; set; }

        public IEnumerable<MedicineOrderLineSave> MedicineOrderLines { get; set; } = new List<MedicineOrderLineSave>();
    }

    public class MedicineOrderDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid CompanyId { get; set; }

        /// <summary>
        /// Ngày thanh toán
        /// </summary>
        public DateTime DateOrder { get; set; }

        public Guid? EmployeeId { get; set; }
        public EmployeeSimple Employee { get; set; }

        public Guid ToaThuocId { get; set; }
        public ToaThuocDisplay ToaThuoc { get; set; }

        public Guid JournalId { get; set; }
        public AccountJournalSimple Journal { get; set; }

        public Guid PartnerId { get; set; }
        public PartnerBasic Partner { get; set; }

        public Guid? AccountPaymentId { get; set; }
        public AccountPaymentDisplay AccountPayment { get; set; }

        public decimal Amount { get; set; }
        public string State { get; set; }

        public IEnumerable<MedicineOrderLineDisplay> MedicineOrderLines { get; set; } = new List<MedicineOrderLineDisplay>();
    }

    public class MedicineOrderPrint
    {
        public Company Company { get; set; }
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid CompanyId { get; set; }

        /// <summary>
        /// Ngày thanh toán
        /// </summary>
        public DateTime DateOrder { get; set; }

        public Guid? EmployeeId { get; set; }
        public EmployeeSimple Employee { get; set; }

        public Guid ToaThuocId { get; set; }
        public ToaThuocDisplay ToaThuoc { get; set; }

        public Guid JournalId { get; set; }
        public AccountJournalSimple Journal { get; set; }

        public Guid PartnerId { get; set; }
        public PartnerDisplay Partner { get; set; }

        public Guid? AccountPaymentId { get; set; }
        public AccountPaymentDisplay AccountPayment { get; set; }

        public decimal Amount { get; set; }
        public string State { get; set; }

        public IEnumerable<MedicineOrderLineDisplay> MedicineOrderLines { get; set; } = new List<MedicineOrderLineDisplay>();
    }
}

