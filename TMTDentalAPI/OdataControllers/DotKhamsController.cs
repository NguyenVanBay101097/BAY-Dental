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
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class DotKhamsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IDotKhamService _dotkhamService;
        private readonly IViewRenderService _view;
        private readonly IUserService _userService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public DotKhamsController(
           IMapper mapper,
           IDotKhamService doikhamService,
           IViewRenderService view,
           IUserService userService,
           IUnitOfWorkAsync unitOfWork
         )
        {
            _mapper = mapper;
            _dotkhamService = doikhamService;
            _view = view;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        [EnableQuery]
        [HttpGet]
        public IActionResult Get()
        {
            var results = _mapper.ProjectTo<DotKhamVm>(_dotkhamService.SearchQuery());
            return Ok(results);
        }

        [EnableQuery]
        public SingleResult<DotKhamVm> Get([FromODataUri] Guid key)
        {

            var results = _mapper.ProjectTo<DotKhamVm>(_dotkhamService.SearchQuery(x => x.Id == key));

            return SingleResult.Create(results);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateDotKham([FromBody] DotKhamVm val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _unitOfWork.BeginTransactionAsync();
            await _dotkhamService.CreateOrUpdateDotKham(val);
            _unitOfWork.Commit();

            return NoContent();
        }


        [HttpPost]
        public async Task<IActionResult> GetAllDotKhamForSaleOrder([FromBody] GetAllDotKhamVm val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _unitOfWork.BeginTransactionAsync();
            var results = _mapper.ProjectTo<DotKhamVm>(_dotkhamService.SearchQuery(x => val.OrderIds.Contains(x.SaleOrderId.Value)));          
            _unitOfWork.Commit();

            return Ok(results);

        }



       


    }
}
