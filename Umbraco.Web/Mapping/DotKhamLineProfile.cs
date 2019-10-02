using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class DotKhamLineProfile : Profile
    {
        public DotKhamLineProfile()
        {
            CreateMap<DotKhamLine, DotKhamLineBasic>();
            CreateMap<DotKhamLine, DotKhamLineDisplay>()
                .ForMember(x => x.HasOps, x => x.MapFrom(s => s.Operations.Any()));
            CreateMap<DotKhamLineDisplay, DotKhamLine>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.DotKham, x => x.Ignore())
                .ForMember(x => x.Routing, x => x.Ignore())
                .ForMember(x => x.Operations, x => x.Ignore())
                .ForMember(x => x.User, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore())
                .ForMember(x => x.State, x => x.Ignore());
            CreateMap<IGrouping<Guid?,DotKhamLine>, DotKhamLineBasic > ();
        }
    }
}
