using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberLevelsController : BaseApiController
    {
        private readonly IMemberLevelService _memberLevelService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        public MemberLevelsController(IMemberLevelService memberLevelService, IMapper mapper, IUnitOfWorkAsync unitOfWorkAsync)
        {
            _memberLevelService = memberLevelService;
            _mapper = mapper;
            _unitOfWorkAsync = unitOfWorkAsync;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _memberLevelService.Get();
            var basics = results.Select(x => new MemberLevelBasic
            {
                Id = x.Id,
                Name = x.Name,
                Color = x.Color,
                Point = x.Point
            });
            return Ok(basics);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MemberLevelSave val)
        {
            await _unitOfWorkAsync.BeginTransactionAsync();
            var memberLevel = _mapper.Map<MemberLevel>(val);
            memberLevel.CompanyId = CompanyId;
            await _memberLevelService.CreateAsync(memberLevel);
            _unitOfWorkAsync.Commit();
            var basic = _mapper.Map<MemberLevelBasic>(memberLevel);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, MemberLevelSave val)
        {
            var level = await _memberLevelService.GetByIdAsync(id);
            if (level == null)
                return NotFound();

            level = _mapper.Map(val, level);
            await _unitOfWorkAsync.BeginTransactionAsync();
            await _memberLevelService.UpdateAsync(level);
            _unitOfWorkAsync.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var level = await _memberLevelService.GetByIdAsync(id);
            if (level == null)
                return NotFound();
            await _memberLevelService.DeleteAsync(level);

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateMember(IEnumerable<MemberLevelSave> vals)
        {
            await _unitOfWorkAsync.BeginTransactionAsync();
            await _memberLevelService.UpdateMember(vals);
            _unitOfWorkAsync.Commit();
            return NoContent();
        } 
    }
}
