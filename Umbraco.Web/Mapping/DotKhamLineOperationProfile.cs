using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class DotKhamLineOperationProfile : Profile
    {
        public DotKhamLineOperationProfile()
        {
            CreateMap<DotKhamLineOperation, DotKhamLineOperationDisplay>();
            CreateMap<DotKhamLineOperationDisplay, DotKhamLineOperation>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Line, x => x.Ignore())
                .ForMember(x => x.State, x => x.Ignore());
        }
    }
}
