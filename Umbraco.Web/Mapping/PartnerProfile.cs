using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class PartnerProfile : Profile
    {
        public PartnerProfile()
        {
            CreateMap<Partner, PartnerSimpleContact>();
            CreateMap<Partner, PartnerBasic>().ReverseMap();

            CreateMap<Partner, PartnerDisplay>()
                .ForMember(x => x.Categories, x => x.MapFrom(s => s.PartnerPartnerCategoryRels))
                .ForMember(x => x.Histories, x => x.MapFrom(s => s.PartnerHistoryRels));
            CreateMap<PartnerDisplay, Partner>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Employees, x => x.Ignore())
                .ForMember(x => x.ZaloId, x => x.Ignore());

            CreateMap<Partner, PartnerSimple>();
            CreateMap<Partner, PartnerInfoViewModel>();

            CreateMap<Partner, PartnerPatch>();
            CreateMap<PartnerPatch, Partner>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Employees, x => x.Ignore());

            CreateMap<PartnerImportExcel, Partner>()
                .ForMember(x=>x.Id, x=>x.Ignore())
                .ForMember(x => x.Employees, x => x.Ignore())
                .ForMember(x=>x.Company, x=>x.Ignore());
        }
    }
}
