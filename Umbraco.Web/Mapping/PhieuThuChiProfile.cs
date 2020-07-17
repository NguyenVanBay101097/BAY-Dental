using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class PhieuThuChiProfile : Profile
    {
        public PhieuThuChiProfile()
        {
            CreateMap<PhieuThuChi, PhieuThuChiBasic>();

            CreateMap<PhieuThuChi, PhieuThuChiSave>();
            CreateMap<PhieuThuChiSave, PhieuThuChi>()
                 .ForMember(x => x.Id, x => x.Ignore())
                 .ForMember(x => x.LoaiThuChi, x => x.Ignore())
                 .ForMember(x => x.Journal, x => x.Ignore());

            CreateMap<PhieuThuChi, PhieuThuChiDisplay>();
        }


    }
}
