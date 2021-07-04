using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
   public class CustomerReceiptProfile : Profile
    {
        public CustomerReceiptProfile()
        {
            CreateMap<CustomerReceipt, CustomerReceiptBasic>();

            CreateMap<CustomerReceipt, CustomerReceiptDisplay>()
                .ForMember(x => x.Products, x => x.MapFrom(s => s.CustomerReceiptProductRels.Select(q => q.Product)));

            CreateMap<CustomerReceiptDisplay, CustomerReceipt>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<CustomerReceipt, CustomerReceiptSave>();

            CreateMap<CustomerReceiptSave, CustomerReceipt>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
