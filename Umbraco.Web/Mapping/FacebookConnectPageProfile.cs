using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class FacebookConnectPageProfile : Profile
    {
        public FacebookConnectPageProfile()
        {
            CreateMap<FacebookConnectPage, FacebookConnectPageDisplay>();
        }
    }
}
