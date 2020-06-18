using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ToaThuocLineProfile : Profile
    {
        public ToaThuocLineProfile()
        {
            CreateMap<ToaThuocLine, ToaThuocLineDisplay>();

            CreateMap<ToaThuocLineDisplay, ToaThuocLine>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore())
                .ForMember(x => x.ToaThuoc, x => x.Ignore());

            CreateMap<ToaThuocLine, ToaThuocLinePrintViewModel>();

            CreateMap<ToaThuocLineSave, ToaThuocLine>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
