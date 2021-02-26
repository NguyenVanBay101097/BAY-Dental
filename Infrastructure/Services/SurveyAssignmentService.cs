using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Utilities;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class SurveyAssignmentService : BaseService<SurveyAssignment>, ISurveyAssignmentService
    {
        readonly public IMapper _mapper;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public SurveyAssignmentService(IAsyncRepository<SurveyAssignment> repository, IHttpContextAccessor httpContextAccessor,
            RoleManager<ApplicationRole> roleManager,
             IMapper mapper
            ) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<PagedResult2<SurveyAssignmentDefaultGet>> DefaultGetList(SurveyAssignmentDefaultGetPar val)
        {
            var saleOrderObj = GetService<ISaleOrderService>();
            saleOrderObj.Sudo = true;

            var query = saleOrderObj.SearchQuery(x => x.State == "done" && !x.Assignments.Any() && x.DateDone.HasValue);
            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.Name.Contains(val.Search) || x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search));
            }

            if (val.dateFrom.HasValue)
            {
                query = query.Where(x => x.DateDone >= val.dateFrom || x.DateDone == null);
            }

            if (val.dateTo.HasValue)
            {
                query = query.Where(x => x.DateDone <= val.dateTo || x.DateDone == null);
            }

            var totalItem = await query.CountAsync();

            var res = await query.Include(x => x.Partner).Skip(val.Offset).Take(val.Limit).Select(x => new SurveyAssignmentDefaultGet()
            {
                DateOrder = x.DateOrder,
                PartnerName = x.Partner.Name,
                PartnerPhone = x.Partner.Phone,
                PartnerRef = x.Partner.Ref,
                SaleOrderId = x.Id,
                SaleOrderName = x.Name,
                PartnerId = x.PartnerId,
                SaleOrderDateCreated = x.DateCreated,
                SaleOrderDateDone = x.DateDone
            }).ToListAsync();

            return new PagedResult2<SurveyAssignmentDefaultGet>(totalItem, val.Offset, val.Limit)
            {
                Items = res
            };
        }

        public async Task<PagedResult2<SurveyAssignmentBasic>> GetPagedResultAsync(SurveyAssignmentPaged val)
        {
            var pnCateRelObj = GetService<IPartnerPartnerCategoryRelService>();

            var query = getAllQuery(val);
            if (val.IsGetScore.HasValue && val.IsGetScore == true)
            {
                query = query.Include(x => x.UserInput);
            }
            query = query.Include(x => x.Employee).Include(x => x.Partner).Include(x => x.SaleOrder);
            var count = await query.CountAsync();
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
                query = query.Where(x => x.SaleOrder.Partner.Name.Contains(val.Search) || x.Employee.Name.Contains(val.Search)
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

            if (val.UserId.HasValue)
            {
                query = query.Where(x => x.Employee.UserId == val.UserId.ToString());
            }

            query = query.OrderByDescending(x => x.SaleOrder.DateDone ?? x.SaleOrder.DateDone.Value);

            return query;
        }

        public async Task<SurveyAssignmentDisplay> GetDisplay(Guid id)
        {
            var saleOrderObj = GetService<ISaleOrderService>();
            saleOrderObj.Sudo = true;
            var saleOrderLineObj = GetService<ISaleOrderLineService>();
            saleOrderLineObj.Sudo = true;
            var partnerObj = GetService<IPartnerService>();
            partnerObj.Sudo = true;

            var assign = await SearchQuery(x => x.Id == id).Include(x => x.UserInput).ThenInclude(s => s.Lines).Include(x => x.SaleOrder).FirstOrDefaultAsync();
            if (assign == null) throw new Exception("Không tìm thấy khảo sát!");

            var assignDisplay = _mapper.Map<SurveyAssignmentDisplay>(assign);
            assignDisplay.SaleOrder = _mapper.Map<SaleOrderDisplayVm>(await saleOrderObj.SearchQuery(x => x.Id == assignDisplay.SaleOrderId).FirstOrDefaultAsync());
            assignDisplay.SaleOrder.Partner = await partnerObj.GetInfoPartner(assignDisplay.SaleOrder.PartnerId);
            assignDisplay.SaleOrder.OrderLines = await saleOrderLineObj.GetDisplayBySaleOrder(assignDisplay.SaleOrderId);
            assignDisplay.SaleOrder.DotKhams = await saleOrderObj._GetListDotkhamInfo(assignDisplay.SaleOrderId);
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

            if (assign == null) throw new Exception("Không tìm thấy Khảo sát!");
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
                if (!assign.UserInputId.HasValue)
                    throw new Exception("Không tìm thấy phiếu khảo sát ");

                await userInputObj.Unlink(assign.UserInputId.Value);

                assign.Status = "contact";
                assign.CompleteDate = null;
            }

            await UpdateAsync(assigns);
        }

        public async Task<int> GetSummary(SurveyAssignmentPaged val)
        {
            var query = getAllQuery(val);
            return await query.CountAsync();
        }

        public async Task<long> GetCount(SurveyAssignmentGetCountVM val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Status))
                query = query.Where(x => x.Status == val.Status);

            if (val.DateFrom.HasValue)
            {
                var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.SaleOrder.LastUpdated >= val.DateFrom);
            }

            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.SaleOrder.LastUpdated <= dateTo);
            }

            if (val.EmployeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == val.EmployeeId);
            }

            if (val.UserId.HasValue)
            {
                query = query.Where(x => x.Employee.UserId == val.UserId.ToString());
            }

            return await query.LongCountAsync();
        }

        public override ISpecification<SurveyAssignment> RuleDomainGet(IRRule rule)
        {
            switch (rule.Code)
            {
                case "survey.assignment_employee": // group cho việc : nếu là nhân viên thuộc group nhân viên viên thì get theo nhân viên
                    var employeeObj = GetService<IEmployeeService>();
                    var employeeId = employeeObj.SearchQuery(x => x.UserId == UserId).Select(x => x.Id).FirstOrDefault();
                    if (employeeId == null) return null;
                    return new InitialSpecification<SurveyAssignment>(x => x.EmployeeId == employeeId);
                default:
                    return null;
            }
        }

        public async Task AddIrDataForSurvey()
        {
            //tao cate
            var cateObj = GetService<IIrModuleCategoryService>();
            var cateSequenceMax = await cateObj.SearchQuery().MaxAsync(x => x.Sequence);
            var cate = await cateObj.CreateAsync(new IrModuleCategory()
            {
                Name = "Survey",
                Sequence = cateSequenceMax == null ? 1 : cateSequenceMax + 1,
                Visible = false
            });
            //tao resgroup include
            var groupObj = GetService<IResGroupService>();
            var empGroup = new ResGroup()
            {
                CategoryId = cate.Id,
                Name = "Nhân viên khảo sát",
            };
            var manageGroup = new ResGroup()
            {

                CategoryId = cate.Id,
                Name = "Quản lý khảo sát",
            };
            var groups = await groupObj.CreateAsync(new List<ResGroup>() {
            manageGroup,
            empGroup
             });
            //tao model data quản lý 2 group
            var modelDataObj = GetService<IIRModelDataService>();
            var modelData = await modelDataObj.CreateAsync(new List<IRModelData>() {
            new IRModelData() // for show combobox
            {
                Name = "survey_assignment",
                Module = "survey",
                ResId = cate.Id.ToString(),
                Model = "ir.module.category"
            },
            //for ẩn hiện menu
             new IRModelData()
            {
                Name = "survey_assignment_Quanly",
                Module = "survey_Quanly",
                ResId = manageGroup.Id.ToString(),
                Model = "res.groups"
            },
              new IRModelData()
            {
                Name = "survey_assignment_Nhanvien",
                Module = "survey_Nhanvien",
                ResId = empGroup.Id.ToString(),
                Model = "res.groups"
            }
            });

            // tao rule cho việc get assignment by employee
            var ruleObj = GetService<IIRRuleService>();
            var modelObj = GetService<IIRModelService>();
            var model = await modelObj.CreateAsync(new IRModel()
            {
                Name = "Survey Assignment",
                Model = "SurveyAssignment",
                Transient = true,
            });
            var rule = new IRRule()
            {
                Name = "survey assignment by employee",
                Global = true,
                Active = true,
                PermUnlink = true,
                PermWrite = true,
                PermRead = true,
                PermCreate = true,
                ModelId = model.Id,
                Code = "survey.assignment_employee",
            };

            foreach (var group in groups)
            {
                if (group.Id == empGroup.Id)
                    rule.RuleGroupRels.Add(new RuleGroupRel() { GroupId = group.Id });
            }
            await ruleObj.CreateAsync(rule);
        }

        
    }
}
