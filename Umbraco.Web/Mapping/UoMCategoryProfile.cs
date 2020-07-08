﻿using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class UoMCategoryProfile : Profile
    {
        public UoMCategoryProfile() 
        {
            CreateMap<UoMCategorySave, UoMCategory>();
            CreateMap<UoMCategory, UoMCategoryBasic>();

            CreateMap<UoMCategory, UoMCategorySimple>();
        }
    }
}
