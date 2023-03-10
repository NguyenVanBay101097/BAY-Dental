using ApplicationCore.Entities;
using ApplicationCore.Models.PrintTemplate;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class LaboBiteJointProfile : Profile
    {
        public LaboBiteJointProfile()
        {
            CreateMap<LaboBiteJoint, LaboBiteJointBasic>();
            CreateMap<LaboBiteJoint, LaboBiteJointDisplay>();
            CreateMap<LaboBiteJointSave, LaboBiteJoint>();
            CreateMap<LaboBiteJointDisplay, LaboBiteJoint>().ForMember(x => x.Id, x => x.Ignore());
            CreateMap<LaboBiteJoint, LaboBiteJointSimple>();

            CreateMap<LaboBiteJoint, LaboBiteJointSimplePrintTemplate>();
        }

    }
}
