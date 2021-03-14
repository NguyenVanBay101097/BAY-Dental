using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class StockInventoryCriteriaProfile: Profile
    {
        public StockInventoryCriteriaProfile()
        {
            CreateMap<StockInventoryCriteria, StockInventoryCriteriaDisplay>();
            CreateMap<StockInventoryCriteriaSave, StockInventoryCriteria>();
            CreateMap<StockInventoryCriteria, StockInventoryCriteriaBasic>();
            CreateMap<StockInventoryCriteria, StockInventoryCriteriaSimple>();
        }
    }
}
