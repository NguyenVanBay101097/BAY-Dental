using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ChamCongProfile : Profile
    {
        public ChamCongProfile()
        {
            CreateMap<ChamCongSave, ChamCong>().ForMember(x=> x.Id, y=> y.Ignore());
            CreateMap<ChamCong, ChamCongDisplay>();
         
        }
    }
}
