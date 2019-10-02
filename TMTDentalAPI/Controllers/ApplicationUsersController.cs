using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
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
        public ApplicationUsersController(UserManager<ApplicationUser> userManager,
            IMapper mapper, IUnitOfWorkAsync unitOfWork, IPartnerService partnerService,
            IIRModelAccessService modelAccessService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _partnerService = partnerService;
            _modelAccessService = modelAccessService;
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
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ApplicationUserDisplay>(user));
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

            await _partnerService.CreateAsync(partner);

            var user = _mapper.Map<ApplicationUser>(val);
            user.PartnerId = partner.Id;
            var result = await _userManager.CreateAsync(user, val.Password);
            if (!result.Succeeded)
                throw new Exception("fail create user");

            _unitOfWork.Commit();

            val.Id = user.Id;
            return CreatedAtAction(nameof(Get), new { id = user.Id }, val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, ApplicationUserDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            _modelAccessService.Check("ResUser", "Update");
            var user = await _userManager.Users.Where(x => x.Id == id).Include(x => x.Partner).FirstOrDefaultAsync();
            if (user == null)
                return NotFound();
            await _unitOfWork.BeginTransactionAsync();

            user = _mapper.Map(val, user);
            await _userManager.UpdateAsync(user);
            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, val.Password);

            var partner = user.Partner;
            partner.Name = val.Name;
            partner.Email = val.Email;
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
            var res = await _userManager.Users.Where(x => string.IsNullOrEmpty(filter) || x.Name.Contains(filter))
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
            var res = await _userManager.Users.Where(x => string.IsNullOrEmpty(user.SearchNameUserName) || x.Name.Contains(user.SearchNameUserName))
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
            var res = await _userManager.Users.Where(x => string.IsNullOrEmpty(val.Search) || x.Name.Contains(val.Search) ||
            x.UserName.Contains(val.Search) || x.NormalizedUserName.Contains(val.Search))
                .OrderBy(x => x.Name).Skip(val.Offset).Take(val.Limit)
                .Select(x => new ApplicationUserSimple
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();
            return Ok(res);
        }

        [HttpPost("DefaultGet")]
        public IActionResult DefaultGet()
        {
            var res = new ApplicationUserDisplay();
            res.CompanyId = CompanyId;
            return Ok(res);
        }
    }
}