using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaasKit.Multitenancy;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : BaseApiController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IHrPayrollStructureTypeService _structureTypeService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IPartnerService _partnerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationRoleFunctionService _roleFunctionService;
        private readonly IIRModelDataService _iRModelDataService;
        private readonly IResGroupService _resGroupService;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMyCache _cache;
        private readonly AppTenant _tenant;
        private readonly IApplicationRoleService _appRoleService;

        public EmployeesController(IEmployeeService employeeService, IHrPayrollStructureTypeService structureTypeService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork, IPartnerService partnerService,
            UserManager<ApplicationUser> userManager, IApplicationRoleFunctionService roleFunctionService,
            IIRModelDataService iRModelDataService, IResGroupService resGroupService, RoleManager<ApplicationRole> roleManager,
            IMyCache cache, ITenant<AppTenant> tenant,
            IApplicationRoleService appRoleService)
        {
            _employeeService = employeeService;
            _structureTypeService = structureTypeService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _partnerService = partnerService;
            _userManager = userManager;
            _roleFunctionService = roleFunctionService;
            _iRModelDataService = iRModelDataService;
            _resGroupService = resGroupService;
            _roleManager = roleManager;
            _cache = cache;
            _tenant = tenant?.Value;
            _appRoleService = appRoleService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Catalog.Employee.Read")]
        public async Task<IActionResult> Get([FromQuery] EmployeePaged val)
        {
            var result = await _employeeService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Catalog.Employee.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var employee = await _employeeService.SearchQuery(x => x.Id == id)
                .Include(x => x.Commission)
                .Include(x => x.AssistantCommission)
                .Include(x => x.CounselorCommission)
                .Include(x => x.User).ThenInclude(x => x.Partner)
                .Include(x => x.User).ThenInclude(x => x.Company)
                .Include(x => x.User).ThenInclude(x => x.ResCompanyUsersRels).ThenInclude(x => x.Company)
                .Include(x => x.Group)
                .Include(x => x.HrJob)
                .FirstOrDefaultAsync();
            if (employee == null)
                return NotFound();
            var res = _mapper.Map<EmployeeDisplay>(employee);
            //get role
            if (employee.User != null)
            {
                var roleNames = await _userManager.GetRolesAsync(employee.User);
                res.Roles = await _roleManager.Roles.Where(x => roleNames.Contains(x.Name) && x.Hidden == false)
                    .Select(x => new ApplicationRoleBasic() { Id = x.Id, Name = x.Name }).ToListAsync();
            }
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Catalog.Employee.Create")]
        public async Task<IActionResult> Create(EmployeeSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();

            var employee = _mapper.Map<Employee>(val);
            if (!employee.CompanyId.HasValue)
                employee.CompanyId = CompanyId;
            employee.GroupId = employee.IsAllowSurvey ? employee.GroupId : null;

            if (val.IsUser)
            {
                var user = await _CreateEmployeeUser(val);
                employee.UserId = user.Id;
            }

            await UpdateSalary(val, employee);

            await UpdateCommission(val, employee);

            await _employeeService.CreateAsync(employee);
            _unitOfWork.Commit();

            var basic = _mapper.Map<EmployeeBasic>(employee);
            return Ok(basic);
        }

        private async Task UpdateRole(ApplicationUser user, EmployeeSave val)
        {
            var currentRoleNames = await _userManager.GetRolesAsync(user);

            //remove all role
            var result = await _userManager.RemoveFromRolesAsync(user, currentRoleNames);
            if (!result.Succeeded)
                throw new Exception(string.Join(";", result.Errors.Select(x => x.Description)));

            //add role
            var idsAdd = val.RoleIds.Select(x => x.ToString()).ToList();
            var roleNamesAdd = await this._roleManager.Roles.Where(x => idsAdd.Contains(x.Id)).Include(x => x.Functions).Select(x => x.Name).ToListAsync();
            var resultAdd = await _userManager.AddToRolesAsync(user, roleNamesAdd);

            if (!result.Succeeded)
                throw new Exception(string.Join(";", result.Errors.Select(x => x.Description)));

            //clear cache
            _cache.RemoveByPattern($"{(_tenant != null ? _tenant.Hostname : "localhost")}-permissions-{user.Id}");
        }

        private async Task AddGroupUser(ApplicationUser user)
        {
            //add base.group_user cho user và các group đang kế thừa base.group_user
            var groupInternalUser = await _iRModelDataService.GetRef<ResGroup>("base.group_user");
            var to_add = new List<Guid>();
            if (groupInternalUser != null)
            {
                to_add.Add(groupInternalUser.Id);
                var add_dict = _resGroupService._GetTransImplied(new List<Guid>() { groupInternalUser.Id });
                to_add = to_add.Union(add_dict[groupInternalUser.Id].Select(x => x.Id)).ToList();
            }

            foreach (var group_id in to_add)
                user.ResGroupsUsersRels.Add(new ResGroupsUsersRel { GroupId = group_id });
        }

        private async Task UpdatePartnerToEmployee(Employee employee)
        {
            if (employee.Partner == null)
                return;
            var pn = employee.Partner;
            pn.Name = employee.Name;
            pn.Phone = employee.Phone;
            pn.Email = employee.Email;
            await _partnerService.UpdateAsync(pn);
        }

        private async Task UpdateSalary(EmployeeSave val, Employee emp)
        {
            var accessResult = await _roleFunctionService.HasAccess(new string[] { "Catalog.Employee.Salary.Update" });
            if (accessResult.Access)
            {
                emp.Wage = val.Wage;
                emp.HourlyWage = val.HourlyWage;
                emp.LeavePerMonth = val.LeavePerMonth;
                emp.RegularHour = val.RegularHour;
                emp.OvertimeRate = val.OvertimeRate;
                emp.RestDayRate = val.RestDayRate;
                emp.Allowance = val.Allowance;
            }
        }

        private async Task UpdateCommission(EmployeeSave val, Employee emp)
        {
            var accessResult = await _roleFunctionService.HasAccess(new string[] { "Catalog.Employee.Commission.Update" });
            if (accessResult.Access)
            {
                emp.CommissionId = val.CommissionId.HasValue ? val.CommissionId : null;
                emp.CounselorCommissionId = val.CounselorCommissionId.HasValue ? val.CounselorCommissionId : null;
                emp.AssistantCommissionId = val.AssistantCommissionId.HasValue ? val.AssistantCommissionId : null;

            }
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Catalog.Employee.Update")]
        public async Task<IActionResult> Update(Guid id, EmployeeSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var employee = await _employeeService.SearchQuery(x => x.Id == id)
                .Include(x => x.Commission)
                .Include(x => x.AssistantCommission)
                .Include(x => x.CounselorCommission)
                .Include(x => x.Partner)
                .FirstOrDefaultAsync();

            if (employee == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();

            var oldSurveyGroupId = employee.GroupId;
            employee = _mapper.Map(val, employee);
            employee.GroupId = employee.IsAllowSurvey ? employee.GroupId : null;

            await UpdateSalary(val, employee);

            await UpdateCommission(val, employee);

            await UpdatePartnerToEmployee(employee);

            ApplicationUser user = null;
            if (!string.IsNullOrEmpty(employee.UserId))
            {
                user = await _userManager.Users.Where(x => x.Id == employee.UserId)
                  .Include(x => x.Partner)
                  .Include(x => x.ResCompanyUsersRels)
                  .Include(x => x.ResGroupsUsersRels)
                  .FirstOrDefaultAsync();
            }

            if (val.IsUser)
            {
                //ko có user thì tạo
                if (user == null)
                {
                    user = await _CreateEmployeeUser(val);
                    employee.UserId = user.Id;
                }
                else
                {
                    var userPartner = user.Partner;
                    userPartner.Name = val.Name;
                    userPartner.Email = val.Email;
                    userPartner.Phone = val.Phone;
                    await _partnerService.UpdateAsync(userPartner);

                    user.Name = val.Name;
                    user.UserName = val.UserName;
                    user.Email = val.Email;
                    user.PhoneNumber = val.Phone;
                    user.CompanyId = val.UserCompanyId.Value;

                    if (val.IsAllowSurvey && val.GroupId.HasValue && val.GroupId.Value != oldSurveyGroupId)
                    {
                        if (oldSurveyGroupId.HasValue)
                        {
                            var oldSurveyGroup = user.ResGroupsUsersRels.Where(x => x.GroupId == oldSurveyGroupId).FirstOrDefault();
                            user.ResGroupsUsersRels.Remove(oldSurveyGroup);
                        }

                        if (!user.ResGroupsUsersRels.Any(x => x.GroupId == val.GroupId))
                            user.ResGroupsUsersRels.Add(new ResGroupsUsersRel { GroupId = val.GroupId.Value });
                        //clear rule cache
                        _cache.RemoveByPattern($"{(_tenant != null ? _tenant.Hostname : "localhost")}-ir.rule-{user.Id}");
                    }
                    else if (!val.IsAllowSurvey && oldSurveyGroupId.HasValue)
                    {
                        var oldSurveyGroup = user.ResGroupsUsersRels.Where(x => x.GroupId == oldSurveyGroupId).FirstOrDefault();
                        user.ResGroupsUsersRels.Remove(oldSurveyGroup);
                        //clear rule cache
                        _cache.RemoveByPattern($"{(_tenant != null ? _tenant.Hostname : "localhost")}-ir.rule-{user.Id}");
                    }

                    var allowedCompanyIds = val.UserCompanyIds.Union(new List<Guid>() { val.UserCompanyId.Value });
                    user.ResCompanyUsersRels.Clear();
                    foreach (var companyId in allowedCompanyIds)
                        user.ResCompanyUsersRels.Add(new ResCompanyUsersRel { CompanyId = companyId });

                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        if (updateResult.Errors.Any(x => x.Code == "DuplicateUserName"))
                            throw new Exception($"Tài khoản {val.UserName} đã được sử dụng");
                        else
                            throw new Exception($"Cập nhật người dùng không thành công");
                    }

                    if (val.CreateChangePassword && !string.IsNullOrEmpty(val.UserPassword))
                    {
                        if (await _userManager.HasPasswordAsync(user))
                        {
                            await _userManager.RemovePasswordAsync(user);
                            await _userManager.AddPasswordAsync(user, val.UserPassword);
                        }
                        else
                        {
                            await _userManager.AddPasswordAsync(user, val.UserPassword);
                        }
                    }

                    await UpdateRole(user, val);
                }
            }
            else
            {
                if (user != null)
                {
                    var userPartner = user.Partner;
                    userPartner.Name = employee.Name;
                    userPartner.Email = employee.Email;
                    userPartner.Phone = employee.Phone;
                    await _partnerService.UpdateAsync(userPartner);

                    user.Name = employee.Name;
                    user.Email = employee.Email;
                    user.PhoneNumber = employee.Phone;
                    user.CompanyId = val.UserCompanyId.Value;
                    user.Active = false;
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                        throw new Exception($"Cập nhật người dùng không thành công");
                }
            }

            await _employeeService.UpdateAsync(employee);

            _unitOfWork.Commit();

            return NoContent();
        }

        private async Task<ApplicationUser> _CreateEmployeeUser(EmployeeSave val)
        {
            var userPartner = new Partner()
            {
                Name = val.Name,
                Email = val.Email,
                CompanyId = CompanyId,
                Phone = val.Phone,
                Customer = false,
            };

            await _partnerService.CreateAsync(userPartner);

            if (!val.UserCompanyId.HasValue)
                throw new Exception($"Chi nhánh hiện tại không được trống");

            var user = new ApplicationUser()
            {
                Name = val.Name,
                Email = val.Email,
                PhoneNumber = val.Phone,
                UserName = val.UserName,
                CompanyId = val.UserCompanyId.Value,
                PartnerId = userPartner.Id,
            };

            await AddGroupUser(user);

            var allowedCompanyIds = val.UserCompanyIds.Union(new List<Guid>() { val.UserCompanyId.Value });
            foreach (var companyId in allowedCompanyIds)
                user.ResCompanyUsersRels.Add(new ResCompanyUsersRel { CompanyId = companyId });

            if (val.IsAllowSurvey && val.GroupId.HasValue)
                user.ResGroupsUsersRels.Add(new ResGroupsUsersRel { GroupId = val.GroupId.Value });

            var result = await _userManager.CreateAsync(user, val.UserPassword);

            if (!result.Succeeded)
            {
                if (result.Errors.Any(x => x.Code == "DuplicateUserName"))
                    throw new Exception($"Tài khoản {val.UserName} đã được sử dụng");
                else
                    throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));
            }

            await UpdateRole(user, val);

            return user;
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Catalog.Employee.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            //var employee = await _employeeService.GetByIdAsync(id);
            var listPartnerDelete = new List<Partner>();
            await _unitOfWork.BeginTransactionAsync();
            var employee = await _employeeService.SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .Include(x => x.User).ThenInclude(x => x.Partner)
                .FirstOrDefaultAsync();

            if (employee == null)
                return NotFound();

            if (employee.Partner != null)
                listPartnerDelete.Add(employee.Partner);

            if (employee.User != null)
            {
                if (employee.User.Partner != null)
                    listPartnerDelete.Add(employee.User.Partner);
                await _userManager.DeleteAsync(employee.User);
            }

            await _employeeService.DeleteAsync(employee);

            if (listPartnerDelete.Any())
                await _partnerService.DeleteAsync(listPartnerDelete);

            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("Autocomplete")]
        [CheckAccess(Actions = "Catalog.Employee.Read")]
        public async Task<IActionResult> Autocomplete(EmployeePaged val)
        {
            var result = await _employeeService.GetAutocompleteAsync(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Catalog.Employee.Read")]
        public async Task<IActionResult> AutocompleteInfos(EmployeePaged val)
        {
            var emps = await _employeeService.GetAutocomplete(val);
            var res = _mapper.Map<IEnumerable<EmployeeSimpleInfo>>(emps);
            return Ok(res);
        }

        //Lấy danh sách nhân viên có thể thực hiện khảo sát
        [HttpGet("[action]")]
        [CheckAccess(Actions = "Catalog.Employee.Read")]
        public async Task<IActionResult> AllowSurveyList()
        {
            var result = await _employeeService.GetAllowSurveyList();
            return Ok(result);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Catalog.Employee.Read")]
        public async Task<IActionResult> SearchRead(EmployeePaged val)
        {
            var result = await _employeeService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpPost("[action]/{id}")]
        [CheckAccess(Actions = "Catalog.Employee.Update")]
        public async Task<IActionResult> ActionActive(Guid id, [FromBody] EmployeeActive val)
        {
            var result = await _employeeService.ActionActive(id, val);
            return Ok(result);
        }


        [HttpGet("GetEmployeeSurveyCount")]
        [CheckAccess(Actions = "Catalog.Employee.Read")]
        public async Task<IActionResult> GetEmployeeSurveyCount([FromQuery] EmployeePaged val)
        {
            var result = await _employeeService.GetEmployeeSurveyCount(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateXML()
        {
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\employee.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 25);
            var entities = await _employeeService.SearchQuery(x => x.DateCreated <= dateToData).ToListAsync();//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<EmployeeXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var item = _mapper.Map<EmployeeXmlSampleDataRecord>(entity);
                item.Id = $@"sample.employee_{entities.IndexOf(entity) + 1}";
                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "employee",
                    ResId = entity.Id.ToString(),
                    Name = $"employee_{ entities.IndexOf(entity) + 1}"
                });
                if (entity.PartnerId.HasValue)
                {
                    // add IRModelData
                    irModelCreate.Add(new IRModelData()
                    {
                        Module = "sample",
                        Model = "partner",
                        ResId = entity.PartnerId.ToString(),
                        Name = $"employee_{ entities.IndexOf(entity) + 1}_partner"
                    });
                }

            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
        }
    }
}