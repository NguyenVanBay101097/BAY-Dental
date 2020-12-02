﻿using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class DotKhamProfile : Profile
    {
        public DotKhamProfile()
        {
            CreateMap<DotKham, DotKhamBasic>();

            CreateMap<DotKham, DotKhamVm>();
            CreateMap<DotKhamVm, DotKham>()
                 .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Name, x => x.Ignore())
                .ForMember(x => x.SaleOrder, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore())
                .ForMember(x => x.State, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.Doctor, x => x.Ignore())
                .ForMember(x => x.Appointment, x => x.Ignore());


            CreateMap<DotKham, DotKhamDisplay>();
            CreateMap<DotKhamDisplay, DotKham>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Name, x => x.Ignore())
                .ForMember(x => x.SaleOrder, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore())
                .ForMember(x => x.State, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.Doctor, x => x.Ignore())
                .ForMember(x => x.Appointment, x => x.Ignore())
                .ForMember(x => x.SaleOrderId, x => x.Condition(s => s.State == "draft"))
                .ForMember(x => x.PartnerId, x => x.Condition(s => s.State == "draft"))
                .ForMember(x => x.Date, x => x.Condition(s => s.State == "draft"));

            CreateMap<DotKham, DotKhamSimple>();

            CreateMap<DotKham, DotKhamPatch>();
            CreateMap<DotKhamPatch, DotKham>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Name, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore())
                .ForMember(x => x.State, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.Doctor, x => x.Ignore())
                .ForMember(x => x.Appointment, x => x.Ignore());

            CreateMap<DotKhamSave, DotKham>();
        }
    }
}
