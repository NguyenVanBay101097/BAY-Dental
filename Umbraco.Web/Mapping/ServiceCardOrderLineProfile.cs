using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ServiceCardOrderLineProfile : Profile
    {
        public ServiceCardOrderLineProfile()
        {
            CreateMap<ServiceCardOrderLine, ServiceCardOrderLineDisplay>()
                .ForMember(x => x.CardCount, x => x.MapFrom(s => s.Cards.Count));
            CreateMap<ServiceCardOrderLineSave, ServiceCardOrderLine>();
        }
    }
}
