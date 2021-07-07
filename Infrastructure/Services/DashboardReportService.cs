using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class DashboardReportService : IDashboardReportService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DashboardReportService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CustomerReceiptDisplay> GetDefaultCustomerReceipt(GetDefaultRequest val)
        {
            var appointmentObj = GetService<IAppointmentService>();

            var appointment = await appointmentObj.SearchQuery(x => x.Id == val.AppointmentId)
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
            var customerReceiptObj = GetService<ICustomerReceiptService>();
            var appointmentObj = GetService<IAppointmentService>();

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

            await customerReceiptObj.CreateAsync(customerReceipt);

            if (val.AppointmentId.HasValue)
            {
                var appointment = await appointmentObj.SearchQuery(x => x.Id == val.AppointmentId)
                .FirstOrDefaultAsync();

                if (appointment.State == "confirmed")
                    appointment.State = "arrived";

                await appointmentObj.UpdateAsync(appointment);
            }

        }

        public async Task<GetCountMedicalXamination> GetCountMedicalXaminationToday()
        {
            var today = DateTime.Today;
            var customerReceiptObj = GetService<ICustomerReceiptService>();
            var appointmentObj = GetService<IAppointmentService>();


            var appointmentNewToday = await appointmentObj.SearchQuery(x => x.Date >= today.AbsoluteBeginOfDate() && x.Date <= today.AbsoluteEndOfDate() && !x.IsRepeatCustomer).CountAsync();
            var appointmentOldToday = await appointmentObj.SearchQuery(x => x.Date >= today.AbsoluteBeginOfDate() && x.Date <= today.AbsoluteEndOfDate() && x.IsRepeatCustomer).CountAsync();

            var customerReceiptNewToday = await customerReceiptObj.SearchQuery(x => x.DateWaitting >= today.AbsoluteBeginOfDate() && x.DateWaitting <= today.AbsoluteEndOfDate() && !x.IsRepeatCustomer).CountAsync();
            var customerReceiptOldToday = await customerReceiptObj.SearchQuery(x => x.DateWaitting >= today.AbsoluteBeginOfDate() && x.DateWaitting <= today.AbsoluteEndOfDate() && x.IsRepeatCustomer).CountAsync();

            var res = new GetCountMedicalXamination
            {
                NewMedical = appointmentNewToday + customerReceiptNewToday,
                ReMedical = appointmentOldToday + customerReceiptOldToday
            };

            return res;
        }

        public async Task<RevenueTodayReponse> GetSumary(RevenueTodayRequest val)
        {
            var movelineObj = GetService<IAccountMoveLineService>();
            var yesterday = DateTime.Today.AddDays(-1);
            var revenue = new RevenueTodayReponse();

            var dateFrom = val.DateFrom;
            if (dateFrom.HasValue)
                dateFrom = dateFrom.Value.AbsoluteBeginOfDate();

            var dateTo = val.DateTo;
            if (dateTo.HasValue)
                dateTo = dateTo.Value.AbsoluteEndOfDate();

            var query = movelineObj._QueryGet(companyId: val.CompanyId, state: "posted", dateFrom: dateFrom,
               dateTo: dateTo);

            var query2 = movelineObj._QueryGet(companyId: val.CompanyId, state: "posted", dateFrom: yesterday.AbsoluteBeginOfDate(),
             dateTo: yesterday.AbsoluteEndOfDate());

            query = query.Where(x => x.AccountInternalType == "receivable" && x.Reconciled == true && x.ExcludeFromInvoiceTab == false);

            var items = await query.Include(x => x.Journal).ToListAsync();

            var sign = -1;
            revenue.TotalBank = items.Where(x => x.Journal.Type == "bank").Sum(x => x.Debit - x.Credit * sign);
            revenue.TotalCash = items.Where(x => x.Journal.Type == "cash").Sum(x => x.Debit - x.Credit * sign);
            revenue.TotalAmount = items.Sum(x => x.Debit - x.Credit * sign);
            revenue.TotalAmountYesterday = await query2.Where(x => x.AccountInternalType == "receivable" && x.Reconciled == true && x.ExcludeFromInvoiceTab == false).SumAsync(x => x.Debit - x.Credit * sign);

            return revenue;
        }

        protected T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }


    }



}
