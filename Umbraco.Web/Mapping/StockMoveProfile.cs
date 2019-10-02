using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class StockMoveProfile : Profile
    {
        public StockMoveProfile()
        {
            CreateMap<StockMoveDisplay, StockMove>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore());
            CreateMap<StockMove, StockMoveDisplay>();
        }
    }
}
