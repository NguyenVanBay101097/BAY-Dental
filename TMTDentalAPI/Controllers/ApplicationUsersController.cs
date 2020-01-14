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
        public ApplicationUsersController(UserManager<ApplicationUser> userManager,
            IMapper mapper, IUnitOfWorkAsync unitOfWork, IPartnerService partnerService,
            IIRModelAccessService modelAccessService, IUserService userService,
            IUploadService uploadService, ICompanyService companyService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _partnerService = partnerService;
            _modelAccessService = modelAccessService;
            _userService = userService;
            _uploadService = uploadService;
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]ApplicationUserPaged val)
        {
            _modelAccessService.Check("ResUser", "Read");
            var query = _userManager.Users;
            if (!string.IsNullOrEmpty(val.SearchNameUserName))
                query = query.Where(x => x.Name.Contains(val.SearchNameUserName) || x.UserName.Contains(val.SearchNameUserName));
            var companyId = CompanyId;
            query = query.Where(x => x.CompanyId == companyId);
            query = query.OrderBy(x => x.Name);
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();
            var totalItems = await query.CountAsync();
            var result = new PagedResult2<ApplicationUserBasic>(totalItems, val.Offset, val.Limit) {
                Items = _mapper.Map<IEnumerable<ApplicationUserBasic>>(items)
            };
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            _modelAccessService.Check("ResUser", "Read");
            var user = await _userManager.Users.Where(x => x.Id == id).Include(x => x.Company).Include(x => x.ResCompanyUsersRels)
                .Include("ResCompanyUsersRels.Company").Include(x=>x.Partner).FirstOrDefaultAsync();
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
        public async Task<IActionResult> Create(ApplicationUserDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            _modelAccessService.Check("ResUser", "Create");
            await _unitOfWork.BeginTransactionAsync();
            var partner = new Partner()
            {
                Name = val.Name,
                Email = val.Email,
                CompanyId = val.CompanyId,
                Customer = false
            };

            await SaveAvatar(partner, val);

            await _partnerService.CreateAsync(partner);

            var user = _mapper.Map<ApplicationUser>(val);
            user.PartnerId = partner.Id;

            foreach (var company in val.Companies)
            {
                user.ResCompanyUsersRels.Add(new ResCompanyUsersRel { CompanyId = company.Id });
            }

            var result = await _userManager.CreateAsync(user, val.Password);
            if (!result.Succeeded)
            {
                if (result.Errors.Any(x => x.Code == "DuplicateUserName"))
                    throw new Exception($"Tài khoản {val.UserName} đã được sử dụng");
                else
                    throw new Exception("Lỗi chưa xác định, vui lòng liên hệ với người quản trị phần mềm");
            }

            _unitOfWork.Commit();

            val.Id = user.Id;
            return CreatedAtAction(nameof(Get), new { id = user.Id }, val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, ApplicationUserDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            _modelAccessService.Check("ResUser", "Write");
            var user = await _userManager.Users.Where(x => x.Id == id).Include(x => x.Partner)
                .Include(x => x.ResCompanyUsersRels).FirstOrDefaultAsync();
            if (user == null)
                return NotFound();
            await _unitOfWork.BeginTransactionAsync();

            user = _mapper.Map(val, user);
            user.ResCompanyUsersRels.Clear();
            foreach (var company in val.Companies)
            {
                user.ResCompanyUsersRels.Add(new ResCompanyUsersRel { CompanyId = company.Id });
            }

            await _userManager.UpdateAsync(user);
            if (!string.IsNullOrEmpty(val.Password))
            {
                await _userManager.RemovePasswordAsync(user);
                await _userManager.AddPasswordAsync(user, val.Password);
            }

            var partner = user.Partner;
            partner.Name = val.Name;
            partner.Email = val.Email;
            await SaveAvatar(partner, val);
            await _partnerService.UpdateAsync(partner);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(string id)
        {
            _modelAccessService.Check("ResUser", "Unlink");
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            await _userManager.DeleteAsync(user);

            return NoContent();
        }

        [HttpGet("Autocomplete")]
        public async Task<IActionResult> Autocomplete(string filter = "")
        {
            var companyId = CompanyId;
            var res = await _userManager.Users.Where(x => (string.IsNullOrEmpty(filter) || x.Name.Contains(filter))
            && x.CompanyId == companyId)
                .Select(x => new ApplicationUserSimple {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();
            return Ok(res);
        }

        [HttpPost("AutocompleteUser")]
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
        public async Task<IActionResult> AutocompleteSimple(ApplicationUserPaged val)
        {
            var companyId = CompanyId;
            var res = await _userManager.Users.Where(x => (string.IsNullOrEmpty(val.Search) || x.Name.Contains(val.Search) ||
            x.UserName.Contains(val.Search) || x.NormalizedUserName.Contains(val.Search)) && x.CompanyId == companyId)
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

        private async Task SaveAvatar(Partner partner, ApplicationUserDisplay val)
        {
            if (!string.IsNullOrEmpty(val.Avatar) && ImageHelper.IsBase64String(val.Avatar))
            {
                var fileName = Path.GetRandomFileName() + "." + ImageHelper.GetImageExtension(val.Avatar);
                var uploadResult = await _uploadService.UploadBinaryAsync(val.Avatar, fileName: fileName);
                if (uploadResult != null)
                    partner.Avatar = uploadResult.Id;
            }
            else if (string.IsNullOrEmpty(val.Avatar))
            {
                partner.Avatar = null;
            }
        }
    }
}