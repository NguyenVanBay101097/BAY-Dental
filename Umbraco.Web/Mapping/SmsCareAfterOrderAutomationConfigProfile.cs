using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SmsCareAfterOrderAutomationConfigProfile : Profile
    {
        public SmsCareAfterOrderAutomationConfigProfile()
        {
            CreateMap<SmsCareAfterOrderAutomationConfigSave, SmsCareAfterOrderAutomationConfig>();
            CreateMap<SmsCareAfterOrderAutomationConfig, SmsCareAfterOrderAutomationConfigDisplay>()
                 .ForMember(x => x.ProductCategories, x => x.MapFrom(s => s.SmsConfigProductCategoryRels.Select(m => m.ProductCategory)))
                .ForMember(x => x.Products, x => x.MapFrom(s => s.SmsConfigProductRels.Select(m => m.Product)));
        }
    }
}
