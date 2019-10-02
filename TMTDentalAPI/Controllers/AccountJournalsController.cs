using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountJournalsController : BaseApiController
    {
        private readonly IAccountJournalService _accountJournalService;
        private readonly IMapper _mapper;

        public AccountJournalsController(IAccountJournalService accountJournalService,
            IMapper mapper)
        {
            _accountJournalService = accountJournalService;
            _mapper = mapper;
        }

        [HttpPost("Autocomplete")]
        public async Task<IActionResult> Autocomplete(AccountJournalFilter val)
        {
            var result = await _accountJournalService.GetAutocomplete(val);
            return Ok(result);
        }
    }
}