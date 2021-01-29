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
    public class SurveyAssignmentService : BaseService<SurveyAssignment>, ISurveyAssignmentService
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
                SaleOrderName = x.Name,
                PartnerId = x.PartnerId
                }).ToListAsync();

            return res;
        }

        public async Task<PagedResult2<SurveyAssignmentBasic>> GetPagedResultAsync(SurveyAssignmentPaged val)
        {
            var pnCateRelObj = GetService<IPartnerPartnerCategoryRelService>();

            var query = getAllQuery(val);
            var count = await query.CountAsync();
            if (val.IsGetScore.HasValue && val.IsGetScore == true)
            {
                query = query.Include(x => x.UserInput);
            }
            query = query.Include(x => x.employee).Include(x => x.Partner).Include(x => x.SaleOrder);
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var pnCategories = await pnCateRelObj.SearchQuery(x => items.Select(y => y.PartnerId).Contains(x.PartnerId))
                .Select(x => new { ParnerId = x.PartnerId, CategoryName = x.Category.Name }).ToListAsync();

            var resItems = _mapper.Map<IEnumerable<SurveyAssignmentBasic>>(items);
            foreach (var item in resItems)
            {
                item.PartnerCategoriesDisplay = string.Join(", ", pnCategories.Where(x => x.ParnerId == item.PartnerId).Select(x => x.CategoryName));
            }

            return new PagedResult2<SurveyAssignmentBasic>(count, val.Offset, val.Limit)
            {
                Items = resItems
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
            if (val.EmployeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == val.EmployeeId.Value);
            }
            return query;
        }

        public async Task<SurveyAssignmentDisplay> GetDisplay(Guid id)
        {
            var saleOrderObj = GetService<ISaleOrderService>();
            var saleOrderLineObj = GetService<ISaleOrderLineService>();
            var surveyCallContentObj = GetService<ISurveyCallContentService>();
            var assign = await SearchQuery(x => x.Id == id).Include(x => x.CallContents).Include(x => x.UserInput).ThenInclude(s => s.Lines).Include(x => x.SaleOrder).FirstOrDefaultAsync();
            var assignDisplay = _mapper.Map<SurveyAssignmentDisplay>(assign);
            assignDisplay.SaleOrder = _mapper.Map<SaleOrderDisplayVm>(await saleOrderObj.SearchQuery(x => x.Id == assignDisplay.SaleOrderId).FirstOrDefaultAsync());
            assignDisplay.SaleOrder.OrderLines = await saleOrderLineObj.GetDisplayBySaleOrder(assignDisplay.SaleOrderId);
            assignDisplay.SaleOrder.DotKhams = await saleOrderObj._GetListDotkhamInfo(assignDisplay.SaleOrderId);
            assignDisplay.CallContents = _mapper.Map<IEnumerable<SurveyCallContentDisplay>>(await surveyCallContentObj.SearchQuery(x => x.AssignmentId == assignDisplay.Id).OrderByDescending(x => x.Date).ToListAsync());
            return assignDisplay;
        }

        /// <summary>
        /// xử lý nút liên hệ
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task ActionContact(IEnumerable<Guid> ids)
        {
            var assigns = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.CallContents)
                .Include(x => x.UserInput)
                .ThenInclude(s => s.Lines)
                .Include(x => x.SaleOrder)
                .ToListAsync();

            foreach (var assign in assigns)
            {
                assign.Status = "contact";
            }

            await UpdateAsync(assigns);
        }

        /// <summary>
        /// xu ly hoan thanh khao sat
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task ActionDone(AssignmentActionDone val)
        {
            var userInputObj = GetService<ISurveyUserInputService>();
            var assign = await SearchQuery(x => x.Id == val.Id).FirstOrDefaultAsync();
            var now = DateTime.Now;
            //xu ly tao userinput
            var userInput = await userInputObj.CreateUserInput(val.SurveyUserInput);

            assign.UserInputId = userInput.Id;
            assign.Status = "done";
            assign.CompleteDate = now;

            await UpdateAsync(assign);
        }

        /// <summary>
        /// xử lý nút hủy khảo sát
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var userInputObj = GetService<ISurveyUserInputService>();
            var assigns = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.UserInput).ToListAsync();

            foreach (var assign in assigns)
            {
                if (assign.Status != "done")
                    throw new Exception("Phiếu khảo sát chưa hoàn thành");

                //xóa userinput
                await userInputObj.Unlink(assign.Id);

                assign.Status = "draft";
                assign.CompleteDate = null;
            }

            await UpdateAsync(assigns);
        }

        public async Task<int> GetSummary(SurveyAssignmentPaged val)
        {
            var query = getAllQuery(val);
            return await query.CountAsync();
        }
    }
}
