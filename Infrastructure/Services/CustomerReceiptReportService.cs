﻿using ApplicationCore.Entities;
using ApplicationCore.Models;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class CustomerReceiptReportService : ICustomerReceiptReportService
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        public CustomerReceiptReportService(CatalogDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _userManager = userManager;
        }


        public async Task<PagedResult2<CustomerReceiptReportBasic>> GetPagedResultAsync(CustomerReceiptReportFilter val)
        {
            var query = GetQueryable(val);

            var totalItems = await query.CountAsync();

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            query = query.OrderByDescending(x => x.DateWaiting);

            var items = await query.Include(x => x.Company).Include(x => x.Doctor).Include(x => x.Partner).ToListAsync();

            var paged = new PagedResult2<CustomerReceiptReportBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<CustomerReceiptReportBasic>>(items)
            };

            return paged;
        }

        public async Task<PagedResult2<CustomerReceiptReportTime>> GetCustomerReceiptForTime(CustomerReceiptReportFilter val)
        {
            var query = GetQueryable(val);
            query = query.OrderByDescending(x => x.DateWaiting);

            var items = await query.Include(x => x.Company).Include(x => x.Doctor).Include(x => x.Partner).ToListAsync();
            var res = new List<CustomerReceiptReportTime>();
            var timeEnd = 21;
            for (var i = 7; i <= timeEnd; i++)
            {
                var timeFrom = DateTime.Now.Date.Add(new TimeSpan(i, 00, 00));
                var timeTo = DateTime.Now.Date.Add(new TimeSpan(i, 59, 59));
                res.Add(new CustomerReceiptReportTime
                {
                    Time = i,
                    TimeRange = $"{i}:00 - {i}:59",
                    TimeRangeCount = items.Where(x => x.DateWaiting.HasValue && timeFrom.TimeOfDay <= x.DateWaiting.Value.TimeOfDay && x.DateWaiting.Value.TimeOfDay <= timeTo.TimeOfDay).Count()
                });

            }

            var totalItems = res.Count();


            var paged = new PagedResult2<CustomerReceiptReportTime>(totalItems, val.Offset, val.Limit)
            {
                Items = res
            };

            return paged;
        }

        public async Task<PagedResult2<CustomerReceiptReportBasic>> GetCustomerReceiptForTimeDetail(CustomerReceiptTimeDetailFilter val)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            var timeFrom = DateTime.Now.Date.Add(new TimeSpan(val.Time, 00, 00));
            var timeTo = DateTime.Now.Date.Add(new TimeSpan(val.Time, 59, 59));

            var query = _context.CustomerReceiptReports.Where(x => companyIds.Contains(x.CompanyId)).AsQueryable();

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateWaiting >= val.DateFrom.Value.AbsoluteBeginOfDate());

            if (val.DateFrom.HasValue)
            {
                var datetimeTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.DateWaiting <= datetimeTo);
            }


            if (val.CompanyId.HasValue)
            {
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);
            }

            query = query.Where(x => x.DateWaiting.HasValue && timeFrom.TimeOfDay <= x.DateWaiting.Value.TimeOfDay && x.DateWaiting.Value.TimeOfDay <= timeTo.TimeOfDay);

            var totalItems = await query.CountAsync();

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            query = query.OrderByDescending(x => x.DateWaiting);

            var items = await query.Include(x => x.Company).Include(x => x.Doctor).Include(x => x.Partner).ToListAsync();

            var paged = new PagedResult2<CustomerReceiptReportBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<CustomerReceiptReportBasic>>(items)
            };

            return paged;
        }

        public async Task<IEnumerable<CustomerReceiptGetCountItem>> GetCountCustomerReceipt(CustomerReceiptReportFilter val)
        {
            var query = GetQueryable(val);

            var totalItems = await query.CountAsync();

            var items = await query.ToListAsync();

            var res = items.GroupBy(x => x.IsRepeatCustomer).Select(x => new CustomerReceiptGetCountItem
            {
                Name = x.Key == true ? "Tái khám" : "Khám mới",
                Color = x.Key == true ? "#1A6DE3" : "#95C8FF",
                TotalCustomerReceipt = totalItems,
                CountCustomerReceipt = x.Count()
            }).ToList();

            return res;
        }

        public async Task<IEnumerable<CustomerReceiptGetCountItem>> GetCountCustomerReceiptNotreatment(CustomerReceiptReportFilter val)
        {
            var query = GetQueryable(val);

            var items = await query.Where(x => x.State == "done").ToListAsync();

            var totalItems = items.Count();

            var res = items.GroupBy(x => x.IsNoTreatment).Select(x => new CustomerReceiptGetCountItem
            {
                Name = x.Key == true ? "Có điều trị" : "Không điều trị",
                Color = x.Key == true ? "#1A6DE3" : "#95C8FF",
                TotalCustomerReceipt = totalItems,
                CountCustomerReceipt = x.Count()
            }).ToList();

            return res;
        }

        public async Task<long> GetCountTime(CustomerReceiptReportFilter val)
        {
            var query = GetQueryable(val);
            return await query.LongCountAsync();
        }


        private IQueryable<CustomerReceiptReport> GetQueryable(CustomerReceiptReportFilter val)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();

            var query = _context.CustomerReceiptReports.Where(x => companyIds.Contains(x.CompanyId)).AsQueryable();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Partner.Name.Contains(val.Search) || x.Partner.Ref.Contains(val.Search) || x.Partner.Phone.Contains(val.Search));

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateWaiting >= val.DateFrom.Value.AbsoluteBeginOfDate());

            if (!string.IsNullOrEmpty(val.state))
                query = query.Where(x => x.State == val.state);

            if (val.DateFrom.HasValue)
            {
                var datetimeTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.DateWaiting <= datetimeTo);
            }

            if (val.TimeFrom.HasValue)
                query = query.Where(x => x.MinuteTotal.HasValue && x.MinuteTotal > val.TimeFrom.Value);


            if (val.TimeTo.HasValue)
            {
                query = query.Where(x => x.MinuteTotal.HasValue && x.MinuteTotal <= val.TimeTo.Value);
            }

            if (val.CompanyId.HasValue)
            {
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);
            }

            if (val.DoctorId.HasValue)
            {
                query = query.Where(x => x.DoctorId == val.DoctorId.Value);
            }

            if (val.IsNoTreatment.HasValue)
            {
                query = query.Where(x => x.IsNoTreatment == val.IsNoTreatment.Value);
            }

            if (val.IsRepeatCustomer.HasValue)
            {
                query = query.Where(x => x.IsRepeatCustomer == val.IsRepeatCustomer.Value);
            }


            return query;
        }

        private T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
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

        public async Task<CustomerReceiptReportPdf<CustomerReceiptReportBasic>> GetPagedResultPdf(CustomerReceiptReportFilter val)
        {
            val.Limit = 0;
            var data = await GetPagedResultAsync(val);
            
            var res = new CustomerReceiptReportPdf<CustomerReceiptReportBasic>()
            {
                Data = data.Items,
                DateFrom = val.DateFrom,
                DateTo = val.DateTo
            };
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == UserId);
            res.User = _mapper.Map<ApplicationUserSimple>(user);

            if (val.CompanyId.HasValue)
            {
                var companyObj = GetService<ICompanyService>();
                res.Company = _mapper.Map<CompanyPrintVM>(await companyObj.SearchQuery(x => x.Id == val.CompanyId)
                    .Include(x => x.Partner).FirstOrDefaultAsync());
            }
            return res;
        }

        public async Task<CustomerReceiptReportPdf<CustomerReceiptForTimePdf>> GetCustomerReceiptForTimePdf(CustomerReceiptReportFilter val)
        {
            val.Limit = 0;
            var data = await GetCustomerReceiptForTime(val);
            var allData = _mapper.Map<IEnumerable<CustomerReceiptForTimePdf>>(data.Items);

            var valDetail = _mapper.Map<CustomerReceiptTimeDetailFilter>(val);
            var allLines = await GetCustomerReceiptForTimeDetail(valDetail);
            foreach (var item in allData)
            {
                var timeFrom = DateTime.Now.Date.Add(new TimeSpan(item.Time, 00, 00));
                var timeTo = DateTime.Now.Date.Add(new TimeSpan(item.Time, 59, 59));
                item.Lines = allLines.Items.Where(x => x.DateWaiting.HasValue && timeFrom.TimeOfDay <= x.DateWaiting.Value.TimeOfDay
                                                       && x.DateWaiting.Value.TimeOfDay <= timeTo.TimeOfDay).ToList();
            }
            var res = new CustomerReceiptReportPdf<CustomerReceiptForTimePdf>()
            {
                Data = allData,
                DateFrom = val.DateFrom,
                DateTo = val.DateTo
            };
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == UserId);
            res.User = _mapper.Map<ApplicationUserSimple>(user);

            if (val.CompanyId.HasValue)
            {
                var companyObj = GetService<ICompanyService>();
                res.Company = _mapper.Map<CompanyPrintVM>(await companyObj.SearchQuery(x => x.Id == val.CompanyId)
                    .Include(x => x.Partner).FirstOrDefaultAsync());
            }
            return res;
        }
    }
}
