﻿using ApplicationCore.Entities;
using ApplicationCore.Models.PrintTemplate;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class MedicineOrderLineProfile : Profile
    {
        public MedicineOrderLineProfile()
        {

            CreateMap<MedicineOrderLine, MedicineOrderLineSave>();

            CreateMap<MedicineOrderLineSave, MedicineOrderLine>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.ToaThuocLine, x => x.Ignore())
                .ForMember(x => x.MedicineOrder, x => x.Ignore());

            CreateMap<MedicineOrderLine, MedicineOrderLineDisplay>();

            CreateMap<MedicineOrderLineDisplay, MedicineOrderLine>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.ToaThuocLine, x => x.Ignore())
                .ForMember(x => x.MedicineOrder, x => x.Ignore());

            CreateMap<MedicineOrderLine, MedicineOrderLinePrintTemplate>();
        }
    }
}
