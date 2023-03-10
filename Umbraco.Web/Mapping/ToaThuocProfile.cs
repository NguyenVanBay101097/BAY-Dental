using ApplicationCore.Entities;
using ApplicationCore.Models.PrintTemplate;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ToaThuocProfile : Profile
    {
        public ToaThuocProfile()
        {
            CreateMap<ToaThuoc, ToaThuocDisplay>()
                .ForMember(x => x.Lines, x => x.Ignore());

            CreateMap<ToaThuocDisplay, ToaThuoc>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Name, x => x.Ignore())
                .ForMember(x => x.User, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.DotKham, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore());

            CreateMap<ToaThuoc, ToaThuocBasic>();

            CreateMap<ToaThuoc, ToaThuocPrintViewModel>();

            CreateMap<ToaThuoc, ToaThuocPrintTemplate>();

            CreateMap<ToaThuocSave, ToaThuoc>()
                .ForMember(x => x.Lines, x => x.Ignore());

            CreateMap<ToaThuocSaveFromUI, ToaThuoc>()
                .ForMember(x => x.Lines, x => x.Ignore());
        }
    }
}
