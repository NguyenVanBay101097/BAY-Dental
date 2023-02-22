﻿using ApplicationCore.Entities;
using ApplicationCore.Models.PrintTemplate;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class AgentProfile : Profile
    {
        public AgentProfile()
        {
            CreateMap<Agent, AgentBasic>();

            CreateMap<Agent, AgentDisplay>();

            CreateMap<Agent, AgentSave>();

            CreateMap<AgentSave, Agent>()
          .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<AgentDisplay, Agent>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<Agent, AgentPrintVM>();

            CreateMap<Agent, AgentPrintTemplate>();
        }
    }
}
