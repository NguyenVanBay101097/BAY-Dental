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
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class DotKhamsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IDotKhamService _dotkhamService;
        private readonly IDotKhamLineService _dotKhamLineService;
        private readonly IPartnerImageService _partnerImageService;
        private readonly IViewRenderService _view;
        private readonly IUserService _userService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public DotKhamsController(
           IMapper mapper,
           IDotKhamService doikhamService,
           IDotKhamLineService dotKhamLineService,
           IPartnerImageService partnerImageService,
           IViewRenderService view,
           IUserService userService,
           IUnitOfWorkAsync unitOfWork
         )
        {
            _mapper = mapper;
            _dotkhamService = doikhamService;
            _dotKhamLineService = dotKhamLineService;
            _partnerImageService = partnerImageService;
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

        [HttpGet]
        public async Task<IActionResult> GetInfo([FromODataUri] Guid key)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            //function return complex type
            var dotkham = await _dotkhamService.SearchQuery(x => x.Id == key).Select(x => new DotKhamDisplayVm { 
                Date = x.Date,
                Doctor = x.Doctor != null ? new EmployeeSimple
                {
                    Id = x.Doctor.Id,
                    Name = x.Doctor.Name
                } : null,
                Id = x.Id,
                Sequence = x.Sequence,
                Reason = x.Reason               
            }).FirstOrDefaultAsync();

            if (dotkham == null)
                return NotFound();

            dotkham.Lines = await _dotKhamLineService.SearchQuery(x => x.DotKhamId == key).OrderBy(x => x.Sequence).Select(x => new DotKhamLineDisplay { 
                Teeth = x.ToothRels.Select(x => new ToothDisplay
                {
                    Id = x.ToothId,
                    Name = x.Tooth.Name
                }),
                Id = x.Id,
                NameStep = x.NameStep,
                Note = x.Note,
                Product = new ProductSimple
                {
                    Id = x.ProductId.Value,
                    Name = x.Product.Name
                },
                SaleOrderLineId = x.SaleOrderLineId
            }).ToListAsync();

            dotkham.DotKhamImages = await _partnerImageService.SearchQuery(x => x.DotkhamId.Value == key).Select(x=> new PartnerImageDisplay { 
                Id = x.Id,
                Date = x.Date.Value,
                Name = x.Name,
                UploadId = x.UploadId
            }).ToListAsync();

            return Ok(dotkham);
        }

        //[HttpPost]
        //public async Task<IActionResult> Post(DotKhamSaveVm val)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    await _unitOfWork.BeginTransactionAsync();
        //    var res = await _dotkhamService.CreateDotKham(val);
        //    var viewdotkham = _mapper.Map<DotKhamVm>(res);
        //    _unitOfWork.Commit();

        //    return Ok(viewdotkham);
        //}

        [HttpPut]
        public async Task<IActionResult> PUT([FromODataUri] Guid key, DotKhamSaveVm val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _dotkhamService.UpdateDotKham(key, val);
            _unitOfWork.Commit();

            return NoContent();
        }

       
    }
}
