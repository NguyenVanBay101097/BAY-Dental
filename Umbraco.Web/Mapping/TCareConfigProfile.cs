using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class TCareConfigProfile: Profile
    {
        public TCareConfigProfile()
        {
            CreateMap<TCareConfigSave, TCareConfig>().ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
