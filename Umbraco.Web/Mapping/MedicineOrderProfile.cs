using ApplicationCore.Entities;
using ApplicationCore.Models.PrintTemplate;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class MedicineOrderProfile : Profile
    {
        public MedicineOrderProfile()
        {
            CreateMap<MedicineOrder, MedicineOrderBasic>();

            CreateMap<MedicineOrder, MedicineOrderSave>();

            CreateMap<MedicineOrder, MedicineOrderDisplay>();

            CreateMap<MedicineOrderDisplay, MedicineOrder>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.MedicineOrderLines, x => x.Ignore());

            CreateMap<MedicineOrderSave, MedicineOrder>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.MedicineOrderLines, x => x.Ignore());

            CreateMap<MedicineOrder, MedicineOrderPrint>();

            CreateMap<MedicineOrder, MedicineOrderPrintTemplate>();

        }
    }
}
