using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class AccountJournalsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAccountJournalService _JournalService;

        public AccountJournalsController(
            IMapper mapper,
            IUnitOfWorkAsync unitOfWork, IAccountJournalService journalService
          )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _JournalService = journalService;
        }

        [EnableQuery]
        [HttpGet]
        public IActionResult Get()
        {
            var results = _mapper.ProjectTo<AccountJournalViewModel>(_JournalService.SearchQuery());
            return Ok(results);
        }
    }
}
