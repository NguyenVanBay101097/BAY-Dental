using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyUserInputsController : BaseApiController
    {
        private readonly ISurveyUserInputService _surveyUserInputService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public SurveyUserInputsController(ISurveyUserInputService surveyUserInputService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _surveyUserInputService = surveyUserInputService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

   

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _surveyUserInputService.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SurveyUserInputSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var userinput = await _surveyUserInputService.CreateUserInput(val);

            _unitOfWork.Commit();

            var basic = _mapper.Map<SurveyUserInputBasic>(userinput);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, SurveyUserInputSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _surveyUserInputService.UpdateUserInput(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }
    }
}
