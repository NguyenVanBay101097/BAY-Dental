using ApplicationCore.Entities;
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
            CreateMap<LaboBiteJointDisplay, LaboBiteJoint>().ForMember(x => x.Id, x => x.Ignore());
        }

    }
}
