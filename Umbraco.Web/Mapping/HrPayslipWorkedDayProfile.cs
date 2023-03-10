using ApplicationCore.Entities;
using ApplicationCore.Models.PrintTemplate;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class HrPayslipWorkedDayProfile : Profile
    {
        public HrPayslipWorkedDayProfile()
        {
            CreateMap<HrPayslipWorkedDays, HrPayslipWorkedDayDisplay>();
            CreateMap<HrPayslipWorkedDays, HrPayslipWorkedDayBasic>();
            CreateMap<HrPayslipWorkedDayDisplay, HrPayslipWorkedDays>().ForMember(x => x.Id, x => x.Ignore());
            CreateMap<HrPayslipWorkedDaySave, HrPayslipWorkedDays>().ForMember(x => x.Id, x => x.Ignore());
            CreateMap<HrPayslipWorkedDayDisplay, HrPayslipWorkedDaySave>().ForMember(x => x.Id, x => x.Ignore());
            CreateMap<HrPayslipWorkedDays, HrPayslipWorkedDayPrintTemplate>();
        }
    }
}
