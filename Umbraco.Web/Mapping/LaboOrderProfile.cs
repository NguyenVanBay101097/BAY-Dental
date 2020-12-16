using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class LaboOrderProfile : Profile
    {
        public LaboOrderProfile()
        {
            CreateMap<LaboOrder, LaboOrderBasic>();
            CreateMap<LaboOrder, LaboOrderDisplay>()
            .ForMember(x => x.Teeth, x => x.MapFrom(s => s.LaboOrderToothRel.Select(x=>x.Tooth)));

            CreateMap<LaboOrderDisplay, LaboOrder>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Name, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore())
                .ForMember(x => x.OrderLines, x => x.Ignore());
            CreateMap<LaboOrder, LaboOrderPrintVM>();
        }
    }
}
