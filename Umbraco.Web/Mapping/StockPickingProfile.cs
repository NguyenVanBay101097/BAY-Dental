﻿using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class StockPickingProfile : Profile
    {
        public StockPickingProfile()
        {
            CreateMap<StockPickingDisplay, StockPicking>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.MoveLines, x => x.Ignore());

            CreateMap<StockPicking, StockPickingBasic>();
            CreateMap<StockPicking, StockPickingDisplay>();
        }
    }
}
