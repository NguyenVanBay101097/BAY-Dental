using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class HrPayslipLineProfile : Profile
    {
        public HrPayslipLineProfile()
        {
            CreateMap<HrPayslipLine, HrPayslipLineDisplay>();
            CreateMap<HrPayslipLineDisplay, HrPayslipLine>().ForMember(x => x.Id, x => x.Ignore());
            CreateMap<HrPayslipLineSave, HrPayslipLine>().ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
