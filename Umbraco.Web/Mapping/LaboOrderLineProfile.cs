using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class LaboOrderLineProfile : Profile
    {
        public LaboOrderLineProfile()
        {
            CreateMap<LaboOrderLine, LaboOrderLineBasic>();

            CreateMap<LaboOrderLine, LaboOrderLineDisplay>();
            CreateMap<LaboOrderLineDisplay, LaboOrderLine>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore())
                .ForMember(x => x.Customer, x => x.Ignore())
                .ForMember(x => x.Supplier, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.Invoice, x => x.Ignore());
        }
    }
}
