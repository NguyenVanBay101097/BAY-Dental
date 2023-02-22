using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class RoutingLineProfile : Profile
    {
        public RoutingLineProfile()
        {
            CreateMap<RoutingLine, RoutingLineDisplay>();
            CreateMap<RoutingLineDisplay, RoutingLine>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore());
        }
    }
}
