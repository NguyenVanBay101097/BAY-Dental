using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class HrPayrollStructureProfile: Profile
    {
        public HrPayrollStructureProfile()
        {
            CreateMap<HrPayrollStructure, HrPayrollStructureDisplay>();
            CreateMap<HrPayrollStructure, HrPayrollStructureBase>();
            CreateMap<HrPayrollStructureDisplay, HrPayrollStructure>().ForMember(x => x.Id, x => x.Ignore());
            CreateMap<HrPayrollStructureSave, HrPayrollStructure>()
                .ForMember(x => x.Rules, x => x.Ignore())
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
