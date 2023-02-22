﻿using ApplicationCore.Entities;
using ApplicationCore.Models.PrintTemplate;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class StockInventoryProfile : Profile
    {
        public StockInventoryProfile()
        {
            CreateMap<StockInventory, StockInventoryBasic>();

            CreateMap<StockInventory, StockInventorySave>();

            CreateMap<StockInventory, StockInventoryDisplay>();

            CreateMap<StockInventorySave, StockInventory>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore())
                .ForMember(x => x.Moves, x => x.Ignore());

            CreateMap<StockInventoryDisplay, StockInventory>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore())
                .ForMember(x => x.Moves, x => x.Ignore());

            CreateMap<StockInventory, StockInventoryPrint>()
                .ForMember(x=>x.DateCreated , x=>x.MapFrom(s=>s.Date));

            CreateMap<StockInventory, StockInventoryPrintTemplate>()
               .ForMember(x => x.DateCreated, x => x.MapFrom(s => s.Date));

        }
    }
}
