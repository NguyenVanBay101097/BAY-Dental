using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class DotKhamSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class DotKhamBasic
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Số đợt khám
        /// </summary>
        public string Name { get; set; }

        public Guid? PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        public Guid? InvoiceId { get; set; }
        public AccountInvoiceBasic Invoice { get; set; }

        /// <summary>
        /// Nhân viên
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUserSimple User { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime Date { get; set; }
    }

    public class DotKhamPaged
    {
        public DotKhamPaged()
        {
            Limit = 20;
        }

        public int Limit { get; set; }

        public int Offset { get; set; }

        public string Search { get; set; }
    }

    public class DotKhamDisplay
    {
        public DotKhamDisplay()
        {
            Date = DateTime.Now;
            State = "draft";
        }

        public Guid Id { get; set; }
        /// <summary>
        /// Số đợt khám
        /// </summary>
        public string Name { get; set; }

        public Guid? PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        /// <summary>
        /// Nhân viên
        /// </summary>
        public Guid? DoctorId { get; set; }
        public EmployeeSimple Doctor { get; set; }

        public Guid? AssistantId { get; set; }
        public EmployeeSimple Assistant { get; set; }

        /// <summary>
        /// Người dùng
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUserSimple User { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Hóa đơn điều trị
        /// </summary>
        public Guid? InvoiceId { get; set; }
        public AccountInvoiceCbx Invoice { get; set; }

        public Guid? SaleOrderId { get; set; }
        public SaleOrderBasic SaleOrder { get; set; }

        public Guid? AppointmentId { get; set; }
        public AppointmentBasic Appointment { get; set; }

        public string State { get; set; }

        public Guid CompanyId { get; set; }

        public IEnumerable<DotKhamLineDisplay> Lines { get; set; } = new List<DotKhamLineDisplay>();
    }

    public class DotKhamDefaultGet
    {
        public Guid? InvoiceId { get; set; }
        public Guid? SaleOrderId { get; set; }
    }

    public class ProductWithStepList
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public List<DotKhamLineBasic> DKLList { get; set; }
    }

    public class DotKhamPatch
    {
        public Guid DotKhamId { get; set; }
        public Guid? AppointmentId { get; set; }
    }
}
