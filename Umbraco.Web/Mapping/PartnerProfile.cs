using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class PartnerProfile : Profile
    {
        public PartnerProfile()
        {
            CreateMap<Partner, PartnerSimpleContact>();
            CreateMap<Partner, PartnerBasic>()
                .ForMember(x => x.LastAppointmentDate, x => x.MapFrom(s => s.Appointments.OrderByDescending(s => s.Date).FirstOrDefault().Date));

            CreateMap<Partner, PartnerDisplay>()
                .ForMember(x => x.Categories, x => x.MapFrom(s => s.PartnerPartnerCategoryRels))
                .ForMember(x => x.Histories, x => x.MapFrom(s => s.PartnerHistoryRels));
            CreateMap<PartnerDisplay, Partner>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Source, x => x.Ignore())
                .ForMember(x => x.Consultant, x => x.Ignore())
                .ForMember(x => x.ReferralUser, x => x.Ignore())
                .ForMember(x => x.Title, x => x.Ignore())
                .ForMember(x => x.Avatar, x => x.Ignore())
                .ForMember(x => x.DateCreated, x => x.Ignore())
                .ForMember(x => x.ZaloId, x => x.Ignore());


            CreateMap<Partner, PartnerSimple>();
            CreateMap<Partner, PartnerSimpleInfo>()
                  .ForMember(x => x.Categories, x => x.MapFrom(s => s.PartnerPartnerCategoryRels));
            CreateMap<Partner, PartnerInfoViewModel>();
            CreateMap<Partner, PartnerInfoChangePhone>();

            CreateMap<Partner, PartnerPatch>();
            CreateMap<PartnerPatch, Partner>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Source, x => x.Ignore())
                .ForMember(x => x.ReferralUser, x => x.Ignore())
                .ForMember(x => x.Title, x => x.Ignore());

            CreateMap<PartnerImportExcel, Partner>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Source, x => x.Ignore())
                .ForMember(x => x.ReferralUser, x => x.Ignore())
                .ForMember(x => x.Title, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore());

            CreateMap<Partner, PartnerChangePhone>();
            CreateMap<PartnerChangePhone, Partner>();
            CreateMap<Partner, PartnerPrintVM>();
            CreateMap<Partner, PartnerViewModel>();

            CreateMap<Partner, PartnerInfoVm>();
            CreateMap<Partner, PartnerCustomerDonThuoc>();
        }
    }
}
