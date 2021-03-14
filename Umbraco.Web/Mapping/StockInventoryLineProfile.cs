using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class StockInventoryLineProfile : Profile
    {
        public StockInventoryLineProfile()
        {
            CreateMap<StockInventoryLine, StockInventoryLineSave>();

            CreateMap<StockInventoryLineSave, StockInventoryLine>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<StockInventoryLine, StockInventoryLineDisplay>();

            CreateMap<StockInventoryLineDisplay, StockInventoryLine>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
