using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class BankProfile : Profile
    {
        public BankProfile()
        {
            CreateMap<Bank, BankBasic>();
         
            CreateMap<Bank, BankSave>();

            CreateMap<BankSave, Bank>()
                 .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
