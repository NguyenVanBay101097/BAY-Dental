using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Constants;
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

        [PrinterNameFilterAttribute(Name = AppConstants.PhieuThuChiPaperCode)]
        public async Task<IActionResult> Print(Guid id)
        {
            var res = await _phieuThuChiService.SearchQuery(x => x.Id == id)
                .Include(x => x.Company)
                .Include(x => x.CreatedBy)
                .Include(x => x.Partner)
                .Include(x => x.LoaiThuChi).FirstOrDefaultAsync();

            var phieu = _mapper.Map<PhieuThuChiPrintVM>(res);


            if (phieu == null)
                return NotFound();

            phieu.AmountText = AmountToText.amount_to_text(phieu.Amount);


            return View(phieu);

        }

        [PrinterNameFilterAttribute(Name = AppConstants.PhieuThuChiPaperCode)]
        public async Task<IActionResult> Print2(Guid id)
        {
            var res = await _phieuThuChiService.GetPrint(id);

            if (res == null)
                return NotFound();

            res.AmountText = AmountToText.amount_to_text(res.Amount);

            return View(res);
        }
    }
}
