using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ResInsuranceReportsProfile : Profile
    {
        public ResInsuranceReportsProfile()
        {
            CreateMap<InsuranceReportItem, ReportInsuranceDebitExcel>();
        }
    }
}
