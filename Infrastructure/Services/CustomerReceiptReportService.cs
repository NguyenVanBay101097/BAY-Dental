using ApplicationCore.Entities;
using ApplicationCore.Models;
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
    public class CustomerReceiptReportService : ICustomerReceiptReportService
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CustomerReceiptReportService(CatalogDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }


        public async Task<PagedResult2<CustomerReceiptReport>> GetPagedResultAsync(CustomerReceiptReportFilter val)
        {
            var query = GetQueryable(val);          

            var totalItems = await query.CountAsync();

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            query = query.OrderByDescending(x => x.DateWaiting);

            var items = await query.ToListAsync();

            var paged = new PagedResult2<CustomerReceiptReport>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<CustomerReceiptReport>>(items)
            };

            return paged;
        }




        private IQueryable<CustomerReceiptReport> GetQueryable(CustomerReceiptReportFilter val)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();

            var query = _context.CustomerReceiptReports.Where(x => companyIds.Contains(x.CompanyId)
            ).AsQueryable();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Partner.Name.Contains(val.Search) || x.Partner.Ref.Contains(val.Search) || x.Partner.Phone.Contains(val.Search));

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateWaiting >= val.DateFrom.Value.AbsoluteBeginOfDate());


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
    }
}
