using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class LoaiThuChiProfile : Profile
    {
        public LoaiThuChiProfile()
        {
            CreateMap<LoaiThuChi, LoaiThuChiBasic>();
            CreateMap<LoaiThuChi, LoaiThuChiSave>();

            CreateMap<LoaiThuChiSave, LoaiThuChi>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore());
          
        }
    }
}
