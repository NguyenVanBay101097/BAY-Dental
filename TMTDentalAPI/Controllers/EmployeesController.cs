using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public EmployeesController(IEmployeeService employeeService, IHrPayrollStructureTypeService structureTypeService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork, IPartnerService partnerService,
            UserManager<ApplicationUser> userManager, IApplicationRoleFunctionService roleFunctionService,
            IIRModelDataService iRModelDataService, IResGroupService resGroupService
            )
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
                .Include(x => x.User).ThenInclude(x => x.Partner)
                .Include(x => x.User).ThenInclude(x => x.Company)
                .Include(x => x.User).ThenInclude(x => x.ResCompanyUsersRels).ThenInclude(x => x.Company)
                .FirstOrDefaultAsync();
            if (employee == null)
                return NotFound();
            return Ok(_mapper.Map<EmployeeDisplay>(employee));
        }

        [HttpPost]
        [CheckAccess(Actions = "Catalog.Employee.Create")]
        public async Task<IActionResult> Create(EmployeeSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();

            var employeePartner = new Partner()
            {
                Name = val.Name,
                Employee = true,
                Ref = val.Ref,
                Phone = val.Phone,
                Email = val.Email,
                Customer = false
            };

            await _partnerService.CreateAsync(employeePartner);

            var employee = _mapper.Map<Employee>(val);
            employee.PartnerId = employeePartner.Id;
            if (!employee.CompanyId.HasValue)
                employee.CompanyId = CompanyId;

            await SaveUser(employee, val);

            await UpdateSalary(val, employee);

            await _employeeService.UpdateResgroupForSurvey(employee);

            await _employeeService.CreateAsync(employee);
            _unitOfWork.Commit();

            var basic = _mapper.Map<EmployeeBasic>(employee);
            return Ok(basic);
        }

        private async Task SaveUser(Employee employee, EmployeeSave val)
        {
            var user = employee.User;
            if (val.IsUser)
            {
                if (user != null)
                {
                    if (user.Partner != null)
                    {
                        var userPartner = user.Partner;
                        userPartner.Name = employee.Name;
                        userPartner.Email = employee.Email;
                        userPartner.Phone = employee.Phone;
                        userPartner.Avatar = val.UserAvatar;
                        await _partnerService.UpdateAsync(userPartner);
                    }

                    user.Name = employee.Name;
                    if (user.UserName != val.UserName)
                    {
                        if (string.IsNullOrEmpty(val.UserName))
                            throw new Exception("Tên đăng nhập không được trống");
                        user.UserName = val.UserName;
                    }

                    user.Active = true;
                    user.Email = employee.Email;
                    user.PhoneNumber = employee.Phone;
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                        throw new Exception($"Cập nhật người dùng không thành công");
                }
                else
                {
                    //check if exist user that usernam == val.Username and this user ever map => map this user
                    var existUser = await _userManager.Users.Where(x => x.UserName == val.UserName && x.CompanyId == employee.CompanyId).Include(x => x.ResCompanyUsersRels).FirstOrDefaultAsync();
                    if (existUser != null)
                    {
                        user = existUser;
                        employee.UserId = existUser.Id;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(val.UserName))
                            throw new Exception("Tên đăng nhập không được trống");
                        if (!val.UserCompanyId.HasValue)
                            throw new Exception("Chi nhánh hiện tại không được trống");

                        var userPartner = new Partner()
                        {
                            Name = employee.Name,
                            Email = employee.Email,
                            CompanyId = employee.CompanyId,
                            Phone = employee.Phone,
                            Customer = false,
                            Avatar = val.UserAvatar
                        };

                        await _partnerService.CreateAsync(userPartner);

                        user = new ApplicationUser()
                        {
                            Name = employee.Name,
                            UserName = val.UserName,
                            CompanyId = val.UserCompanyId.Value,
                            PartnerId = userPartner.Id,
                        };

                        try
                        {
                            var result = await _userManager.CreateAsync(user);

                            if (!result.Succeeded)
                            {
                                if (result.Errors.Any(x => x.Code == "DuplicateUserName"))
                                    throw new Exception($"Tài khoản {val.UserName} đã được sử dụng");
                                else
                                    throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));
                            }

                            await SaveUserResGroup(user);
                        }
                        catch (Exception)
                        {
                            throw new Exception("Tạo tài khoản người dùng không thành công");
                        }

                        employee.UserId = user.Id;
                        employee.User = user;
                    }
                }

                user.ResCompanyUsersRels.Clear();
                foreach (var userCompanyId in val.UserCompanyIds)
                {
                    user.ResCompanyUsersRels.Add(new ResCompanyUsersRel { CompanyId = userCompanyId });
                }

                if (val.CreateChangePassword)
                {
                    if (string.IsNullOrEmpty(val.UserPassword))
                        throw new Exception("Mật khẩu không được trống");

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

                await _userManager.UpdateAsync(user);
            }
            else
            {
                if (user != null)
                {
                    user.Active = false;
                    await _userManager.UpdateAsync(user);
                }
            }
        }

        private async Task SaveUserResGroup( ApplicationUser user)
        {
            //get group internal user to add to user then call function add all group to user
            var groupInternalUser = await _iRModelDataService.GetRef<ResGroup>("base.group_user");
            var to_add = new List<Guid>();
            if (groupInternalUser != null)
                to_add.Add(groupInternalUser.Id);
            var add_dict = _resGroupService._GetTransImplied(to_add);

            foreach (var group_id in to_add)
            {
                var rel2 = user.ResGroupsUsersRels.FirstOrDefault(x => x.GroupId == group_id);
                if (rel2 == null)
                    user.ResGroupsUsersRels.Add(new ResGroupsUsersRel { GroupId = group_id });

                var groups = add_dict[group_id];
                foreach (var group in groups)
                {
                    var rel = user.ResGroupsUsersRels.FirstOrDefault(x => x.GroupId == group.Id);
                    if (rel == null)
                        user.ResGroupsUsersRels.Add(new ResGroupsUsersRel { GroupId = group.Id });
                }
            }
            await _userManager.UpdateAsync(user);
            await _resGroupService.AddAllImpliedGroupsToAllUser(to_add);
        }

        private void UpdatePartnerToEmployee(Employee employee)
        {
            if (employee.Partner == null) return;
            var pn = employee.Partner;
            pn.Name = employee.Name;
            pn.Employee = true;
            pn.Ref = employee.Ref;
            pn.Phone = employee.Phone;
            pn.Email = employee.Email;
            //pn.BirthDay = employee.BirthDay.HasValue ? employee.BirthDay.Value.Day : 1;
            //pn.BirthMonth = employee.BirthDay.HasValue ? employee.BirthDay.Value.Month : 1;
            //pn.BirthYear = employee.BirthDay.HasValue ? employee.BirthDay.Value.Year : DateTime.Now.Year;
            //pn.Barcode = employee.EnrollNumber;
            pn.Supplier = false;
            pn.Customer = false;
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

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Catalog.Employee.Update")]
        public async Task<IActionResult> Update(Guid id, EmployeeSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var employee = await _employeeService.SearchQuery(x => x.Id == id)
                .Include(x => x.Commission)
                .Include(x => x.User.Partner)
                .Include(x => x.User.Company)
                .Include(x => x.User).ThenInclude(x => x.ResCompanyUsersRels).ThenInclude(x => x.Company)
                .FirstOrDefaultAsync();

            if (employee == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();

            employee = _mapper.Map(val, employee);
            await UpdateSalary(val, employee);

            UpdatePartnerToEmployee(employee);

            await _employeeService.UpdateResgroupForSurvey(employee);

            await _employeeService.UpdateAsync(employee);

            await SaveUser(employee, val);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Catalog.Employee.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            //var employee = await _employeeService.GetByIdAsync(id);
            await _unitOfWork.BeginTransactionAsync();
            var employee = await _employeeService.SearchQuery(x => x.Id == id).Include(x => x.User).FirstOrDefaultAsync();

            if (employee == null)
                return NotFound();
            if(employee.User != null )
            {
                await _userManager.DeleteAsync(employee.User);
            }
            await _employeeService.DeleteAsync(employee);
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

        [HttpPost("AutocompleteSudo")]
        [CheckAccess(Actions = "Catalog.Employee.Read")]
        public async Task<IActionResult> AutocompleteSudo(EmployeePaged val)
        {
            var result = await _employeeService.GetAutocompleteSudoAsync(val);
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
    }
}