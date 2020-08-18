using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class AccountMoveLineProfile : Profile
    {
        public AccountMoveLineProfile()
        {
            CreateMap<AccountMoveLine, AccountMoveLineReport>();
            CreateMap<AccountMoveLine, AccountMoveLineBasic>();
        }
    }
}
