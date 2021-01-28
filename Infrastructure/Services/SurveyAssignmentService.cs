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
                query = query.Where(x => x.CompleteDate >= val.dateFrom.Value);
            }
            if (val.dateTo.HasValue)
            {
                query = query.Where(x => x.CompleteDate <= val.dateTo.Value);
            }
            var count = await query.CountAsync();
            query = query.Include(x => x.employee).Include(x => x.SaleOrder).ThenInclude(x => x.Partner).ThenInclude(x => x.PartnerPartnerCategoryRels).ThenInclude(x => x.Category);
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();
            return new PagedResult2<SurveyAssignmentBasic>(count, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<SurveyAssignmentBasic>>(items)
            };
        }

        private IQueryable getQuery()
        {
            return SearchQuery();
        }
        public async Task<SurveyAssignmentDisplay> GetDisplay(Guid id)
        {
            var saleorderObj = GetService<ISaleOrderService>();
            var assign = await SearchQuery(x => x.Id == id).Include(x=>x.CallContents).Include(x=>x.SaleOrder).FirstOrDefaultAsync();
            var assignDisplay = _mapper.Map<SurveyAssignmentDisplay>(assign);
            assignDisplay.SaleOrder = await saleorderObj.GetDisplayAsync(id);

            return assignDisplay;
        }

        /// <summary>
        /// xử lý nút liên hệ
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task ActionContact(IEnumerable<Guid> ids)
        {

        }

        /// <summary>
        /// xử lý nút khảo sát
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task ActionSurvey(IEnumerable<Guid> ids)
        {

        }

        /// <summary>
        /// xử lý nút hủy khảo sát
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task ActionCancel(IEnumerable<Guid> ids)
        {

        }

        public Task<SurveyAssignmentSummary> GetSummary(SurveyAssignmentPaged val)
        {
            throw new NotImplementedException();
        }
    }
}
