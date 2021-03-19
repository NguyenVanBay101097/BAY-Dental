using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmployeeCategoryService _employeeCategoryService;
        private readonly IMyCache _cache;
        private readonly AppTenant _tenant;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public EmployeeService(IAsyncRepository<Employee> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, IEmployeeCategoryService employeeCategoryService, UserManager<ApplicationUser> userManager, IMyCache cache, ITenant<AppTenant> tenant,
            RoleManager<ApplicationRole> roleManager
            )
        : base(repository, httpContextAccessor)
        {
            _employeeCategoryService = employeeCategoryService;
            _mapper = mapper;
            _userManager = userManager;
            _cache = cache;
            _tenant = tenant?.Value;
            _roleManager = roleManager;
        }

        private IQueryable<Employee> GetQueryPaged(EmployeePaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.Ref.Contains(val.Search) || x.Phone.Contains(val.Search));

            string[] positionList = null;
            if (!string.IsNullOrEmpty(val.Position))
            {
                positionList = (val.Position).Split(",");
                query = query.Where(x => positionList.Contains(x.Category.Type));
            }

            if (val.Ids != null)
                query = query.Where(x => val.Ids.Contains(x.Id));

            if (val.IsDoctor.HasValue)
                query = query.Where(x => x.IsDoctor == val.IsDoctor);

            if (val.IsAssistant.HasValue)
                query = query.Where(x => x.IsAssistant == val.IsAssistant);

            if (val.Active.HasValue)
            {
                query = query.Where(x => x.Active == val.Active.Value);
            }

            if (val.IsAllowSurvey.HasValue)
            {
                query = query.Where(x => x.IsAllowSurvey == val.IsAllowSurvey.Value);
            }

            query = query.OrderBy(s => s.Name);
            return query;
        }

        private void CheckConstraints(Employee self)
        {
            if (!string.IsNullOrEmpty(self.UserId))
            {
                if (SearchQuery(x => x.UserId == self.UserId).Count() > 1)
                {
                    throw new Exception($"Tai khoan {self.User.UserName} da duoc lien ket");
                }
            }
        }

        public override ISpecification<Employee> RuleDomainGet(IRRule rule)
        {
            //ra đc list company id ma nguoi dung dc phép
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "hr.employee_comp_rule":
                    return new InitialSpecification<Employee>(x => !x.CompanyId.HasValue || companyIds.Contains(x.CompanyId.Value));
                default:
                    return null;
            }
        }

        public async Task<PagedResult2<EmployeeSurveyDisplay>> GetEmployeeSurveyCount(EmployeePaged val)
        {
            var assignObj = GetService<ISurveyAssignmentService>();

            var query = SearchQuery(x => x.IsAllowSurvey == true && x.Active == true && x.User.Active == true);
            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.Name.Contains(val.Search));
            }

            var totalItem = await query.CountAsync();

            if (val.Limit > 0 )
            {
                query = query.Skip(val.Offset).Take(val.Limit);
            }

            var employees = await query.Select(x => new EmployeeSurveyDisplay()
            {
                CompanyName = x.Company.Name,
                Email = x.Email,
                Id = x.Id,
                Phone = x.Phone,
                Name = x.Name,
                Ref = x.Ref
            }).ToListAsync();
            var employeeIds = employees.Select(z => z.Id).ToList();

            var countQuery = assignObj.SearchQuery(x => employeeIds.Contains(x.EmployeeId));
            if (val.DateFrom.HasValue)
            {
                countQuery = countQuery.Where(x => x.AssignDate.Value >= val.DateFrom);
            }

            if (val.DateTo.HasValue)
            {
                countQuery = countQuery.Where(x => x.AssignDate.Value <= val.DateTo);
            }

            var countDicts = await countQuery.GroupBy(x => x.EmployeeId).Select(x => new
            {
                EmployeeId = x.Key,
                Total = x.Sum(o => 1),
                Done = x.Sum(o => o.Status == "done" ? 1 : 0)
            }).ToDictionaryAsync(x=> x.EmployeeId, x=> new { x.Done, x.Total});

            foreach (var item in employees)
            {
                item.TotalAssignment = countDicts.ContainsKey(item.Id) ? countDicts[item.Id].Total : 0;
                item.DoneAssignment = countDicts.ContainsKey(item.Id) ? countDicts[item.Id].Done : 0;
            }

            return new PagedResult2<EmployeeSurveyDisplay>(totalItem, val.Offset, val.Limit)
            {
                Items = employees
            };
        }

        public async Task<PagedResult2<EmployeeBasic>> GetPagedResultAsync(EmployeePaged val)
        {
            var query = GetQueryPaged(val).Include(x => x.Company).AsQueryable();
            var totalItems = await query.CountAsync();

            query = query.Include(x => x.User).OrderByDescending(x => x.DateCreated);
            if (val.Limit > 0)
            {
                query = query.Skip(val.Offset).Take(val.Limit);
            }

            var items = await query.ToListAsync();

            return new PagedResult2<EmployeeBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<EmployeeBasic>>(items)
            };
        }

        public async Task<IEnumerable<EmployeeSimple>> GetAutocompleteAsync(EmployeePaged val)
        {
            var query = GetQueryPaged(val);
            if (val.Position == "doctor")
            {
                query = query.Where(x => x.IsDoctor == true && x.IsAssistant == false);
            }
            else if (val.Position == "assistant")
            {
                query = query.Where(x => x.IsDoctor == false && x.IsAssistant == true);
            }
            var items = await query.Where(x => x.Active == true).Skip(val.Offset).Take(val.Limit)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EmployeeSimple>>(items);
        }

        public async Task<IEnumerable<EmployeeSimple>> GetAllowSurveyList()
        {
            Sudo = true;
            var items = await SearchQuery(x => x.Active && x.IsAllowSurvey).Select(x => new EmployeeSimple
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();

            return items;
        }

        public override async Task<Employee> CreateAsync(Employee entity)
        {
            if (string.IsNullOrEmpty(entity.Ref) || entity.Ref == "/")
            {
                var sequenceService = (IIRSequenceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IIRSequenceService));
                entity.Ref = await sequenceService.NextByCode("employee");
                if (string.IsNullOrEmpty(entity.Ref))
                {
                    await InsertEmployeeSequence();
                    entity.Ref = await sequenceService.NextByCode("employee");
                }
            }

            var partnerObj = GetService<IPartnerService>();
            var partner = new Partner()
            {
                Name = entity.Name,
                Employee = true,
                Ref = entity.Ref,
                Phone = entity.Phone,
                Email = entity.Email,
                CompanyId = entity.CompanyId,
                Customer = false,
                NameNoSign = StringUtils.RemoveSignVietnameseV2(entity.Name)
            };

            await partnerObj.CreateAsync(partner);
            entity.PartnerId = partner.Id;

            var emp = await base.CreateAsync(entity);
            //CheckConstraints(entity);
            return emp;
        }

        public async Task<Employee> GetByUserIdAsync(string userId)
        {
            return await SearchQuery(x => x.UserId == userId).FirstOrDefaultAsync();
        }

        //private async Task InsertDoctorSequence()
        //{
        //    var seqObj = GetService<IIRSequenceService>();
        //    await seqObj.CreateAsync(new IRSequence
        //    {
        //        Code = "doctor",
        //        Name = "Mã bác sĩ",
        //        Prefix = "BS",
        //        Padding = 5,
        //    });
        //}

        //private async Task InsertAssistantSequence()
        //{
        //    var seqObj = GetService<IIRSequenceService>();
        //    await seqObj.CreateAsync(new IRSequence
        //    {
        //        Code = "assistant",
        //        Name = "Mã phụ tá",
        //        Prefix = "PT",
        //        Padding = 5,
        //    });
        //}

        private async Task InsertEmployeeSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Code = "employee",
                Name = "Mã nhân viên",
                Prefix = "NV",
                Padding = 5,
            });
        }

        public override async Task UpdateAsync(Employee entity)
        {
            //var category = await _employeeCategoryService.SearchQuery(x => x.Id == entity.CategoryId).FirstOrDefaultAsync();
            //if (category.Type == "doctor")
            //{
            //    entity.IsDoctor = true;
            //    entity.IsAssistant = false;
            //}
            //else if (category.Type == "assistant")
            //{
            //    entity.IsAssistant = true;
            //    entity.IsDoctor = false;
            //}
            //else
            //{
            //    entity.IsAssistant = false;
            //    entity.IsDoctor = false;
            //}
            await base.UpdateAsync(entity);
            //CheckConstraints(entity);
        }

        public async Task updateSalary(EmployeeDisplay val, Employee emp)
        {
            var roleObj = GetService<IApplicationRoleFunctionService>();
            var accessResult = await roleObj.HasAccess(new string[] { "Catalog.Employee.Salary.Update" });
            if (!accessResult.Access)
            {
                val.Wage = emp.Wage;
                val.HourlyWage = emp.HourlyWage;
                val.LeavePerMonth = emp.LeavePerMonth;
                val.RegularHour = emp.RegularHour;
                val.OvertimeRate = emp.OvertimeRate;
                val.RestDayRate = emp.RestDayRate;
                val.Allowance = emp.Allowance;
            }
        }

        public async Task<bool> ActionActive(Guid id, EmployeeActive val)
        {
            var partnerObj = GetService<IPartnerService>();
            var listPartnerUpdate = new List<Partner>();

            var entity = SearchQuery(x => x.Id == id).Include(x => x.Partner).Include(x => x.User).ThenInclude(x=> x.Partner).FirstOrDefault();
            if (entity == null) throw new Exception("Không tìm thấy nhân viên!");
            entity.Active = val.Active;

            if(entity.User != null)
            {
                entity.User.Active = val.Active;
                if (entity.User.Partner != null)
                    listPartnerUpdate.Add(entity.User.Partner);
            }

            await UpdateAsync(entity);

            if(entity.Partner != null)
                listPartnerUpdate.Add(entity.Partner);

            if (listPartnerUpdate.Any())
            {
                foreach (var item in listPartnerUpdate)
                    item.Active = val.Active;

                await partnerObj.UpdateAsync(listPartnerUpdate);
            }

            return true;
        }

        public async Task UpdateResgroupForSurvey(Employee empl)
        {
            //update valid employee attribute
            if (empl.IsAllowSurvey == false)
                empl.GroupId = null;
            if (empl.IsAllowSurvey == true && empl.GroupId == null)
                throw new Exception("Phải chọn nhóm nhân viên khảo sát");

            if (empl.UserId == null) 
                return;

            var user = await _userManager.Users.Where(x => x.Id == empl.UserId)
                .Include(x => x.ResGroupsUsersRels)
                .FirstOrDefaultAsync();

            //lấy danh sách groups: nv và quản lý, lấy dựa vào ir module category
            var modelDataService = GetService<IIRModelDataService>();
            var category = await modelDataService.GetRef<IrModuleCategory>("survey.module_category_survey");
            if (category == null)
                return;

            var resGroupService = GetService<IResGroupService>();
            resGroupService.Sudo = true;
            var groupIds = await resGroupService.SearchQuery(x => x.CategoryId == category.Id).OrderBy(x => x.Name).Select(x => x.Id).ToListAsync();
            //remove nv và quản lý
            var rels = user.ResGroupsUsersRels.Where(x => groupIds.Contains(x.GroupId)).ToList();
            foreach (var rel in rels)
            {
                user.ResGroupsUsersRels.Remove(rel);
            }
            //add lại cái nó chọn
            if (empl.GroupId.HasValue && !user.ResGroupsUsersRels.Any(x => x.GroupId == empl.GroupId))
                user.ResGroupsUsersRels.Add(new ResGroupsUsersRel { GroupId = empl.GroupId.Value });
        
            await _userManager.UpdateAsync(user);

            //clear cache
            _cache.RemoveByPattern($"{(_tenant != null ? _tenant.Hostname : "localhost")}-ir.rule-{empl.UserId}");
        }


    }
}
