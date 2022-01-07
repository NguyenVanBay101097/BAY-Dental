using ApplicationCore.Entities;
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
            customerReceipt.IsRepeatCustomer = val.IsRepeatCustomer;
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

            var journalTypes = new string[] { "cash", "bank", "advance", "debt" }; //danh sach cac phuong thuc thanh toan
            query = query.Where(x => x.AccountInternalType == "receivable");

            var sign = -1;
            revenue.TotalBank = query.Where(x => x.Journal.Type == "bank").Sum(x => x.Debit - x.Credit * sign);
            revenue.TotalCash = query.Where(x => x.Journal.Type == "cash").Sum(x => x.Debit - x.Credit * sign);
            revenue.TotalAmount = query.Where(x => journalTypes.Contains(x.Journal.Type)).Sum(x => x.Debit - x.Credit * sign);
            revenue.TotalAmountYesterday = await query2.Where(x => x.AccountInternalType == "receivable" && journalTypes.Contains(x.Journal.Type)).SumAsync(x => x.Debit - x.Credit * sign);

            return revenue;
        }

        public async Task<SumaryRevenueReport> GetSumaryRevenueReport(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string accountCode, string resultSelection)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var RevenueReport = new SumaryRevenueReport();
            var sign = -1;

            if (dateFrom.HasValue)
                dateFrom = dateFrom.Value.AbsoluteBeginOfDate();

            if (dateTo.HasValue)
                dateTo = dateTo.Value.AbsoluteEndOfDate();

            var query = amlObj._QueryGet(dateFrom: dateFrom, dateTo: dateTo, state: "posted", companyId: companyId);

            var types = new string[] { "cash", "bank" };
            query = query.Where(x => types.Contains(x.Journal.Type));

            query = query.Where(x => x.Account.Code == accountCode);

            RevenueReport.Type = resultSelection;
            RevenueReport.Credit = await query.SumAsync(x => x.Credit);
            RevenueReport.Debit = await query.SumAsync(x => x.Debit);
            RevenueReport.Balance = await query.SumAsync(x => x.Balance * sign);
            return RevenueReport;
        }

        public async Task<IEnumerable<ReportRevenueChart>> GetRevenueChartReport(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string groupBy)
        {
            var invoiceObj = GetService<IAccountInvoiceReportService>();
            var cashbookObj = GetService<ICashBookService>();

            var accCodes = new string[] { "131", "CNKH", "KHTU" };
            var revenues = await invoiceObj.GetRevenueChartReport(dateFrom: dateFrom, dateTo: dateTo, companyId: companyId, groupBy: groupBy, accountCode: accCodes);
            var revenue_dict = revenues.ToDictionary(x => x.InvoiceDate, x => x);

            var cashbooks = await cashbookObj.GetCashBookChartReport(dateFrom: dateFrom, dateTo: dateTo, companyId: companyId, groupBy: groupBy);
            var cashbook_dict = cashbooks.ToDictionary(x => x.Date, x => x);

            var dates = revenues.Select(x => x.InvoiceDate).Union(cashbooks.Select(s => s.Date.Value));

            var res = new List<ReportRevenueChart>();
            foreach (var date in dates)
            {
                var item = new ReportRevenueChart();
                item.Date = date;
                item.AmountRevenue = revenue_dict.ContainsKey(date) ? revenue_dict[date].PriceSubTotal : 0;
                item.AmountCashBook = cashbook_dict.ContainsKey(date) ? cashbook_dict[date].TotalThu : 0;

                res.Add(item);
            }

            res = res.OrderBy(x => x.Date).ToList();

            return res;

        }
        public async Task<GetThuChiReportResponse> GetThuChiReport(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, Guid? journalId)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var userService = GetService<IUserService>();
            //báo cáo doanh thu thực thu
            var resdateFrom = dateFrom.HasValue ? dateFrom.Value.AbsoluteBeginOfDate() : (DateTime?)null;
            var resdateTo = dateTo.HasValue ? dateTo.Value.AbsoluteEndOfDate() : (DateTime?)null;
            var query = amlObj._QueryGet(dateTo: resdateTo, dateFrom: resdateFrom, state: "posted", companyId: companyId);
            query = query.Where(x => (x.Journal.Type == "cash" || x.Journal.Type == "bank") && x.AccountInternalType != "liquidity");

            if (journalId.HasValue)
                query = query.Where(x => x.JournalId == journalId);

            var customerIncomeTotal = await query.Where(x => x.AccountInternalType == "receivable").SumAsync(x => x.Credit);
            var advanceIncomeTotal = await query.Where(x => x.Account.Code == "KHTU").SumAsync(x => x.Credit);
            var debtIncomeTotal = await query.Where(x => x.Account.Code == "CNKH").SumAsync(x => x.Credit);

            var insuranceIncomeTotal = 0.0M;
            if (await userService.HasGroup("insurance.group_insurance"))
                insuranceIncomeTotal = await query.Where(x => x.Account.Code == "CNBH").SumAsync(x => x.Credit);
            var supplierIncomeTotal = await query.Where(x => x.AccountInternalType == "payable").SumAsync(x => x.Credit);
            var cashBankIncomeTotal = await query.SumAsync(x => x.Credit);

            var supplierExpenseTotal = await query.Where(x => x.AccountInternalType == "payable").SumAsync(x => x.Debit);
            var advanceExpenseTotal = await query.Where(x => x.Account.Code == "HTU").SumAsync(x => x.Debit);
            var salaryExpenseTotal = await query.Where(x => x.Account.Code == "334").SumAsync(x => x.Debit);
            var commissionExpenseTotal = await query.Where(x => x.Account.Code == "HHNGT").SumAsync(x => x.Debit);
            var cashBankExpenseTotal = await query.SumAsync(x => x.Debit);

            return new GetThuChiReportResponse
            {
                CustomerIncomeTotal = customerIncomeTotal,
                AdvanceIncomeTotal = advanceIncomeTotal,
                DebtIncomeTotal = debtIncomeTotal,
                InsuranceIncomeTotal = insuranceIncomeTotal,
                SupplierIncomeTotal = supplierIncomeTotal,
                CashBankIncomeTotal = cashBankIncomeTotal,
                SupplierExpenseTotal = supplierExpenseTotal,
                AdvanceExpenseTotal = advanceExpenseTotal,
                SalaryExpenseTotal = salaryExpenseTotal,
                CommissionExpenseTotal = commissionExpenseTotal,
                CashBankExpenseTotal = cashBankExpenseTotal
            };
        }


        public async Task<CashBookReportDay> GetDataCashBookReportDay(DateTime? dateFrom, DateTime? dateTo, Guid? companyId)
        {
            var cashbookObj = GetService<ICashBookService>();

            var res = new CashBookReportDay();
            /// load dữ liệu tổng tiền 
            var types = new string[] { "", "cash", "bank" };
            var summary = await cashbookObj.GetSumaryDayReport(dateFrom, dateTo, companyId, null, null);
            res.SumaryDayReport = summary;

            /// load dữ liệu báo cáo thu chi
            var reportThuChi = await GetThuChiReport(dateFrom, dateTo, companyId, null);

            res.DataThuChiReport = reportThuChi;
            /// load dữ liệu chi tiết
            var cashbooks = await cashbookObj.GetDetails(dateFrom, dateTo, 0, 0, companyId, null, null, null);
            res.DataDetails = cashbooks.Items;

            return res;
        }

        protected T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }

        public async Task<IEnumerable<RevenueReportChartVM>> GetRevenueReportChart(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string groupBy)
        {
            var saleOrderObj = GetService<ISaleOrderService>();
            var accountMoveLineObj = GetService<IAccountMoveLineService>();
            var accountMoveObj = GetService<IAccountMoveService>();
            var accountInvoiceReportObj = GetService<IAccountInvoiceReportService>();

            var res = new List<RevenueReportChartVM>();

            var types = new string[] { "cash", "bank" };

            var amountTotalQuery = saleOrderObj.SearchQuery(x => x.State != "draft" && (!companyId.HasValue || x.CompanyId == companyId.Value))
                .GroupBy(x => x.DateOrder.Date)
                .Select(x => new
                {
                    Date = x.Key,
                    AmountTotal = x.Sum(y => y.AmountTotal ?? 0),
                    Type = "Sale",
                });

            var revenueQuery = _context.AccountInvoiceReports.Where(x => (x.Type == "out_invoice" || x.Type == "out_refund") && x.State == "posted"
            && x.AccountInternalType != "receivable" && (!companyId.HasValue || x.CompanyId == companyId.Value))
                .GroupBy(x => x.InvoiceDate.Value.Date)
                .Select(x => new
                {
                    Date = x.Key,
                    AmountTotal = x.Sum(y => y.PriceSubTotal),
                    Type = "Revenue"
                });


            var totalThuQuery = accountMoveLineObj.SearchQuery(x => x.Move.State == "posted" && x.AccountInternalType == "liquidity"
            && types.Contains(x.Journal.Type) &&(!companyId.HasValue || x.CompanyId == companyId.Value)).GroupBy(x => x.Date.Value.Date)
            .Select(x => new
            {
                Date = x.Key,
                AmountTotal = x.Sum(y => y.Debit),
                Type = "Liquidity"
            });

            var combineQuery = amountTotalQuery.Union(revenueQuery).Union(totalThuQuery);

            if (groupBy == "groupby:day")
            {
                if (dateFrom.HasValue)
                    combineQuery = combineQuery.Where(x => x.Date >= dateFrom.Value.AbsoluteBeginOfDate());

                if (dateTo.HasValue)
                    combineQuery = combineQuery.Where(x => x.Date <= dateTo.Value.AbsoluteEndOfDate());

                res = await combineQuery.GroupBy(x => x.Date)
                  .Select(x => new RevenueReportChartVM
                  {
                      Date = x.Key,
                      TotalSaleAmount = x.Sum(s => s.Type == "Sale" ? s.AmountTotal : 0),
                      TotalRevenueAmount = x.Sum(s => s.Type == "Revenue" ? s.AmountTotal : 0),
                      TotalLiquidityAmount = x.Sum(s => s.Type == "Liquidity" ? s.AmountTotal : 0),
                  }).ToListAsync();

            }
            if (groupBy == "groupby:month")
            {
                if (dateFrom.HasValue)
                {
                    var dateFromTmp = new DateTime(dateFrom.Value.Year, dateFrom.Value.Month, 1);
                    combineQuery = combineQuery.Where(x => x.Date >= dateFromTmp);
                }

                if (dateTo.HasValue)
                {
                    var dateToTmp = new DateTime(dateTo.Value.Year, dateTo.Value.Month, DateTime.DaysInMonth(dateTo.Value.Year, dateTo.Value.Month));
                    combineQuery = combineQuery.Where(x => x.Date <= dateToTmp.AbsoluteEndOfDate());
                }

                res = await combineQuery.GroupBy(x => new
                {
                    x.Date.Year,
                    x.Date.Month,
                })
                 .Select(x => new RevenueReportChartVM
                 {
                     Date = new DateTime(x.Key.Year, x.Key.Month, 1),
                     TotalSaleAmount = x.Sum(s => s.Type == "Sale" ? s.AmountTotal : 0),
                     TotalRevenueAmount = x.Sum(s => s.Type == "Revenue" ? s.AmountTotal : 0),
                     TotalLiquidityAmount = x.Sum(s => s.Type == "Liquidity" ? s.AmountTotal : 0),
                 }).ToListAsync();

            }

            return res;
        }
    }



}
