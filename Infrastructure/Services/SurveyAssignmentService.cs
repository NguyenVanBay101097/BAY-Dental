using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
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
    public class SurveyAssignmentService: BaseService<SurveyAssignment>, ISurveyAssignmentService
    {
        readonly public IMapper _mapper;
        public SurveyAssignmentService(IAsyncRepository<SurveyAssignment> repository, IHttpContextAccessor httpContextAccessor,
             IMapper mapper
            ) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<SurveyAssignmentDefaultGet>> DefaultGetList()
        {
            var saleOrderObj = GetService<ISaleOrderService>();

            var res = await saleOrderObj.SearchQuery(x => x.State == "done" && !x.Assignments.Any()).Include(x=> x.Partner)
                .Select(x =>new SurveyAssignmentDefaultGet() { 
                DateOrder = x.DateOrder,
                PartnerName = x.Partner.Name,
                PartnerPhone = x.Partner.Phone,
                PartnerRef = x.Partner.Ref,
                SaleOrderId = x.Id,
                SaleOrderName = x.Name
                }).ToListAsync();

            return res;
        }

        public async Task<PagedResult2<SurveyAssignmentBasic>> GetPagedResultAsync(SurveyAssignmentPaged val)
        {
            var query = getAllQuery(val);
            var count = await query.CountAsync();
            if (val.IsGetScore.HasValue && val.IsGetScore == true)
            {
                query = query.Include(x=> x.UserInput);
            }
            query = query.Include(x => x.employee).Include(x => x.SaleOrder).ThenInclude(x => x.Partner).ThenInclude(x => x.PartnerPartnerCategoryRels).ThenInclude(x => x.Category);
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();
            return new PagedResult2<SurveyAssignmentBasic>(count, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<SurveyAssignmentBasic>>(items)
            };
        }

        private IQueryable<SurveyAssignment> getAllQuery(SurveyAssignmentPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.SaleOrder.Partner.Name.Contains(val.Search) || x.employee.Name.Contains(val.Search)
                || x.SaleOrder.Name.Contains(val.Search)
                );
            }
            if (!string.IsNullOrEmpty(val.Status))
            {
                query = query.Where(x => x.Status == val.Status);
            }
            if (val.dateFrom.HasValue)
            {
                query = query.Where(x => x.SaleOrder.LastUpdated >= val.dateFrom.Value);
            }
            if (val.dateTo.HasValue)
            {
                query = query.Where(x => x.SaleOrder.LastUpdated <= val.dateTo.Value);
            }
            return query;
        }

        public async Task<int> GetSummary(SurveyAssignmentPaged val)
        {
            var query = getAllQuery(val);
            return await query.CountAsync();
        }
    }
}
