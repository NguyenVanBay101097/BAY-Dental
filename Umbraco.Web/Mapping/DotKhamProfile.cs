using ApplicationCore.Entities;
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

            CreateMap<DotKham, DotKhamDisplay>();
            CreateMap<DotKhamDisplay, DotKham>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Name, x => x.Ignore())
                .ForMember(x => x.Invoice, x => x.Ignore())
                .ForMember(x => x.SaleOrder, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore())
                .ForMember(x => x.User, x => x.Ignore())
                .ForMember(x => x.State, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.Doctor, x => x.Ignore())
                .ForMember(x => x.Assistant, x => x.Ignore())
                .ForMember(x => x.Appointment, x => x.Ignore());

            CreateMap<DotKham, DotKhamSimple>();

            CreateMap<DotKham, DotKhamPatch>();
            CreateMap<DotKhamPatch, DotKham>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Name, x => x.Ignore())
                .ForMember(x => x.Invoice, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore())
                .ForMember(x => x.User, x => x.Ignore())
                .ForMember(x => x.State, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.Doctor, x => x.Ignore())
                .ForMember(x => x.Assistant, x => x.Ignore())
                .ForMember(x => x.Appointment, x => x.Ignore());
        }
    }
}
