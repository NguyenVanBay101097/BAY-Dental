using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models;
using Umbraco.Web.Models.ContentEditing;
namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsComposersController : BaseApiController
    {
        private readonly ISmsComposerService _smsComposerService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        public SmsComposersController(IMapper mapper, ISmsComposerService smsComposerService, IUnitOfWorkAsync unitOfWorkAsync)
        {
            _smsComposerService = smsComposerService;
            _mapper = mapper;
            _unitOfWorkAsync = unitOfWorkAsync;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(SmsComposerSave val)
        {
            var entity = _mapper.Map<SmsComposer>(val);
            entity.CompositionMode = "mass";
            await _unitOfWorkAsync.BeginTransactionAsync();
            entity = await _smsComposerService.CreateAsync(entity);
            _unitOfWorkAsync.Commit();
            return Ok(entity);
        }
    }
}
