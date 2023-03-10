using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class AccountInvoiceReportService : IAccountInvoiceReportService
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountInvoiceReportService(CatalogDbContext context, IAsyncRepository<Product> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        private T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }
        public IQueryable<AccountInvoiceReport> GetRevenueReportQuery(RevenueReportQueryCommon val)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();

            var query = _context.AccountInvoiceReports.Where(x => (x.Type == "out_invoice" || x.Type == "out_refund") && x.State == "posted"
            && x.AccountInternalType != "receivable"
            && (!x.CompanyId.HasValue || companyIds.Contains(x.CompanyId.Value))
            ).AsQueryable();

            if (val.DateFrom.HasValue)
            {
                var dateFrom = new DateTime(val.DateFrom.Value.Year, val.DateFrom.Value.Month, val.DateFrom.Value.Day, 0, 0, 0, 0);
                query = query.Where(x => x.InvoiceDate >= dateFrom);
            }

            if (val.DateTo.HasValue)
            {
                var dateTo = new DateTime(val.DateTo.Value.Year, val.DateTo.Value.Month, val.DateTo.Value.Day, 23, 59, 59, 999);
                query = query.Where(x => x.InvoiceDate <= dateTo);
            }

            if (val.CompanyId.HasValue)
            {
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);
            }

            if (!string.IsNullOrEmpty(val.SearchPartner))
            {
                query = query.Where(x => x.Partner.Name.Contains(val.SearchPartner)
                                        || x.Partner.NameNoSign.Contains(val.SearchPartner)
                                        || x.Partner.Phone.Contains(val.SearchPartner)
                                        || x.Partner.Ref.Contains(val.SearchPartner));
            }

            return query;
        }

        public async Task<IEnumerable<RevenueReportItem>> GetRevenueChartReport(DateTime? dateFrom, DateTime? dateTo , Guid? companyId , string groupBy , string[] accountCode = null)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var res = new List<RevenueReportItem>();

            var query = amlObj._QueryGet(dateFrom: dateFrom, dateTo: dateTo, state: "posted", companyId: companyId);
        
            if (dateFrom.HasValue)
                query = query.Where(x => x.Date >= dateFrom.Value.AbsoluteBeginOfDate());


            if (dateTo.HasValue)
                query = query.Where(x => x.Date <= dateTo.Value.AbsoluteEndOfDate());


            if (companyId.HasValue)
                query = query.Where(x => x.CompanyId == companyId);

            var types = new string[] { "cash", "bank" };
            query = query.Where(x => types.Contains(x.Journal.Type));

            if (accountCode.Any())
                query = query.Where(x => accountCode.Contains(x.Account.Code));

            if (groupBy == "groupby:day")
            {

                res = await query.GroupBy(x => x.Date.Value.Date)
                  .Select(x => new RevenueReportItem
                  {
                      InvoiceDate = x.Key,
                      PriceSubTotal = Math.Abs(x.Sum(z => z.Credit)),
                  }).ToListAsync();
            }
            else if (groupBy == "groupby:month")
            {
                res = await query.GroupBy(x => new
                {
                    x.Date.Value.Year,
                    x.Date.Value.Month,
                })
                 .Select(x => new RevenueReportItem
                 {
                     InvoiceDate = new DateTime(x.Key.Year, x.Key.Month, 1),
                     PriceSubTotal = Math.Abs(x.Sum(z => z.Credit)),
                 }).ToListAsync();
            }
            else if (groupBy == "groupby:employee")
            {
                res = await query.GroupBy(x => new { x.EmployeeId, x.Employee.Name }).Select(x => new RevenueReportItem
                {
                    Id = x.Key.EmployeeId,
                    Name = x.Key.Name,
                    PriceSubTotal = Math.Abs(x.Sum(z => z.Credit)),
                    ToDetailEmployeeId = x.Key.EmployeeId ?? Guid.Empty
                }).ToListAsync();

            }
            else if (groupBy == "groupby:assistant")
            {

                res = await query.GroupBy(x => new { x.AssistantId, x.Assistant.Name }).Select(x => new RevenueReportItem
                {
                    Id = x.Key.AssistantId,
                    Name = x.Key.Name,
                    PriceSubTotal = Math.Abs(x.Sum(z => z.Credit)),
                    ToDetailEmployeeId = x.Key.AssistantId ?? Guid.Empty
                }).ToListAsync();

            }
            else if (groupBy == "groupby:product")
            {

                res = await query.GroupBy(x => new { x.ProductId, x.Product.Name }).Select(x => new RevenueReportItem
                {
                    Id = x.Key.ProductId.Value,
                    Name = x.Key.Name,
                    PriceSubTotal = Math.Abs(x.Sum(z => z.Credit)),
                }).ToListAsync();
            }


            return res;
        }

        public async Task<IEnumerable<RevenueReportItem>> GetRevenueReport(RevenueReportFilter val)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            var res = new List<RevenueReportItem>();

            var query = _context.AccountInvoiceReports.Where(x => (x.Type == "out_invoice" || x.Type == "out_refund") && x.State == "posted"
            && x.AccountInternalType != "receivable"
            && (!x.CompanyId.HasValue || companyIds.Contains(x.CompanyId.Value)));

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.InvoiceDate >= val.DateFrom.Value.AbsoluteBeginOfDate());


            if (val.DateTo.HasValue)
                query = query.Where(x => x.InvoiceDate <= val.DateTo.Value.AbsoluteEndOfDate());

            if (val.AssistantId.HasValue)
                query = query.Where(x => x.AssistantId == val.AssistantId);



            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);

            if (val.GroupBy == "groupby:day")
            {
                if (!string.IsNullOrEmpty(val.Search))
                    query = query.Where(x => x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search) ||
                    x.Partner.Phone.Contains(val.Search) || x.Partner.Ref.Contains(val.Search));

                res = await query.GroupBy(x => x.InvoiceDate.Value.Date)
                  .Select(x => new RevenueReportItem
                  {
                      InvoiceDate = x.Key,
                      PriceSubTotal = Math.Abs(x.Sum(z => z.PriceSubTotal)),
                      CompanyId = val.CompanyId,
                      DateFrom = val.DateFrom,
                      DateTo = val.DateTo,
                  }).ToListAsync();
            }
            else if (val.GroupBy == "groupby:month")
            {
                res =  await query.GroupBy(x => new
                {
                    x.InvoiceDate.Value.Year,
                    x.InvoiceDate.Value.Month,
                })
                 .Select(x => new RevenueReportItem
                 {
                     InvoiceDate = new DateTime(x.Key.Year, x.Key.Month, 1),
                     PriceSubTotal = Math.Abs(x.Sum(z => z.PriceSubTotal)),
                     CompanyId = val.CompanyId,
                     DateFrom = val.DateFrom,
                     DateTo = val.DateTo,
                 }).ToListAsync();
            }
            else if(val.GroupBy == "groupby:employee")
            {
                res = await query.GroupBy(x => new { x.EmployeeId, x.Employee.Name }).Select(x => new RevenueReportItem
                {
                    Id = x.Key.EmployeeId,
                    Name = x.Key.Name,
                    PriceSubTotal = Math.Abs(x.Sum(z => z.PriceSubTotal)),
                    CompanyId = val.CompanyId,
                    DateFrom = val.DateFrom,
                    DateTo = val.DateTo,
                    ToDetailEmployeeId = x.Key.EmployeeId ?? Guid.Empty
                }).ToListAsync();

            }
            else if (val.GroupBy == "groupby:assistant")
            {
        
                res = await query.GroupBy(x => new { x.AssistantId, x.Assistant.Name }).Select(x => new RevenueReportItem
                {
                    Id = x.Key.AssistantId,
                    Name = x.Key.Name,
                    PriceSubTotal = Math.Abs(x.Sum(z => z.PriceSubTotal)),
                    CompanyId = val.CompanyId,
                    DateFrom = val.DateFrom,
                    DateTo = val.DateTo,
                    ToDetailEmployeeId = x.Key.AssistantId ?? Guid.Empty
                }).ToListAsync();

            }
            else if (val.GroupBy == "groupby:product")
            {

                res = await query.GroupBy(x => new { x.ProductId, x.Product.Name } ).Select(x => new RevenueReportItem
                {
                    Id = x.Key.ProductId.Value,
                    Name = x.Key.Name,
                    PriceSubTotal = Math.Abs(x.Sum(z => z.PriceSubTotal)),
                    DateTo = val.DateTo,
                    DateFrom = val.DateFrom,
                    CompanyId = val.CompanyId
                }).ToListAsync();
            }
           

            return res;
        }

        public async Task<IEnumerable<RevenueTimeReportDisplay>> GetRevenueTimeReport(RevenueTimeReportPar val)
        {
            var query = (GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId)))
                            .GroupBy(x => x.InvoiceDate.Value.Date);

            var res = await query.Select(x => new RevenueTimeReportDisplay
            {
                InvoiceDate = x.Key as DateTime?,
                PriceSubTotal = Math.Abs(x.Sum(z => z.PriceSubTotal)),
                CompanyId = val.CompanyId,
                DateFrom = val.DateFrom,
                DateTo = val.DateTo
            }).ToListAsync();

            res = res.OrderByDescending(z => z.InvoiceDate.Value).ToList();

            return res;
        }

        public async Task<IEnumerable<RevenueTimeReportDisplay>> GetRevenueTimeByMonth(RevenueTimeReportPar val)
        {
            var query = (GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId)))
                            .GroupBy(x => new { 
                                Month = x.InvoiceDate.Value.Month,
                                Year = x.InvoiceDate.Value.Year
                            });

            var res = await query.Select(x => new RevenueTimeReportDisplay
            {
                InvoiceDate = new DateTime(x.Key.Year, x.Key.Month, 1),
                PriceSubTotal = Math.Abs(x.Sum(z => z.PriceSubTotal)),
                CompanyId = val.CompanyId,
                DateFrom = val.DateFrom,
                DateTo = val.DateTo
            }).ToListAsync();

            res = res.OrderByDescending(z => z.InvoiceDate.Value).ToList();

            return res;
        }

        public async Task<IEnumerable<RevenueServiceReportDisplay>> GetRevenueServiceReport(RevenueServiceReportPar val)
        {
            var query = GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId));
            if (val.ProductId.HasValue)
            {
                query = query.Where(x => x.ProductId == val.ProductId);
            }
            var queryRes = query.GroupBy(x => new { x.ProductId, x.Product.Name });

            var res = await queryRes.Select(x => new RevenueServiceReportDisplay
            {
                ProductId = x.Key.ProductId.Value,
                ProductName = x.Key.Name,
                PriceSubTotal = Math.Abs(x.Sum(z => z.PriceSubTotal)),
                DateTo = val.DateTo,
                DateFrom = val.DateFrom,
                CompanyId = val.CompanyId
            }).ToListAsync();

            return res;
        }

        public async Task<IEnumerable<RevenueEmployeeReportDisplay>> GetRevenueEmployeeReport(RevenueEmployeeReportPar val)
        {
            var query = GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId));
            IQueryable<IGrouping<object, AccountInvoiceReport>> queryRes;

            switch (val.GroupBy)
            {
                case "employee":
                    if (val.GroupById.HasValue)
                    {
                        query = query.Where(x => x.EmployeeId == val.GroupById);
                    }
                    queryRes = query.GroupBy(x => new EmployeeAssistantKeyGroup { Id = x.EmployeeId, Name = x.Employee.Name });
                    break;
                case "assistant":
                    if (val.GroupById.HasValue)
                    {
                        query = query.Where(x => x.AssistantId == val.GroupById);
                    }
                    queryRes = query.GroupBy(x => new EmployeeAssistantKeyGroup { Id = x.AssistantId, Name = x.Assistant.Name });
                    break;
                default:
                    throw new Exception("Vui lòng chọn trường cần group");
            }

            var res = await queryRes.Select(x => new RevenueEmployeeReportDisplay
            {
                EmployeeId = (x.Key as EmployeeAssistantKeyGroup).Id,
                EmployeeName = (x.Key as EmployeeAssistantKeyGroup).Name,
                PriceSubTotal = Math.Abs(x.Sum(z => z.PriceSubTotal)),
                GroupBy = val.GroupBy,
                CompanyId = val.CompanyId,
                DateFrom = val.DateFrom,
                DateTo = val.DateTo,
                ToDetailEmployeeId = (x.Key as EmployeeAssistantKeyGroup).Id ?? Guid.Empty
            }).ToListAsync();

            return res;
        }

        public async Task<PagedResult2<RevenueReportDetailDisplay>> GetRevenueReportDetailPaged(RevenueReportDetailPaged val)
        {
            var query = GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId));

            var res = new List<RevenueReportDetailDisplay>();
            var count = 0;

            if (val.ProductId.HasValue)
            {
                query = query.Where(x => x.ProductId == val.ProductId);
            }
            if (val.EmployeeId.HasValue && val.EmployeeId != Guid.Empty)
            {
                query = query.Where(x => x.EmployeeId == val.EmployeeId);
            }
            if (val.EmployeeId.HasValue && val.EmployeeId == Guid.Empty)
            {
                query = query.Where(x => x.EmployeeId == null);
            }
            if (val.AssistantId.HasValue && val.AssistantId != Guid.Empty)
            {
                query = query.Where(x => x.AssistantId == val.AssistantId);
            }
            if (val.AssistantId.HasValue && val.AssistantId == Guid.Empty)
            {
                query = query.Where(x => x.AssistantId == null);
            }
            if (val.PartnerId.HasValue)
            {
                query = query.Where(x => x.PartnerId == val.PartnerId);
            }

            count = await query.CountAsync();
            query = query.OrderByDescending(x => x.InvoiceDate);
            if (val.Limit > 0) query = query.Skip(val.Offset).Take(val.Limit);
            res = await query.Select(x => new RevenueReportDetailDisplay
            {
                InvoiceDate = x.InvoiceDate,
                AssistantName = x.Assistant.Name,
                EmployeeName = x.Employee.Name,
                EmployeeId = x.EmployeeId,
                AssistantId = x.AssistantId,
                InvoiceOrigin = x.InvoiceOrigin,
                PartnerName = x.Partner.DisplayName,
                PartnerId = x.PartnerId,
                PriceSubTotal = Math.Abs(x.PriceSubTotal),
                ProductName = x.Product.Name,
                ProductId = x.ProductId,
                ProductUoMName = x.ProductUoM.Name
            }).ToListAsync();

            return new PagedResult2<RevenueReportDetailDisplay>(count, val.Offset, val.Limit)
            {
                Items = res
            };
        }

        public async Task<decimal> SumRevenueReport(SumRevenueReportPar val)
        {
            var query = GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId));
            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId);
            var res = await query.SumAsync(x => x.PriceSubTotal);
            return res;
        }

     

        public async Task<IEnumerable<RevenuePartnerReportDisplay>> GetRevenuePartnerReport(RevenuePartnerReportPar val)
        {
            var query = GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId, val.Search));

            var queryRes = query.GroupBy(x => new { x.PartnerId, x.Partner.DisplayName });

            var res = await queryRes.Select(x => new RevenuePartnerReportDisplay
            {
                PartnerId = x.Key.PartnerId.Value,
                PartnerName = x.Key.DisplayName,
                PriceSubTotal = Math.Abs(x.Sum(z => z.PriceSubTotal)),
                DateTo = val.DateTo,
                DateFrom = val.DateFrom,
                CompanyId = val.CompanyId,
                Search = val.Search
            }).ToListAsync();

            return res;
        }
        private string UserId
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    return null;

                return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
        }

        public async Task<RevenueReportPrintVM<RevenueTimeReportPrint>> GetRevenueTimeReportPrint(RevenueTimeReportPar val)
        {
            var data = _mapper.Map<IEnumerable<RevenueTimeReportPrint>>(await GetRevenueTimeReport(val));
            var detailReq = new RevenueReportDetailPaged()
            {
                CompanyId = val.CompanyId,
                Limit = 0,
                DateFrom = val.DateFrom,
                DateTo = val.DateTo
            };
            var allLines = await GetRevenueReportDetailPaged(detailReq);

            foreach (var item in data)
            {
                item.Lines = allLines.Items.Where(x => x.InvoiceDate.Value.Date == item.InvoiceDate.Value.Date).ToList();
            }

            var res = new RevenueReportPrintVM<RevenueTimeReportPrint>(val.DateFrom, val.DateTo)
            {
                Data = data,
                User = _mapper.Map<ApplicationUserSimple>(await _userManager.Users.FirstOrDefaultAsync(x => x.Id == UserId))
            };

            if (val.CompanyId.HasValue)
            {
                var companyObj = GetService<ICompanyService>();
                var company = await companyObj.SearchQuery(x => x.Id == val.CompanyId).Include(x => x.Partner).FirstOrDefaultAsync();
                res.Company = _mapper.Map<CompanyPrintVM>(company);
            }
            return res;
        }

        public async Task<RevenueReportPrintVM<RevenueServiceReportPrint>> GetRevenueServiceReportPrint(RevenueServiceReportPar val)
        {
            var data = _mapper.Map<IEnumerable<RevenueServiceReportPrint>>(await GetRevenueServiceReport(val));
            var detailReq = new RevenueReportDetailPaged()
            {
                CompanyId = val.CompanyId,
                Limit = 0,
                DateFrom = val.DateFrom,
                DateTo = val.DateTo
            };
            var allLines = await GetRevenueReportDetailPaged(detailReq);

            foreach (var item in data)
            {
                item.Lines = allLines.Items.Where(x => x.ProductId == item.ProductId).ToList();
            }
            await _userManager.Users.FirstOrDefaultAsync(x => x.Id == UserId);
            var res = new RevenueReportPrintVM<RevenueServiceReportPrint>(val.DateFrom, val.DateTo)
            {
                Data = data,
                User = _mapper.Map<ApplicationUserSimple>(await _userManager.Users.FirstOrDefaultAsync(x => x.Id == UserId))
            };

            if (val.CompanyId.HasValue)
            {
                var companyObj = GetService<ICompanyService>();
                var company = await companyObj.SearchQuery(x => x.Id == val.CompanyId).Include(x => x.Partner).FirstOrDefaultAsync();
                res.Company = _mapper.Map<CompanyPrintVM>(company);
            }
            return res;
        }

        public async Task<RevenueReportPrintVM<RevenueEmployeeReportPrint>> GetRevenueEmployeeReportPrint(RevenueEmployeeReportPar val)
        {
            var data = _mapper.Map<IEnumerable<RevenueEmployeeReportPrint>>(await GetRevenueEmployeeReport(val));
            var detailReq = new RevenueReportDetailPaged()
            {
                CompanyId = val.CompanyId,
                Limit = 0,
                DateFrom = val.DateFrom,
                DateTo = val.DateTo
            };
            var allLines = await GetRevenueReportDetailPaged(detailReq);

            foreach (var item in data)
            {
                item.Lines = allLines.Items.Where(x => x.EmployeeId == item.EmployeeId).ToList();
            }

            var res = new RevenueReportPrintVM<RevenueEmployeeReportPrint>(val.DateFrom, val.DateTo)
            {
                Data = data,
                User = _mapper.Map<ApplicationUserSimple>(await _userManager.Users.FirstOrDefaultAsync(x => x.Id == UserId))
            };

            if (val.CompanyId.HasValue)
            {
                var companyObj = GetService<ICompanyService>();
                var company = await companyObj.SearchQuery(x => x.Id == val.CompanyId).Include(x => x.Partner).FirstOrDefaultAsync();
                res.Company = _mapper.Map<CompanyPrintVM>(company);
            }
            return res;
        }
        public async Task<RevenueReportPrintVM<RevenuePartnerReportPrint>> GetRevenuePartnerReportPrint(RevenuePartnerReportPar val)
        {
            var data = _mapper.Map<IEnumerable<RevenuePartnerReportPrint>>(await GetRevenuePartnerReport(val));
            var detailReq = new RevenueReportDetailPaged()
            {
                CompanyId = val.CompanyId,
                Limit = 0,
                DateFrom = val.DateFrom,
                DateTo = val.DateTo
            };
            var allLines = await GetRevenueReportDetailPaged(detailReq);

            foreach (var item in data)
            {
                item.Lines = allLines.Items.Where(x => x.PartnerId == item.PartnerId).ToList();
            }

            var res = new RevenueReportPrintVM<RevenuePartnerReportPrint>(val.DateFrom, val.DateTo)
            {
                Data = data,
                User = _mapper.Map<ApplicationUserSimple>(await _userManager.Users.FirstOrDefaultAsync(x => x.Id == UserId))
            };

            if (val.CompanyId.HasValue)
            {
                var companyObj = GetService<ICompanyService>();
                var company = await companyObj.SearchQuery(x => x.Id == val.CompanyId).Include(x => x.Partner).FirstOrDefaultAsync();
                res.Company = _mapper.Map<CompanyPrintVM>(company);
            }
            return res;
        }

        public async Task<RevenueReportExcelVM<RevenueTimeReportExcel>> GetRevenueTimeReportExcel(RevenueTimeReportPar val)
        {
            var data = _mapper.Map<IEnumerable<RevenueTimeReportExcel>>(await GetRevenueTimeReport(val));
            var detailReq = new RevenueReportDetailPaged()
            {
                CompanyId = val.CompanyId,
                Limit = 0,
                DateFrom = val.DateFrom,
                DateTo = val.DateTo
            };
            var allLines = await GetRevenueReportDetailPaged(detailReq);

            foreach (var item in data)
            {
                item.Lines = allLines.Items.Where(x => x.InvoiceDate.Value.Date == item.InvoiceDate.Value.Date).ToList();
            }

            var res = new RevenueReportExcelVM<RevenueTimeReportExcel>(val.DateFrom, val.DateTo)
            {
                Title = "BÁO CÁO DOANH THU THEO THỜI GIAN",
                ColumnTitle = "Ngày",
                Data = data
            };
            return res;
        }

        public async Task<RevenueReportExcelVM<RevenueServiceReportExcel>> GetRevenueServiceReportExcel(RevenueServiceReportPar val)
        {
            var data = _mapper.Map<IEnumerable<RevenueServiceReportExcel>>(await GetRevenueServiceReport(val));
            var detailReq = new RevenueReportDetailPaged()
            {
                CompanyId = val.CompanyId,
                Limit = 0,
                DateFrom = val.DateFrom,
                DateTo = val.DateTo
            };
            var allLines = await GetRevenueReportDetailPaged(detailReq);

            foreach (var item in data)
            {
                item.Lines = allLines.Items.Where(x => x.ProductId == item.ProductId).ToList();
            }

            var res = new RevenueReportExcelVM<RevenueServiceReportExcel>(val.DateFrom, val.DateTo)
            {
                Title = "BÁO CÁO DOANH THU THEO DỊCH VỤ",
                ColumnTitle = "Dịch vụ & Thuốc",
                Data = data
            };
            return res;
        }

        public async Task<RevenueReportExcelVM<RevenueEmployeeReportExcel>> GetRevenueEmployeeReportExcel(RevenueEmployeeReportPar val)
        {
            var data = _mapper.Map<IEnumerable<RevenueEmployeeReportExcel>>(await GetRevenueEmployeeReport(val));
            var detailReq = new RevenueReportDetailPaged()
            {
                CompanyId = val.CompanyId,
                Limit = 0,
                DateFrom = val.DateFrom,
                DateTo = val.DateTo
            };
            var allLines = await GetRevenueReportDetailPaged(detailReq);

            foreach (var item in data)
            {
                item.Lines = allLines.Items.Where(x => x.EmployeeId == item.EmployeeId).ToList();
            }

            var res = new RevenueReportExcelVM<RevenueEmployeeReportExcel>(val.DateFrom, val.DateTo)
            {
                Title = "BÁO CÁO DOANH THU THEO NHÂN VIÊN",
                ColumnTitle = "Nhân viên",
                Data = data
            };
            return res;
        }

        public async Task<RevenueReportExcelVM<RevenuePartnerReportExcel>> GetRevenuePartnerReportExcel(RevenuePartnerReportPar val)
        {
            var data = _mapper.Map<IEnumerable<RevenuePartnerReportExcel>>(await GetRevenuePartnerReport(val));
            var detailReq = new RevenueReportDetailPaged()
            {
                CompanyId = val.CompanyId,
                Limit = 0,
                DateFrom = val.DateFrom,
                DateTo = val.DateTo
            };
            var allLines = await GetRevenueReportDetailPaged(detailReq);

            foreach (var item in data)
            {
                item.Lines = allLines.Items.Where(x => x.PartnerId == item.PartnerId).ToList();
            }

            var res = new RevenueReportExcelVM<RevenuePartnerReportExcel>(val.DateFrom, val.DateTo)
            {
                Title = "BÁO CÁO DOANH THU THEO KHÁCH HÀNG",
                ColumnTitle = "Khách hàng",
                Data = data
            };
            return res;
        }
        // doanh thu khách hàng điều trị theo khu vực
        public async Task<IEnumerable<RevenueDistrictAreaDisplay>> GetRevenueDistrictArea(RevenueDistrictAreaPar val)
        {
            var saleOrderObj = GetService<ISaleOrderService>();
            var query = saleOrderObj.SearchQuery(x => x.State != "draft");
            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateOrder.Date >= val.DateFrom.Value.AbsoluteBeginOfDate());
            if (val.DateTo.HasValue)
                query = query.Where(x => x.DateOrder.Date <= val.DateTo.Value.AbsoluteEndOfDate());
            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);
            if (!string.IsNullOrEmpty(val.CityCode))
                query = query.Where(x => x.Partner.CityCode == val.CityCode);

            var revenueTotal = query.Sum(x => x.TotalPaid ?? 0);
            var saleOrderPartners = query.GroupBy(x => new
            {
                PartnerId = x.PartnerId,
                DistrictName = x.Partner.DistrictName,
                DistrictCode = x.Partner.DistrictCode,
                CityCode = x.Partner.CityCode,
                CityName = x.Partner.CityName,
            })
            .Select(x => new
            {
                PartnerId = x.Key.PartnerId,
                DistrictName = x.Key.DistrictName,
                DistrictCode = x.Key.DistrictCode,
                CityCode = x.Key.CityCode,
                CityName = x.Key.CityName,
                Revenue = x.Sum(z => z.TotalPaid)
            });
            var grList = await saleOrderPartners.GroupBy(x => new { DistrictName = x.DistrictName, DistrictCode = x.DistrictCode, CityCode = x.CityCode, CityName = x.CityName })
                                .Select(x => new RevenueDistrictAreaDisplay()
                                {
                                    CityCode = x.Key.CityCode,
                                    CityName = x.Key.CityName,
                                    DistrictName = x.Key.DistrictName,
                                    DistrictCode = x.Key.DistrictCode,
                                    Revenue = x.Sum(z => z.Revenue),
                                    PartnerCount = x.Count(),
                                }).OrderByDescending(x => x.Revenue).ToListAsync();
            return grList;
            //var dictGrList = grList.ToDictionary(x => x.DistrictCode, x => x);
            //foreach(var item in dictGrList)
            //{
            //    dictGrList[item.Key].Percent = (revenueTotal != 0 && dictGrList[item.Key].Revenue.HasValue) ? Math.Round(dictGrList[item.Key].Revenue.Value / revenueTotal * 100, 2) + "%" : "0%";
            //}

            //var list = dictGrList.Values.ToList();
            //if (list.Count() <= 10)
            //    return list;

            //var result = new List<RevenueDistrictAreaDisplay>();
            //var top10 = list.Skip(0).Take(10);
            //var otherList = list.Skip(10);
            //var revenueOther = otherList.Sum(x => x.Revenue ?? 0);
            //var partnerOtherCount = otherList.Sum(x => x.PartnerCount);

            //var otherObj = new RevenueDistrictAreaDisplay()
            //{
            //    DistrictName = "Khác",
            //    Revenue = revenueOther,
            //    PartnerCount = partnerOtherCount,
            //    Percent = revenueTotal != 0 ? Math.Round(revenueOther / revenueTotal * 100, 2) + "%" : "0%"
            //};

            //result.AddRange(top10);
            //result.Add(otherObj);
            //return result;
        }
    }
}
