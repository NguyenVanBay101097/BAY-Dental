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

        public DotKhamsController(IDotKhamService dotKhamService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork, IToaThuocService toaThuocService,
            IDotKhamLineService dotKhamLineService,
            ILaboOrderLineService laboOrderLineService,
            IIRModelAccessService modelAccessService,
            IDotKhamStepService dotKhamStepService,
            IIrAttachmentService attachmentService,
            ILaboOrderService laboOrderService,
            IUploadService uploadService)
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
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]DotKhamPaged val)
        {
            var result = await _dotKhamService.GetPagedResultAsync(val);

            var paged = new PagedResult2<DotKhamBasic>(result.TotalItems, val.Offset, val.Limit)
            {
                //Có thể dùng thư viện automapper
                Items = _mapper.Map<IEnumerable<DotKhamBasic>>(result.Items),
            };

            return Ok(paged);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var dotKham = await _dotKhamService.GetDotKhamForDisplayAsync(id);
            if (dotKham == null)
            {
                return NotFound();
            }
            var res = _mapper.Map<DotKhamDisplay>(dotKham);
            res.Lines = res.Lines.OrderBy(x => x.Sequence);
            foreach (var line in res.Lines)
            {
                line.Operations = line.Operations.OrderBy(x => x.Sequence);
            }
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DotKhamDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            var dotKham = _mapper.Map<DotKham>(val);
            var res = await _dotKhamService.CreateAsync(dotKham);
            val.Name = res.Name;
            val.Id = dotKham.Id;
            return Ok(val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, DotKhamDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var dotKham = await _dotKhamService.GetByIdAsync(id);
            if (dotKham == null)
                return NotFound();

            dotKham = _mapper.Map(val, dotKham);
            await _dotKhamService.UpdateAsync(dotKham);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _dotKhamService.Unlink(new List<Guid>() { id });
            return NoContent();
        }

        [HttpPost("{id}/ActionConfirm")]
        public async Task<IActionResult> ActionConfirm(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _dotKhamService.ActionConfirm(id);
            _unitOfWork.Commit();
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

        [HttpPost("{id}/GetDotKhamLines")]
        public async Task<IActionResult> GetDotKhamLines(Guid id)
        {
            var res = await _dotKhamLineService.GetAllForDotKham(id);
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

        [HttpGet("{id}/VisibleSteps")]
        public async Task<IActionResult> VisibleSteps(Guid id, string show = "dotkham")
        {
            var steps = await _dotKhamStepService.GetVisibleSteps(id, show);
            var res = _mapper.Map<IEnumerable<DotKhamStepDisplay>>(steps);
            return Ok(res);
        }

        [HttpGet("{id}/VisibleSteps2")]
        public async Task<IActionResult> VisibleSteps2(Guid id, string show = "dotkham")
        {
            var steps = await _dotKhamStepService.GetVisibleSteps2(id, show);
            var res = _mapper.Map<IEnumerable<IEnumerable<DotKhamStepDisplay>>>(steps);
            return Ok(res);
        }

        //Lấy đợt hẹn duy nhất của đợt khám
        [HttpGet("GetAppointmentDotKham")]
        public async Task<IActionResult> GetAppointmentDotKham(Guid id, string show = "dotkham")
        {
            var steps = await _dotKhamStepService.GetVisibleSteps2(id, show);
            var res = _mapper.Map<IEnumerable<IEnumerable<DotKhamStepDisplay>>>(steps);
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
                    Name = item.Name,
                    Type = "upload",
                    UploadId = item.Id,
                    MineType = item.MineType,
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


    }
}