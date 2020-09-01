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

        /// <summary>
        /// Nhân viên
        /// </summary>
        public Guid? DoctorId { get; set; }
        public EmployeeBasic Doctor { get; set; }

        public string Note { get; set; }

        public Guid? AssistantId { get; set; }
        public EmployeeBasic Assistant { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime Date { get; set; }

        public Guid? SaleOrderId { get; set; }
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
        public Guid? AppointmentId { get; set; }
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

        public Guid? DoctorId { get; set; }
        public EmployeeBasic Doctor { get; set; }

        public Guid? AssistantId { get; set; }
        public EmployeeBasic Assistant { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime Date { get; set; }

        public Guid? AppointmentId { get; set; }
        public AppointmentBasic Appointment { get; set; }

        public Guid? SaleOrderId { get; set; }

        public string State { get; set; }

        public Guid CompanyId { get; set; }
    }

    public class DotKhamSave
    {
        public DotKhamSave()
        {
            Date = DateTime.Now;
        }

        public Guid? AssistantId { get; set; }

        public Guid? DoctorId { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime Date { get; set; }

        public Guid? SaleOrderId { get; set; }

        public Guid CompanyId { get; set; }
    }

    public class DotKhamDefaultGet
    {
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
