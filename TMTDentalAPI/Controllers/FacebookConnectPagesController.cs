using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookConnectPagesController : ControllerBase
    {
        private readonly IFacebookConnectPageService _facebookConnectPageService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public FacebookConnectPagesController(IFacebookConnectPageService facebookConnectPageService,
            IUnitOfWorkAsync unitOfWork)
        {
            _facebookConnectPageService = facebookConnectPageService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddConnect(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _facebookConnectPageService.AddConnect(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RemoveConnect(IEnumerable<Guid> ids)
        {
            await _facebookConnectPageService.RemoveConnect(ids);
            return NoContent();
        }
    }
}