using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SaleOrderProfile : Profile
    {
        public SaleOrderProfile()
        {
            CreateMap<SaleOrder, SaleOrderBasic>().ReverseMap();

            CreateMap<SaleOrder, SaleOrderDisplay>();
            CreateMap<SaleOrderDisplay, SaleOrder>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Name, x => x.Ignore())
                .ForMember(x => x.State, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.User, x => x.Ignore())
                .ForMember(x => x.OrderLines, x => x.Ignore());
        }
    }
}
