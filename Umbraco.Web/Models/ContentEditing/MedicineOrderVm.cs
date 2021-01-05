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

        public SaleOrderBasic SaleOrder { get; set; }

        public decimal Amount { get; set; }
        public string State { get; set; }
    }

    public class MedicineOrderSave
    {

        /// <summary>
        /// Ngày thanh toán
        /// </summary>
        public DateTime DateOrder { get; set; }

        public Guid JoirnalId { get; set; }
        public Guid ToaThuocId { get; set; }

        public string Note { get; set; }

        public decimal Amount { get; set; }
        public string State { get; set; }

        public IEnumerable<MedicineOrderLineSave> MedicineOrderLines { get; set; } = new List<MedicineOrderLineSave>();
    }

    public class MedicineOrderDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Ngày thanh toán
        /// </summary>
        public DateTime DateOrder { get; set; }

        public Guid EmployeeId { get; set; }
        public EmployeeSimple Employee { get; set; }

        public Guid ToaThuocId { get; set; }
        public ToaThuocDisplay ToaThuoc { get; set; }

        public Guid JournalId { get; set; }
        public AccountJournalSimple Journal { get; set; }

        public Guid PartnerId { get; set; }
        public PartnerDisplay Partner { get; set; }

        public decimal Amount { get; set; }
        public string State { get; set; }

        public IEnumerable<MedicineOrderLineDisplay> MedicineOrderLines { get; set; } = new List<MedicineOrderLineDisplay>();
    }
}

