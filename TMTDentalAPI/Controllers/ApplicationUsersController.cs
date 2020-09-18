﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUsersController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IPartnerService _partnerService;
        private readonly IIRModelAccessService _modelAccessService;
        private readonly IUserService _userService;
        private readonly IUploadService _uploadService;
        private readonly ICompanyService _companyService;
        private readonly IResGroupService _resGroupService;

        public ApplicationUsersController(UserManager<ApplicationUser> userManager,
            IMapper mapper, IUnitOfWorkAsync unitOfWork, IPartnerService partnerService,
            IIRModelAccessService modelAccessService, IUserService userService,
            IUploadService uploadService, ICompanyService companyService, IResGroupService resGroupService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _partnerService = partnerService;
            _modelAccessService = modelAccessService;
            _userService = userService;
            _uploadService = uploadService;
            _companyService = companyService;
            _resGroupService = resGroupService;
        }

        [HttpGet]
        [CheckAccess(Actions = "System.ApplicationUser.Read")]
        public async Task<IActionResult> Get([FromQuery]ApplicationUserPaged val)
        {
            var query = _userManager.Users;
            if (!string.IsNullOrEmpty(val.SearchNameUserName))
                query = query.Where(x => x.Name.Contains(val.SearchNameUserName) || x.UserName.Contains(val.SearchNameUserName));
            var companyId = CompanyId;
            query = query.Where(x => x.CompanyId == companyId);
            query = query.OrderBy(x => x.Name);
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();
            var totalItems = await query.CountAsync();
            var result = new PagedResult2<ApplicationUserBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<ApplicationUserBasic>>(items)
            };
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "System.ApplicationUser.Read")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _userManager.Users.Where(x => x.Id == id).Include(x => x.Company).Include(x => x.ResCompanyUsersRels)
                .Include(x => x.ResGroupsUsersRels).Include("ResGroupsUsersRels.Group")
                .Include("ResCompanyUsersRels.Company").Include(x => x.Partner).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            var res = _mapper.Map<ApplicationUserDisplay>(user);
            res.Avatar = user.Partner.Avatar;
            res.Email = user.Partner.Email;
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "System.ApplicationUser.Create")]
        public async Task<IActionResult> Create(ApplicationUserDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var partner = new Partner()
            {
                Name = val.Name,
                Email = val.Email,
                CompanyId = val.CompanyId,
                Customer = false,

            };

            await SaveAvatar(partner, val);

            await _partnerService.CreateAsync(partner);

            var user = _mapper.Map<ApplicationUser>(val);
            user.PartnerId = partner.Id;
         

            foreach (var company in val.Companies)
            {
                user.ResCompanyUsersRels.Add(new ResCompanyUsersRel { CompanyId = company.Id });
            }

            var to_add = val.Groups.Select(x => x.Id).ToList();
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

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                if (result.Errors.Any(x => x.Code == "DuplicateUserName"))
                    throw new Exception($"Tài khoản {val.UserName} đã được sử dụng");
                else
                    throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));
            }

            if (!string.IsNullOrEmpty(val.Password))
            {
                var addResult = await _userManager.AddPasswordAsync(user, val.Password);
                if (!addResult.Succeeded)
                    throw new Exception($"Add password fail");
            }


            _unitOfWork.Commit();

            val.Id = user.Id;
            return CreatedAtAction(nameof(Get), new { id = user.Id }, val);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "System.ApplicationUser.Update")]
        public async Task<IActionResult> Update(string id, ApplicationUserDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var user = await _userManager.Users.Where(x => x.Id == id).Include(x => x.Partner)
                .Include(x => x.ResCompanyUsersRels).Include(x => x.ResGroupsUsersRels)
                .Include("ResGroupsUsersRels.Group").FirstOrDefaultAsync();
            if (user == null)
                return NotFound();
            await _unitOfWork.BeginTransactionAsync();

            user = _mapper.Map(val, user);
            user.ResCompanyUsersRels.Clear();
            foreach (var company in val.Companies)
            {
                user.ResCompanyUsersRels.Add(new ResCompanyUsersRel { CompanyId = company.Id });
            }


            var to_remove = new List<Guid>();
            foreach (var rel in user.ResGroupsUsersRels)
            {
                if (!val.Groups.Any(x => x.Id == rel.GroupId) && !rel.Group.CategoryId.HasValue)
                    to_remove.Add(rel.GroupId);
            }

            var to_add = val.Groups.Select(x => x.Id).ToList();

            var remove_dict = _resGroupService._GetTransImplied(to_remove);
            foreach (var group_id in to_remove)
            {
                var rel2 = user.ResGroupsUsersRels.FirstOrDefault(x => x.GroupId == group_id);
                if (rel2 != null)
                    user.ResGroupsUsersRels.Remove(rel2);

                var groups = remove_dict[group_id];
                foreach (var group in groups)
                {
                    var rel = user.ResGroupsUsersRels.FirstOrDefault(x => x.GroupId == group.Id);
                    if (rel != null)
                        user.ResGroupsUsersRels.Remove(rel);
                }
            }

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

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new Exception(string.Join(", ", updateResult.Errors.Select(x => x.Description)));

            if (!string.IsNullOrEmpty(val.Password))
            {
                var removeResult = await _userManager.RemovePasswordAsync(user);
                if (!removeResult.Succeeded)
                    throw new Exception($"Remove password fail");

                var addResult = await _userManager.AddPasswordAsync(user, val.Password);
                if (!addResult.Succeeded)
                    throw new Exception($"Add password fail");
            }

            var partner = user.Partner;
            partner.Name = val.Name;
            partner.Email = val.Email;
            await SaveAvatar(partner, val);
            await _partnerService.UpdateAsync(partner);

            _userService.ClearSecurityCache(new List<string>() { id });

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "System.ApplicationUser.Delete")]
        public async Task<IActionResult> Remove(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            await _userManager.DeleteAsync(user);

            return NoContent();
        }

        [HttpGet("Autocomplete")]
        [CheckAccess(Actions = "System.ApplicationUser.Read")]
        public async Task<IActionResult> Autocomplete(string filter = "")
        {
            var companyId = CompanyId;
            var res = await _userManager.Users.Where(x => (string.IsNullOrEmpty(filter) || x.Name.Contains(filter))
            && x.CompanyId == companyId)
                .Select(x => new ApplicationUserSimple
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();
            return Ok(res);
        }

        [HttpPost("AutocompleteUser")]
        [CheckAccess(Actions = "System.ApplicationUser.Read")]
        public async Task<IActionResult> AutocompleteUser(ApplicationUserPaged user)
        {
            var companyId = CompanyId;
            var res = await _userManager.Users.Where(x => (string.IsNullOrEmpty(user.SearchNameUserName) || x.Name.Contains(user.SearchNameUserName)) &&
            x.CompanyId == companyId)
                .Select(x => new ApplicationUserSimple
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();
            var res2 = _mapper.Map<IEnumerable<ApplicationUserSimple>>(res);
            res2 = res.Skip(user.Offset).Take(user.Limit);
            return Ok(res2);
        }

        [HttpPost("AutocompleteSimple")]
        [CheckAccess(Actions = "System.ApplicationUser.Read")]
        public async Task<IActionResult> AutocompleteSimple(ApplicationUserPaged val)
        {
            var companyId = CompanyId;
            var res = await _userManager.Users.Where(x => (string.IsNullOrEmpty(val.Search) || x.Name.Contains(val.Search) ||
            x.UserName.Contains(val.Search) || x.NormalizedUserName.Contains(val.Search)) && (x.ResCompanyUsersRels.Any(x => x.CompanyId == companyId) || x.CompanyId == companyId))
                .OrderBy(x => x.Name).Skip(val.Offset).Take(val.Limit)
                .Select(x => new ApplicationUserSimple
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();
            return Ok(res);
        }

        [HttpPost("DefaultGet")]
        public async Task<IActionResult> DefaultGet()
        {
            var res = new ApplicationUserDisplay();
            res.CompanyId = CompanyId;
            var company = await _companyService.GetByIdAsync(CompanyId);
            var companyBasic = _mapper.Map<CompanyBasic>(company);
            res.Company = companyBasic;
            res.Companies = new List<CompanyBasic>() { companyBasic };
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetChangeCurrentCompany()
        {
            var userId = UserId;
            var user = await _userManager.Users.Where(x => x.Id == userId).Include(x => x.Company).Include(x => x.ResCompanyUsersRels)
                .Include("ResCompanyUsersRels.Company").FirstOrDefaultAsync();
            if (user == null)
                throw new Exception("không tìm thấy tài khoản trong hệ thống !");
            var res = new UserChangeCurrentCompanyVM
            {
                CurrentCompany = _mapper.Map<CompanyBasic>(user.Company),
                Companies = _mapper.Map<IEnumerable<CompanyBasic>>(user.ResCompanyUsersRels.Select(x => x.Company))
            };

            return Ok(res);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> SwitchCompany(UserSwitchCompanyVM val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(UserId);
            user.CompanyId = val.CompanyId;
            await _userManager.UpdateAsync(user);
            _userService.ClearRuleCache(new List<string>() { user.Id });

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "System.ApplicationUser.Create")]
        public async Task<IActionResult> ActionImport(ApplicationUserImportExcelViewModel val)
        {
            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<ApplicationUserRowExcel>();
            var errors = new List<string>();
            using (var stream = new MemoryStream(fileData))
            {
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var errs = new List<string>();

                        var name = Convert.ToString(worksheet.Cells[row, 1].Value);
                        if (string.IsNullOrWhiteSpace(name))
                            errs.Add("Tên người dùng là bắt buộc");

                        var userName = Convert.ToString(worksheet.Cells[row, 4].Value);
                        if (string.IsNullOrWhiteSpace(userName))
                            errs.Add("Tên tài khoản là bắt buộc");

                        if (errs.Any())
                        {
                            errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                            continue;
                        }

                        data.Add(new ApplicationUserRowExcel
                        {
                            Name = name,
                            PhoneNumber = Convert.ToString(worksheet.Cells[row, 2].Value),
                            Email = Convert.ToString(worksheet.Cells[row, 3].Value),
                            UserName = userName,
                            Password = Convert.ToString(worksheet.Cells[row, 5].Value),
                            Group = Convert.ToString(worksheet.Cells[row, 6].Value),
                        });
                    }
                }
            }

            if (errors.Any())
                return Ok(new ApplicationUserImportResponse { Success = false, Errors = errors });

            var companyId = CompanyId;

            var row_tmp = 2;
            await _unitOfWork.BeginTransactionAsync();
            foreach (var item in data)
            {
                var errs = new List<string>();

                var partner = new Partner()
                {
                    Name = item.Name,
                    Email = item.Email,
                    CompanyId = companyId,
                    Customer = false
                };

                await _partnerService.CreateAsync(partner);

                var user = _mapper.Map<ApplicationUser>(item);
                user.CompanyId = companyId;
                user.ResCompanyUsersRels.Add(new ResCompanyUsersRel { CompanyId = companyId });
                user.PartnerId = partner.Id;

                if (!string.IsNullOrEmpty(item.Group))
                {
                    var excel_group = await _resGroupService.SearchQuery(x => x.Name == item.Group).FirstOrDefaultAsync();
                    if (excel_group != null)
                    {
                        var to_add = new List<Guid>() { excel_group.Id };
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
                    }
                }

                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    if (result.Errors.Any(x => x.Code == "DuplicateUserName"))
                        errs.Add($"Tài khoản {item.UserName} đã được sử dụng");
                    else
                        errs.Add(string.Join(", ", result.Errors.Select(x => x.Description)));
                }

                if (!string.IsNullOrEmpty(item.Password))
                {
                    var addResult = await _userManager.AddPasswordAsync(user, item.Password);
                    if (!addResult.Succeeded)
                        errs.Add($"Add password fail");
                }

                if (errs.Any())
                {
                    errors.Add($"Dòng {row_tmp}: {string.Join(", ", errs)}");
                    row_tmp++;
                    continue;
                }
                else
                    row_tmp++;
            }

            if (!errors.Any())
            {
                _unitOfWork.Commit();
                return Ok(new ApplicationUserImportResponse { Success = true, Errors = errors });
            }
            else
            {
                return Ok(new ApplicationUserImportResponse { Success = false, Errors = errors });
            }
        }

        private async Task SaveAvatar(Partner partner, ApplicationUserDisplay val)
        {
            partner.Avatar = val.Avatar;
        }
    }
}