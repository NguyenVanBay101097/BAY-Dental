using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class HrPayslipRunProfile : Profile
    {
        public HrPayslipRunProfile()
        {
            CreateMap<HrPayslipRun, HrPayslipRunBasic>();
           
            CreateMap<HrPayslipRun, HrPayslipRunDisplay>();

            CreateMap<HrPayslipRunDisplay, HrPayslipRun>()
                 .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.Slips, x => x.Ignore());

            CreateMap<HrPayslipRun, HrPayslipRunSave>();

            CreateMap<HrPayslipRunSave, HrPayslipRun>()
                 .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.Slips, x => x.Ignore());
        }
    }
}
