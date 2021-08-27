using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class AccountCommonPartnerReportProfile : Profile
    {
        public AccountCommonPartnerReportProfile()
        {
            CreateMap<ReportPartnerDebitRes, ReportPartnerDebitPrint>();
            CreateMap<ReportPartnerDebitRes, ReportPartnerDebitExcel>();
        }
    }
}
