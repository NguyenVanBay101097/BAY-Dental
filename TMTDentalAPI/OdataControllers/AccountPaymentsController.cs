using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class AccountPaymentsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IViewRenderService _view;
        private readonly IAccountPaymentService _accountPaymentService;
        public AccountPaymentsController(IViewRenderService view, IMapper mapper, IAccountPaymentService accountPaymentService)
        {
            _accountPaymentService = accountPaymentService;
            _mapper = mapper;
            _view = view;
        }

        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            var res = _mapper.ProjectTo<AccountPaymentViewModel>(_accountPaymentService.SearchQuery(x => true));
            return Ok(res);
        }

        [EnableQuery]
        public SingleResult<AccountPaymentViewModel> Get([FromODataUri] Guid key)
        {
            var res = _mapper.ProjectTo<AccountPaymentViewModel>(_accountPaymentService.SearchQuery(x => x.Id == key));
            return SingleResult.Create(res);
        }

        public async Task<IActionResult> GetPrint([FromODataUri] Guid key)
        {
            var res = await _accountPaymentService.GetPrint(key);
            var html = _view.Render("CustomerSalaryPayment", res);
            return Ok(new PrintData() { html = html });
        }
    }
}
