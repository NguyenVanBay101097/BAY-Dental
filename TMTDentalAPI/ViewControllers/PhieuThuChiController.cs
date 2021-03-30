using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.ViewControllers
{
    public class PhieuThuChiController : Controller
    {
        private readonly IPhieuThuChiService _phieuThuChiService;
        private readonly IViewToStringRenderService _viewToStringRenderService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public PhieuThuChiController(IPhieuThuChiService phieuThuChiService, IViewToStringRenderService viewToStringRenderService, IMapper mapper, IUserService userService)
        {
            _phieuThuChiService = phieuThuChiService;
            _viewToStringRenderService = viewToStringRenderService;
            _mapper = mapper;
            _userService = userService;
        }

        [PrinterNameFilterAttribute(Name = "PhieuThuChiPaperFormat")]
        public async Task<IActionResult> Print(Guid id)
        {
            var phieu = await _mapper.ProjectTo<PhieuThuChiPrintVM>(_phieuThuChiService.SearchQuery(x => x.Id == id)
                .Include(x => x.CreatedBy)
                .Include(x => x.Partner))
                .FirstOrDefaultAsync();

            if (phieu == null)
                return NotFound();

            phieu.AmountText = AmountToText.amount_to_text(phieu.Amount);

            var viewdata = ViewData.ToDictionary(x => x.Key, x => x.Value);

            var html = await _viewToStringRenderService.RenderViewAsync("PhieuThuChi/Print", phieu, viewdata);

            return Ok(new PrintData() { html = html });

        }
    }
}
