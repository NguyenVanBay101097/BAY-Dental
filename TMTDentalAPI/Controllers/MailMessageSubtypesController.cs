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
    public class MailMessageSubtypesController : BaseApiController
    {
        private IMapper _mapper;
        private readonly IMailMessageSubtypeService _mailMessageSubtypeService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public MailMessageSubtypesController(IMapper mapper, IMailMessageSubtypeService mailMessageSubtypeService, IUnitOfWorkAsync unitOfWork)
        {
            _mapper = mapper;
            _mailMessageSubtypeService = mailMessageSubtypeService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var subtypes = await _mailMessageSubtypeService.SearchQuery().ToListAsync();
            var res = _mapper.Map<IEnumerable<MailMessageSubtypeListResponse>>(subtypes);
            return Ok(res);
        }
    }
}
