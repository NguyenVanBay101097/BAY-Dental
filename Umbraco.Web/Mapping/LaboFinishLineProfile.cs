using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class LaboFinishLineProfile : Profile
    {
        public LaboFinishLineProfile()
        {
            CreateMap<LaboFinishLine, LaboFinishLineBasic>();
            CreateMap<LaboFinishLine, LaboFinishLineDisplay>();
            CreateMap<LaboFinishLineDisplay, LaboFinishLine>().ForMember(x=>x.Id, x=> x.Ignore());
        }

    }
}
