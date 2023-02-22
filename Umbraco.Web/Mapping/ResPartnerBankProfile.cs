using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ResPartnerBankProfile : Profile
    {
        public ResPartnerBankProfile()
        {
            CreateMap<ResPartnerBank, ResPartnerBankBasic>();
            CreateMap<ResPartnerBankBasic, ResPartnerBank>();

            CreateMap<ResPartnerBank,ResPartnerBankDisplay>();
            CreateMap<ResPartnerBankDisplay, ResPartnerBank>()
                .ForMember(x => x.Bank, x => x.Ignore())
                .ForMember(x=>x.Company, x=>x.Ignore())
                .ForMember(x=>x.Partner, x=>x.Ignore());
        }
    }
}
