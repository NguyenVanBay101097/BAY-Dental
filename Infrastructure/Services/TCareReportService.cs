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
    public class TCareReportService : ITCareReportService
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TCareReportService(CatalogDbContext context, IMapper mapper,
          IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<TCareReports>> GetReportTCare(TCareScenarioFilterReport val)
        {
            //SearchQuery

            //var campaigns = _context.TCareCampaigns.AsQueryable();

            //if (val.TCareScenarioId.HasValue)
            //    campaigns = campaigns.Where(x => x.TCareScenarioId == val.TCareScenarioId.Value);
            //if (val.DateFrom.HasValue)
            //{
            //    var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();

            //    campaigns = campaigns.Where(x => x.DateCreated.Value >= dateFrom);
            //}
            //if (val.DateTo.HasValue)
            //{
            //    var dateTo = val.DateTo.Value.AbsoluteEndOfDate();

            //    campaigns = campaigns.Where(x => x.DateCreated.Value <= dateTo);
            //}

            //var list = await campaigns.Include(x => x.TCareScenario).Include(x => x.Traces).ToListAsync();

            //var query2 = list.GroupBy(x => new
            //{
            //    x.TCareScenario.Id,
            //    x.TCareScenario.Name,
            //}).Select(x => new TCareReports
            //{
            //    Id = x.Key.Id,
            //    Name = x.Key.Name,
            //    Items = x.Count(),
            //    MessageTotal = x.Sum(s => s.Traces.Count),
            //    DeliveryTotal = x.Sum(s => s.Traces.Where(s => s.Delivery.HasValue).Count()),
            //    ReadTotal = x.Sum(s => s.Traces.Where(s => s.Opened.HasValue).Count())

            //}).ToList();


            //return query2;
            return null;
        }

        public async Task<List<TCareReportsItem>> GetReportTCareDetail(TCareReports val)
        {
            //var traceObj = (ITCareMessagingTraceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(ITCareMessagingTraceService));
            //var campaigns =  _context.TCareCampaigns.Where(x => x.TCareScenarioId == val.Id).Include(x => x.Traces);
            //var reportItems = await campaigns.Select(x => new TCareReportsItem
            //{
            //    Id = x.Id,
            //    Name = x.Name,
            //    MessageTotal = x.Traces.Count(),
            //    DeliveryTotal = x.Traces.Where(s => s.Delivery.HasValue).Count(),
            //    ReadTotal = x.Traces.Where(s => s.Opened.HasValue).Count(),
            //    Active = x.Active
            //}).ToListAsync();

            //return reportItems;
            return null;
        }
    }
}
