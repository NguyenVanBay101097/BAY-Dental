﻿using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ToothCategoryProfile : Profile
    {
        public ToothCategoryProfile()
        {
            CreateMap<ToothCategory, ToothCategoryBasic>();
            CreateMap<ToothCategoryBasic, ToothCategory>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
