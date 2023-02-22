using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class RoutingProfile : Profile
    {
        public RoutingProfile()
        {
            CreateMap<Routing, RoutingBasic>();
            CreateMap<Routing, RoutingSimple>();
            CreateMap<Routing, RoutingDisplay>();

            CreateMap<RoutingDisplay, Routing>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore());
        }
    }
}
