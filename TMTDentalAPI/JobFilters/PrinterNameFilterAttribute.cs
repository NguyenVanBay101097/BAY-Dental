using Microsoft.Extensions.DependencyInjection;
using ApplicationCore.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.JobFilters
{
    public class PrinterNameFilterAttribute : ActionFilterAttribute 
    {
        public string Name { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var requestSerivce = context.HttpContext.RequestServices;
            var configPrintObj = requestSerivce.GetService<IConfigPrintService>();
            var _mapper = requestSerivce.GetService<IMapper>();
            var configPrint = new ConfigPrint();
            string name = Name;
            if (!string.IsNullOrEmpty(name))
                configPrint = configPrintObj.SearchQuery(x => x.Name == name).Include(x => x.PrintPaperSize).FirstOrDefault();

            if (!configPrint.PaperSizeId.HasValue)
                throw new Exception("Lỗi");

            var controller = context.Controller as Controller;
            controller.ViewData["ConfigPrint"] = _mapper.Map<ConfigPrintDisplay>(configPrint);

            base.OnActionExecuting(context);
        }

    }
}
