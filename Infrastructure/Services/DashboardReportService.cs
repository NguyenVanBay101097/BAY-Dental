﻿using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Data;
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
        private readonly IMapper _mapper;
        private readonly CatalogDbContext _context;
        public DashboardReportService(IHttpContextAccessor httpContextAccessor, IMapper mapper, CatalogDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _context = context;
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
                throw new Exception("Không thể tiếp nhận lịch hẹn ở trạng thái hủy hẹn");

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


        public async Task<CustomerReceiptReqonse> CreateCustomerReceiptToAppointment(CustomerReceiptRequest val)
        {
            var customerReceiptObj = GetService<ICustomerReceiptService>();
            var appointmentObj = GetService<IAppointmentService>();
            var res = new CustomerReceiptReqonse();
            var customerReceipt = new CustomerReceipt();
            customerReceipt.CompanyId = val.CompanyId;
            customerReceipt.PartnerId = val.PartnerId;
            customerReceipt.DoctorId = val.DoctorId;
            customerReceipt.DateWaiting = val.DateWaiting.HasValue ? val.DateWaiting.Value : DateTime.Now;
            customerReceipt.IsNoTreatment = val.IsRepeatCustomer;
            customerReceipt.Note = val.Note;
            customerReceipt.State = "waiting";
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
            customerReceipt = await customerReceiptObj.SearchQuery(x => x.Id == customerReceipt.Id).Include(x => x.Partner).Include(x => x.Doctor).FirstOrDefaultAsync();
            res.CustomerReceipt = _mapper.Map<CustomerReceiptBasic>(customerReceipt);

            if (val.AppointmentId.HasValue)
            {
                var appointment = await appointmentObj.SearchQuery(x => x.Id == val.AppointmentId)
                    .Include(x => x.Partner)
                    .Include(x => x.Doctor)
                .FirstOrDefaultAsync();

                if (appointment.State == "confirmed")
                    appointment.State = "arrived";

                await appointmentObj.UpdateAsync(appointment);

                res.Appointment = _mapper.Map<AppointmentBasic>(appointment);

            }

            return res;

        }

        public async Task<GetCountMedicalXamination> GetCountMedicalXaminationToday(ReportTodayRequest val)
        {
            var customerReceiptObj = GetService<ICustomerReceiptService>();
         

            var query2 = customerReceiptObj.SearchQuery();

            if (val.DateFrom.HasValue)
                query2 = query2.Where(x => x.DateWaiting > val.DateFrom.Value.AbsoluteBeginOfDate());

            if (val.DateFrom.HasValue)
                query2 = query2.Where(x => x.DateWaiting <= val.DateTo.Value.AbsoluteEndOfDate());

            if (val.CompanyId.HasValue)
                query2 = query2.Where(x => x.CompanyId == val.CompanyId);


            var customerReceiptNewToday = await query2.Where(x => !x.IsRepeatCustomer).CountAsync();
            var customerReceiptOldToday = await query2.Where(x => x.IsRepeatCustomer).CountAsync();

            var res = new GetCountMedicalXamination
            {
                NewMedical = customerReceiptNewToday,
                ReMedical = customerReceiptOldToday
            };

            return res;
        }



        public async Task<RevenueTodayReponse> GetSumary(ReportTodayRequest val)
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

        public async Task<SumaryRevenueReport> GetSumaryRevenueReport(SumaryRevenueReportFilter val)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var RevenueReport = new SumaryRevenueReport();
            var sign = -1;

            var types = new string[] { "cash", "bank", "debt", "advance" };
            if (val.ResultSelection == "cash_bank")
                types = new string[] { "cash", "bank" };
            else if (val.ResultSelection == "debt" || val.ResultSelection == "pay_debt")
                types = new string[] { };
            else if (val.ResultSelection == "advance" || val.ResultSelection == "pay_advance")
                types = new string[] { };

            var dateFrom = val.DateFrom;
            if (dateFrom.HasValue)
                dateFrom = dateFrom.Value.AbsoluteBeginOfDate();

            var dateTo = val.DateTo;
            if (dateTo.HasValue)
                dateTo = dateTo.Value.AbsoluteEndOfDate();

            var query = amlObj._QueryGet(dateFrom: dateFrom, dateTo: dateTo, state: "posted", companyId: val.CompanyId);        

            if (types.Any())
                query = query.Where(x => types.Contains(x.Journal.Type));

            query = query.Where(x => x.Account.Code == val.AccountCode);

            RevenueReport.Type = val.ResultSelection;
            RevenueReport.Credit = await query.SumAsync(x => x.Credit);
            RevenueReport.Debit = await query.SumAsync(x => x.Debit);
            RevenueReport.Balance = await query.SumAsync(x => x.Balance * sign);
            return RevenueReport;
        }

        protected T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }


    }



}
