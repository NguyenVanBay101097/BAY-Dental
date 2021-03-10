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
                query = query.Where(x => x.DateDone >= val.dateFrom);
            }

            if (val.dateTo.HasValue)
            {
                var dateTo = val.dateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.DateDone <= dateTo);
            }

            var totalItem = await query.CountAsync();

            var res = await query.Skip(val.Offset).Take(val.Limit).Select(x => new SurveyAssignmentDefaultGet()
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

        public async Task<PagedResult2<SurveyAssignmentGridItem>> GetPagedResultAsync(SurveyAssignmentPaged val)
        {
            var pnCateRelObj = GetService<IPartnerPartnerCategoryRelService>();

            var query = GetAllQuery(val);
            query = query.Include(x => x.UserInput).Include(x => x.Employee).Include(x => x.Partner).Include(x => x.SaleOrder);
            var count = await query.CountAsync();
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var pnCategories = await pnCateRelObj.SearchQuery(x => items.Select(y => y.PartnerId).Contains(x.PartnerId))
                .Select(x => new { ParnerId = x.PartnerId, CategoryName = x.Category.Name }).ToListAsync();

            var resItems = _mapper.Map<IEnumerable<SurveyAssignmentGridItem>>(items);
            foreach (var item in resItems)
            {
                item.PartnerCategoriesDisplay = string.Join(", ", pnCategories.Where(x => x.ParnerId == item.PartnerId).Select(x => x.CategoryName));
            }

            return new PagedResult2<SurveyAssignmentGridItem>(count, val.Offset, val.Limit)
            {
                Items = resItems
            };
        }

        private IQueryable<SurveyAssignment> GetAllQuery(SurveyAssignmentPaged val)
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
            if (val.DateFrom.HasValue)
            {
                query = query.Where(x => x.AssignDate >= val.DateFrom.Value);
            }
            if (val.DateTo.HasValue)
            {
                query = query.Where(x => x.AssignDate <= val.DateTo.Value);
            }
            if (val.EmployeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == val.EmployeeId.Value);
            }

            if (val.UserId.HasValue)
            {
                query = query.Where(x => x.Employee.UserId == val.UserId.ToString());
            }

            query = query.OrderByDescending(x => x.DateCreated);

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
            var dotkhamObj = GetService<IDotKhamService>();
            dotkhamObj.Sudo = true;
            var dotKhamLineObj = GetService<IDotKhamLineService>();
            dotKhamLineObj.Sudo = true;
            var callContentObj = GetService<ISurveyCallContentService>();
            var userInputObj = GetService<ISurveyUserInputService>();

            var assign = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (assign == null) 
                throw new Exception("Không tìm thấy khảo sát!");

            var assignDisplay = _mapper.Map<SurveyAssignmentDisplay>(assign);

            assignDisplay.Partner = await partnerObj.SearchQuery(x => x.Id == assign.PartnerId)
                .Select(x => new SurveyAssignmentDisplayPartner
                {
                    Avatar = x.Avatar,
                    BirthDay = x.BirthDay,
                    BirthMonth = x.BirthMonth,
                    BirthYear = x.BirthYear,
                    Categories = x.PartnerPartnerCategoryRels.Select(s => s.Category.Name),
                    Gender = x.Gender,
                    Name = x.Name,
                    Ref = x.Ref,
                    Date = x.Date,
                    Phone = x.Phone,
                    Email = x.Email,
                    Histories = x.PartnerHistoryRels.Select(s => s.History.Name),
                    JobTitle = x.JobTitle,
                    Street = x.Street,
                    WardName = x.WardName,
                    DistrictName = x.DistrictName,
                    CityName = x.CityName
                }).FirstOrDefaultAsync();

            assignDisplay.SaleOrder = await saleOrderObj.SearchQuery(x => x.Id == assign.SaleOrderId)
                .Select(x => new SurveyAssignmentDisplaySaleOrder { 
                DateOrder = x.DateOrder,
                Name = x.Name,
                State = x.State,
                AmountTotal = x.AmountTotal
            }).FirstOrDefaultAsync();

            assignDisplay.SaleLines = await saleOrderLineObj.SearchQuery(x => x.OrderId == assign.SaleOrderId)
                .OrderBy(x => x.Sequence)
                .Select(x => new SurveyAssignmentDisplaySaleOrderLine
                {
                    ProductName = x.Product.Name,
                    Diagnostic = x.Diagnostic,
                    EmployeeName = x.Employee.Name,
                    ProductUOMQty = x.ProductUOMQty,
                    Teeth = x.SaleOrderLineToothRels.Select(s => s.Tooth.Name)
                }).ToListAsync();

            assignDisplay.DotKhams = await dotkhamObj.SearchQuery(x => x.SaleOrderId == assign.SaleOrderId)
                .OrderBy(x => x.DateCreated)
                .Select(x => new SurveyAssignmentDisplayDotKham { 
                    Id = x.Id,
                    Date = x.Date,
                    DoctorName = x.Doctor.Name,
                    Reason = x.Reason,
                })
              .ToListAsync();

            foreach(var dotKham in assignDisplay.DotKhams)
            {
                dotKham.Lines = await dotKhamLineObj.SearchQuery(x => x.DotKhamId == dotKham.Id)
                    .OrderBy(x => x.Sequence).Select(x => new SurveyAssignmentDisplayDotKhamLine
                    {
                        Teeth = x.ToothRels.Select(s => s.Tooth.Name),
                        NameStep = x.NameStep,
                        Note = x.Note,
                        ProductName = x.Product.Name
                }).ToListAsync();
            }

            assignDisplay.CallContents = await callContentObj.SearchQuery(x => x.AssignmentId == assign.Id)
                .OrderBy(x => x.DateCreated)
                .Select(x => new SurveyAssignmentDisplayCallContent
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            if (assign.UserInputId.HasValue)
            {
                assignDisplay.UserInput = await userInputObj.SearchQuery(x => x.Id == assign.UserInputId)
                    .Select(x => new SurveyAssignmentDisplayUserInput
                    {
                        Id = x.Id,
                        Score = x.Score,
                        MaxScore = x.MaxScore
                    }).FirstOrDefaultAsync();
            }

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
        //public async Task ActionDone(AssignmentActionDone val)
        //{
        //    var userInputObj = GetService<ISurveyUserInputService>();
        //    var assign = await SearchQuery(x => x.Id == val.Id).FirstOrDefaultAsync();

        //    if (assign == null) throw new Exception("Không tìm thấy Khảo sát!");
        //    var now = DateTime.Now;
        //    //xu ly tao userinput
        //    var userInput = await userInputObj.CreateUserInput(val.SurveyUserInput);

        //    assign.UserInputId = userInput.Id;
        //    assign.Status = "done";
        //    assign.CompleteDate = now;

        //    await UpdateAsync(assign);
        //}

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

        public async Task<IEnumerable<SurveyAssignmentGetSummary>> GetSummary(SurveyAssignmentGetSummaryFilter val)
        {
            var query = SearchQuery();

            if (val.DateFrom.HasValue)
            {
                var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.AssignDate >= val.DateFrom);
            }

            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.AssignDate <= dateTo);
            }

            if (val.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == val.EmployeeId);

            if (!string.IsNullOrEmpty(val.UserId))
                query = query.Where(x => x.UserId == val.UserId);

            var res = await query.GroupBy(x => x.Status).Select(x => new SurveyAssignmentGetSummary
            {
                Status = x.Key,
                Count = x.Count()
            }).ToListAsync();

            return res;
        }

        public override ISpecification<SurveyAssignment> RuleDomainGet(IRRule rule)
        {
            var userId = UserId;
            switch (rule.Code)
            {
                case "survey.assignment_employee": // group cho việc : nếu là nhân viên thuộc group nhân viên viên thì get theo nhân viên
                    return new InitialSpecification<SurveyAssignment>(x => x.UserId == userId);
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
