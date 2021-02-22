using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.ViewControllers
{
    public class PhieuThuChiController : Controller
    {
        private readonly IPhieuThuChiService _phieuThuChiService;
        private readonly IMapper _mapper;

        public PhieuThuChiController(IPhieuThuChiService phieuThuChiService, IMapper mapper)
        {
            _phieuThuChiService = phieuThuChiService;
            _mapper = mapper;
        }

        public async  Task<IActionResult> Print(Guid id)
        {
            var phieu = await _mapper.ProjectTo<PhieuThuChiPrintVM>(_phieuThuChiService.SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
            if (phieu == null)
                return NotFound();

            phieu.AmountText = AmountToText.amount_to_text(phieu.Amount);

            return View(phieu);
        }
    }
}
