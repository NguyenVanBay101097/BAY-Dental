﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ApplicationCore.Constants;
using ApplicationCore.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.ViewControllers
{
    public class LaboOrderController : Controller
    {
        private readonly ILaboOrderService _laboOrderService;
        private readonly IViewToStringRenderService _viewToStringRenderService;

        public LaboOrderController(ILaboOrderService laboOrderService, IViewToStringRenderService viewToStringRenderService)
        {
            _laboOrderService = laboOrderService;
            _viewToStringRenderService = viewToStringRenderService;
        }

        [CheckAccess(Actions = "Labo.LaboOrder.Read")]
        [PrinterNameFilterAttribute(Name = AppConstants.LaboOrderPaperCode)]
        public async Task<IActionResult> Print(Guid id)
        {
            var res = await _laboOrderService.SearchQuery(x => x.Id == id)
              .Include(x => x.Company.Partner)
              .Include(x => x.Product)
              .Include(x => x.LaboBridge)
              .Include(x => x.LaboBiteJoint)
              .Include(x => x.LaboFinishLine)
              .Include(x => x.SaleOrderLine.Product)
              .Include(x => x.SaleOrderLine.Order)
              .Include(x => x.SaleOrderLine.Employee)
              .Include(x => x.LaboOrderProductRel).ThenInclude(x => x.Product)
              .Include(x => x.Partner)
              .Include(x => x.Customer)
              .Include("LaboOrderToothRel.Tooth")
              .FirstOrDefaultAsync();

            if (res == null)
                return NotFound();

            return View(res);

        }
    }
}
