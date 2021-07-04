using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class OverviewReportService : IOverviewReportService
    {
        private readonly IMapper _mapper;
        private readonly ICustomerReceiptService _customerReceiptService;
        private readonly IAppointmentService _appointmentService;
        public OverviewReportService(IMapper mapper, ICustomerReceiptService customerReceiptService, IAppointmentService appointmentService)
        {
            _mapper = mapper;
            _customerReceiptService = customerReceiptService;
            _appointmentService = appointmentService;
        }

        public async Task<CustomerReceiptDisplay> GetDefaultCustomerReceipt(GetDefaultRequest val)
        {
            var appointment = await _appointmentService.SearchQuery(x => x.Id == val.AppointmentId)
                .Include(x => x.AppointmentServices).ThenInclude(x => x.Product)
                .Include(x => x.Partner)
                .Include(x => x.Doctor)
                .FirstOrDefaultAsync();
            if (appointment == null)
                throw new Exception("hông tìm thấy lịch hẹn");

            if (appointment.State == "cancel")
                throw new Exception("hông thể tiếp nhận lịch hẹn ở trạng thái hủy hẹn");

            var res = new CustomerReceiptDisplay
            {
                CompanyId = appointment.CompanyId,
                PartnerId = appointment.PartnerId,
                Partner = appointment.Partner != null ? new PartnerSimpleContact
                {
                    Id = appointment.Partner.Id,
                    Name = appointment.Partner.Name,
                    Phone = appointment.Partner.Phone
                } : null,
                DoctorId = appointment.DoctorId,
                Doctor = appointment.Doctor != null ? new EmployeeSimple
                {
                    Id = appointment.Doctor.Id,
                    Name = appointment.Doctor.Name,
                } : null,
                Products = appointment.AppointmentServices.Any() ? appointment.AppointmentServices.Select(x => x.Product).Select(x => new ProductSimple { Id = x.Id, Name = x.Name }).ToList() : null,
                IsRepeatCustomer = appointment.IsRepeatCustomer,
                TimeExpected = appointment.TimeExpected,
                Note = appointment.Note
            };

            return res;
        }


        public async Task CreateCustomerReceiptToAppointment(CustomerReceiptRequest val)
        {
            var customerReceipt = new CustomerReceipt();
            customerReceipt.CompanyId = val.CompanyId;
            customerReceipt.PartnerId = val.PartnerId;
            customerReceipt.DoctorId = val.DoctorId;
            customerReceipt.DateWaitting = val.DateWaitting.HasValue ? val.DateWaitting.Value : DateTime.Now;
            customerReceipt.IsNoTreatment = val.IsRepeatCustomer;
            customerReceipt.Note = val.Note;
            if (val.Products.Any())
            {
                foreach (var product in val.Products)
                {
                    customerReceipt.CustomerReceiptProductRels.Add(new CustomerReceiptProductRel()
                    {
                        ProductId = product.Id
                    });
                }
            }

            await _customerReceiptService.CreateAsync(customerReceipt);

            if (val.AppointmentId.HasValue)
            {
                var appointment = await _appointmentService.SearchQuery(x => x.Id == val.AppointmentId)
                .FirstOrDefaultAsync();

                if (appointment.State == "confirmed")
                    appointment.State = "arrived";

                await _appointmentService.UpdateAsync(appointment);
            }

        }

        public async Task<GetCountMedicalXamination> GetCountMedicalXaminationToday()
        {
            var today = DateTime.Today;
            var appointmentNewToday = await _appointmentService.SearchQuery(x => x.Date >= today.AbsoluteBeginOfDate() && x.Date <= today.AbsoluteEndOfDate() && !x.IsRepeatCustomer).CountAsync();
            var appointmentOldToday = await _appointmentService.SearchQuery(x => x.Date >= today.AbsoluteBeginOfDate() && x.Date <= today.AbsoluteEndOfDate() && x.IsRepeatCustomer).CountAsync();

            var customerReceiptNewToday = await _customerReceiptService.SearchQuery(x => x.DateWaitting >= today.AbsoluteBeginOfDate() && x.DateWaitting <= today.AbsoluteEndOfDate() && !x.IsRepeatCustomer).CountAsync();
            var customerReceiptOldToday = await _customerReceiptService.SearchQuery(x => x.DateWaitting >= today.AbsoluteBeginOfDate() && x.DateWaitting <= today.AbsoluteEndOfDate() && x.IsRepeatCustomer).CountAsync();

            var res = new GetCountMedicalXamination
            {
                NewMedical = appointmentNewToday + customerReceiptNewToday,
                ReMedical = appointmentOldToday + customerReceiptOldToday
            };

            return res;
        }





    }



    public class GetDefaultRequest
    {
        public Guid? AppointmentId { get; set; }
    }

    public class GetCountMedicalXamination
    {
        public int NewMedical { get; set; }
        public int ReMedical { get; set; }
    }

    public class CustomerReceiptRequest
    {
        /// <summary>
        /// thời gian chờ khám
        /// </summary>
        public DateTime? DateWaitting { get; set; }

        /// <summary>
        /// Thời gian dự kiến
        /// </summary>
        public int TimeExpected { get; set; }

        /// <summary>
        /// Danh sách dịch vụ
        /// </summary>
        public IEnumerable<ProductSimple> Products { get; set; } = new List<ProductSimple>();

        /// <summary>
        /// Ghi chú, nội dung
        /// </summary>
        public string Note { get; set; }

        //Hẹn khách hàng nào?
        public Guid PartnerId { get; set; }

        public Guid CompanyId { get; set; }

        public Guid? DoctorId { get; set; }

        /// <summary>
        /// Khách hàng tái khám
        /// </summary>
        public bool IsRepeatCustomer { get; set; }

        public Guid? AppointmentId { get; set; }
    }
}
