using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;


namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DotKhamsController : BaseApiController
    {
        private readonly IDotKhamService _dotKhamService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IToaThuocService _toaThuocService;
        private readonly IDotKhamLineService _dotKhamLineService;
        private readonly ILaboOrderLineService _laboOrderLineService;
        private readonly IIRModelAccessService _modelAccessService;
        private readonly IDotKhamStepService _dotKhamStepService;
        private readonly IIrAttachmentService _attachmentService;
        private readonly ILaboOrderService _laboOrderService;
        private readonly IUploadService _uploadService;
        private readonly IPartnerImageService _partnerImageService;

        public DotKhamsController(IDotKhamService dotKhamService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork, IToaThuocService toaThuocService,
            IDotKhamLineService dotKhamLineService,
            ILaboOrderLineService laboOrderLineService,
            IIRModelAccessService modelAccessService,
            IDotKhamStepService dotKhamStepService,
            IIrAttachmentService attachmentService,
            ILaboOrderService laboOrderService,
            IUploadService uploadService,
            IPartnerImageService partnerImageService)
        {
            _dotKhamService = dotKhamService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _toaThuocService = toaThuocService;
            _dotKhamLineService = dotKhamLineService;
            _laboOrderLineService = laboOrderLineService;
            _modelAccessService = modelAccessService;
            _dotKhamStepService = dotKhamStepService;
            _attachmentService = attachmentService;
            _laboOrderService = laboOrderService;
            _uploadService = uploadService;
            _partnerImageService = partnerImageService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Basic.DotKham.Read")]
        public async Task<IActionResult> Get([FromQuery]DotKhamPaged val)
        {
            var result = await _dotKhamService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Basic.DotKham.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var results = await _mapper.ProjectTo<DotKhamVm>(_dotKhamService.SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
            if (results == null)
                return NotFound();

            return Ok(results);
        }

        [HttpPost]
        [CheckAccess(Actions = "Basic.DotKham.Create")]
        public async Task<IActionResult> Create(DotKhamSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            var dotKham = _mapper.Map<DotKham>(val);
            await _dotKhamService.CreateAsync(dotKham);

            var basic = _mapper.Map<DotKhamBasic>(dotKham);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Basic.DotKham.Update")]
        public async Task<IActionResult> PUT(Guid id, DotKhamSaveVm val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _dotKhamService.UpdateDotKham(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Basic.DotKham.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _dotKhamService.Unlink(new List<Guid>() { id });
            return NoContent();
        }


        [HttpPost("DefaultGet")]
        public async Task<IActionResult> DefaultGet(DotKhamDefaultGet val)
        {
            var res = await _dotKhamService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost("{id}/GetToaThuocs")]
        public async Task<IActionResult> GetToaThuocs(Guid id)
        {
            var res = await _toaThuocService.GetToaThuocsForDotKham(id);
            return Ok(res);
        }

        [HttpPost("{id}/GetDotKhamLines2")]
        public async Task<IActionResult> GetDotKhamLines2(Guid id)
        {
            var res = await _dotKhamLineService.GetAllForDotKham2(id);
            return Ok(res);
        }

        [HttpPost("{id}/GetLaboOrderLines")]
        public async Task<IActionResult> GetLaboOrderLines(Guid id)
        {
            var res = await _laboOrderLineService.GetAllForDotKham(id);
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetLaboOrders(Guid id)
        {
            var res = await _laboOrderService.GetAllForDotKham(id);
            return Ok(res);
        }

        [HttpGet("{id}/GetAppointments")]
        public async Task<IActionResult> GetAppointments(Guid id)
        {
            var res = await _laboOrderLineService.GetAllForDotKham(id);
            return Ok(res);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, JsonPatchDocument<DotKhamPatch> dkpatch)
        {
            var entity = await _dotKhamService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var entityMap = _mapper.Map<DotKhamPatch>(entity);
            dkpatch.ApplyTo(entityMap);

            entity = _mapper.Map(entityMap, entity);
            await _dotKhamService.UpdateAsync(entity);

            return NoContent();
        }

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> BinaryUploadAttachment(Guid id, IList<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest();
            var list = new List<IrAttachment>();
            var tasks = new List<Task<UploadResult>>();
            foreach (var file in files)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    tasks.Add(HandleUploadAsync(Convert.ToBase64String(memoryStream.ToArray()), file.FileName));
                }
            }

            var result = await Task.WhenAll(tasks);
            foreach (var item in result)
            {
                if (item == null)
                    throw new Exception("Hình không hợp lệ hoặc vượt quá kích thước cho phép.");
                var attachment = new IrAttachment()
                {
                    ResModel = "dotkham",
                    ResId = id,
                    Name = item.FileName,
                    Type = "upload",
                    Url = item.FileUrl,
                };

                list.Add(attachment);
            }

            await _attachmentService.CreateAsync(list);

            return Ok(list);
        }

        private async Task<UploadResult> HandleUploadAsync(string base64, string fileName)
        {
            return await _uploadService.UploadBinaryAsync(base64, fileName: fileName);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetSearchedDotKham(Guid appointmentId)
        {
            var paged = new DotKhamPaged();
            paged.AppointmentId = appointmentId;
            var res = await _dotKhamService.GetPagedResultAsync(paged);

            var dotkham = res.Items.FirstOrDefault();

            return Ok(dotkham);
        }

        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Basic.DotKham.Read")]
        public async Task<IActionResult> GetInfo(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            //function return complex type
            var dotkham = await _dotKhamService.SearchQuery(x => x.Id == id).Select(x => new DotKhamDisplayVm
            {
                Date = x.Date,
                Doctor = x.Doctor != null ? new EmployeeSimple
                {
                    Id = x.Doctor.Id,
                    Name = x.Doctor.Name
                } : null,
                Assistant = x.Assistant != null ? new EmployeeSimple
                {
                    Id = x.Assistant.Id,
                    Name = x.Assistant.Name
                } : null,
                Id = x.Id,
                Sequence = x.Sequence,
                Reason = x.Reason,
                Name = x.Name
            }).FirstOrDefaultAsync();

            if (dotkham == null)
                return NotFound();

            dotkham.Lines = await _dotKhamLineService.SearchQuery(x => x.DotKhamId == id).OrderBy(x => x.Sequence).Select(x => new DotKhamLineDisplay
            {
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
            dotkham.IrAttachments = _mapper.Map<IEnumerable<IrAttachmentBasic>>(await _dotKhamService.GetListAttachment(id));

            return Ok(dotkham);
        }
    }
}