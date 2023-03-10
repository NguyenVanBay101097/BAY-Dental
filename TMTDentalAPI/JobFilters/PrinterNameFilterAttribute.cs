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
            var pageSizeObj = requestSerivce.GetService<IPrintPaperSizeService>();
            var irConfigParameter = requestSerivce.GetService<IIrConfigParameterService>();
            var modelDataObj = requestSerivce.GetService<IIRModelDataService>();
            var _mapper = requestSerivce.GetService<IMapper>();

            var paperDefault = new PrintPaperSizeDisplay();
            var configPrint = configPrintObj.SearchQuery(x => x.Name == Name).Include(x => x.PrintPaperSize).FirstOrDefault();
            if (configPrint == null)
            {
                configPrint = new ConfigPrint();
                var paperSize = modelDataObj.GetRef<PrintPaperSize>("base.paperformat_a4").Result;
                if (paperSize == null)
                    throw new Exception("Không tìm thấy khổ giấy mặc định");

                configPrint.PrintPaperSize = paperSize;
            }
            else
            {
                if (configPrint.PrintPaperSize == null)
                {
                    var paperSize = modelDataObj.GetRef<PrintPaperSize>("base.paperformat_a4").Result;
                    if (paperSize == null)
                        throw new Exception("Không tìm thấy khổ giấy mặc định");
                    configPrint.PrintPaperSize = paperSize;
                }
            }    

            var controller = context.Controller as Controller;
            controller.ViewData["ConfigPrint"] =  _mapper.Map<ConfigPrintDisplay>(configPrint);
            controller.ViewData["PrintPaperSizeDefault"] = _mapper.Map<PrintPaperSizeDisplay>(configPrint.PrintPaperSize);

            base.OnActionExecuting(context);
        }

    }
}
