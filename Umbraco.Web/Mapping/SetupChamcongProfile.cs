using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SetupChamcongProfile : Profile
    {
        public SetupChamcongProfile()
        {
            CreateMap<SetupChamcongDisplay, SetupChamcong>().ForMember(x=> x.Id, y=> y.Ignore());
            CreateMap<SetupChamcong, SetupChamcongDisplay>();
         
        }
    }
}
