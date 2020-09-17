using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class HrPayrollStructureTypeProfile: Profile
    {
        public HrPayrollStructureTypeProfile()
        {
            CreateMap<HrPayrollStructureType, HrPayrollStructureTypeDisplay>();
            CreateMap<HrPayrollStructureTypeDisplay, HrPayrollStructureType>().ForMember(x => x.Id, x => x.Ignore());
            CreateMap<HrPayrollStructureTypeSave, HrPayrollStructureType>().ForMember(x => x.Id, x => x.Ignore());
            CreateMap<HrPayrollStructureType, HrPayrollStructureTypeSimple>();
            CreateMap<HrPayrollStructureTypeSimple, HrPayrollStructureType>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
