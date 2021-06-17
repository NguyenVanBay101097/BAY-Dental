using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class StockReportProfile : Profile
    {
        public StockReportProfile()
        {
            CreateMap<GetStockHistoryRes, GetStockHistoryResExcel>();
        }
    }
}
