using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
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

namespace Infrastructure.Services
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeCategoryService _employeeCategoryService;
        public EmployeeService(IAsyncRepository<Employee> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, IEmployeeCategoryService employeeCategoryService)
        : base(repository, httpContextAccessor)
        {
            _employeeCategoryService = employeeCategoryService;
            _mapper = mapper;
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

            if(val.Active.HasValue)
            {
                query = query.Where(x => x.Active == val.Active.Value);
            }

            query = query.OrderBy(s => s.Name);
            return query;
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

        public async Task<PagedResult2<EmployeeBasic>> GetPagedResultAsync(EmployeePaged val)
        {
            var query = GetQueryPaged(val);
            var totalItems = await query.CountAsync();

            query = query.Include(x => x.Category).OrderByDescending(x => x.DateCreated);
            if (val.Limit > 0 )
            {
                query = query.Skip(val.Offset).Take(val.Limit);
            }
            var items = await query
                .ToListAsync();

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
            var items = await query.Where(x=>x.Active == true).Skip(val.Offset).Take(val.Limit)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EmployeeSimple>>(items);
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

            return await base.CreateAsync(entity);
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
            var category = await _employeeCategoryService.SearchQuery(x => x.Id == entity.CategoryId).FirstOrDefaultAsync();
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
            var entity = SearchQuery(x => x.Id == id).FirstOrDefault();
            if (entity == null) throw new Exception("Không tìm thấy nhân viên!");
            entity.Active = val.Active;
            await UpdateAsync(entity);
            return true;
        }
    }
}
