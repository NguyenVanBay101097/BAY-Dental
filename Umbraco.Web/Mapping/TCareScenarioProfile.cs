using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class TCareScenarioProfile : Profile
    {
        public TCareScenarioProfile()
        {
            CreateMap<TCareScenario, TCareScenarioBasic>();
            CreateMap<TCareScenario, TCareScenarioDisplay>();
            CreateMap<TCareScenarioSave, TCareScenario>();
        }
    }
}
