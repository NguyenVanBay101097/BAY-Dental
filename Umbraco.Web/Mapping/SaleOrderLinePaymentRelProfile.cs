using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SaleOrderLinePaymentRelProfile : Profile
    {
        public SaleOrderLinePaymentRelProfile()
        {
            CreateMap<SaleOrderLinePaymentRelSave, SaleOrderLinePaymentRel>()
                .ForMember(x => x.SaleOrderLine, x => x.Ignore());
        }
    }
}
