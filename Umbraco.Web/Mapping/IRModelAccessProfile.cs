﻿using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class IRModelAccessProfile : Profile
    {
        public IRModelAccessProfile()
        {
            CreateMap<IRModelAccess, IRModelAccessDisplay>();
            CreateMap<IRModelAccessDisplay, IRModelAccess>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
