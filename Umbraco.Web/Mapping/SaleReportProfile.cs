using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SaleReportProfile : Profile
    {
        public SaleReportProfile()
        {
            CreateMap<ServiceReportRes, ServiceReportResPrint>();
            CreateMap<ServiceReportReq, ServiceReportDetailReq>();
        }
    }
}
