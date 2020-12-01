using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TMTDentalAPI.OdataControllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class DotKhamLinesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDotKhamLineService _dotkhamLineService;
        private readonly IViewRenderService _view;
        private readonly IUserService _userService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public DotKhamLinesController(
           IMapper mapper,
           IDotKhamLineService doikhamLineService,
           IViewRenderService view,
           IUserService userService,
           IUnitOfWorkAsync unitOfWork
         )
        {
            _mapper = mapper;
            _dotkhamLineService = doikhamLineService;
            _view = view;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            var dotkhamLine = await _dotkhamLineService.GetByIdAsync(key);        

            if (dotkhamLine == null)
                return NotFound();

            await _dotkhamLineService.DeleteAsync(dotkhamLine);
            return NoContent();
        }
    }
}
