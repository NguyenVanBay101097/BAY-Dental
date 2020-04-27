﻿using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ServiceCardOrderProfile : Profile
    {
        public ServiceCardOrderProfile()
        {
            CreateMap<ServiceCardOrder, ServiceCardOrderBasic>();
            CreateMap<ServiceCardOrder, ServiceCardOrderDisplay>()
                .ForMember(x => x.CardCount, x => x.MapFrom(s => s.Cards.Count));
            CreateMap<ServiceCardOrderSave, ServiceCardOrder>();
        }
    }
}
