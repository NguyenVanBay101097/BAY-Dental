using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.PublicApiControllers
{
    [Route("publicApi/Examinations")]
    [ApiController]
    [CheckTokenPublic]
    [AllowAnonymous]
    public class DotKhamsPublicController : ControllerBase
    {
        private readonly IDotKhamService _dotKhamService;
        private readonly IIrAttachmentService _attachmentService;
        private readonly IMapper _mapper;

        public DotKhamsPublicController(IDotKhamService dotKhamService, IMapper mapper,
            IIrAttachmentService attachmentService)
        {
            _dotKhamService = dotKhamService;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid? orderId = null)
        {
            var dotkhams = await _dotKhamService.SearchQuery(x => (!orderId.HasValue || x.SaleOrderId == orderId))
                .Select(x => new DotKhamPublic
                {
                    Id = x.Id,
                    Name = x.Name,
                    AssistantName = x.Assistant.Name,
                    Date = x.Date,
                    DoctorName = x.Doctor.Name,
                    Reason = x.Reason,
                    Lines = x.Lines.Select(s => new DotKhamLinePublic { 
                        NameStep = s.NameStep,
                        Note = s.Note,
                        ProductName = s.Product.Name,
                        Teeth = s.ToothRels.Select(m => m.Tooth.Name)
                    })
                }).ToListAsync();

            var dotKhamIds = dotkhams.Select(x => x.Id).ToList();
            var images = await _attachmentService.SearchQuery(x => x.ResModel == "dot.kham" && dotKhamIds.Contains(x.ResId.Value)).ToListAsync();
            foreach (var dotKham in dotkhams)
                dotKham.Images = images.Where(x => x.ResId == dotKham.Id).Select(x => x.Url).ToList();

            return Ok(dotkhams);
        }
    }
}
