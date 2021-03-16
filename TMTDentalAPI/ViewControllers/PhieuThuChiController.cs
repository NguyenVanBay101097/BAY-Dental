﻿using System;
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
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public PhieuThuChiController(IPhieuThuChiService phieuThuChiService, IMapper mapper, IUserService userService)
        {
            _phieuThuChiService = phieuThuChiService;
            _mapper = mapper;
            _userService = userService;
        }

        public async  Task<IActionResult> Print(Guid id)
        {
            var phieu = await _mapper.ProjectTo<PhieuThuChiPrintVM>(_phieuThuChiService.SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
            if (phieu == null)
                return NotFound();

            phieu.AmountText = AmountToText.amount_to_text(phieu.Amount);

            var user = await _userService.GetCurrentUser();
            phieu.UserName = _phieuThuChiService == null ? null : user.Name;

            return View(phieu);
        }
    }
}
