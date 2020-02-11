using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class LaboOrderProfile : Profile
    {
        public LaboOrderProfile()
        {
            CreateMap<LaboOrder, LaboOrderBasic>();
            CreateMap<LaboOrder, LaboOrderDisplay>();
            CreateMap<LaboOrderDisplay, LaboOrder>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Name, x => x.Ignore())
                .ForMember(x => x.DotKham, x => x.Ignore())
                .ForMember(x => x.SaleOrder, x => x.Ignore())
                .ForMember(x => x.State, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.Customer, x => x.Ignore())
                .ForMember(x => x.OrderLines, x => x.Ignore());
            CreateMap<LaboOrder, LaboOrderPrintVM>();
        }
    }
}
