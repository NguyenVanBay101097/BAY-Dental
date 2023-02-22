using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SaleOrderLineProductRequestedProfile: Profile
    {
        public SaleOrderLineProductRequestedProfile()
        {
            CreateMap<SaleOrderLineProductRequested, SaleOrderLineProductRequestedBasic>();
            CreateMap<SaleOrderLineProductRequested, SaleOrderLineProductRequestedDisplay>();
            CreateMap<SaleOrderLineProductRequestedSave, SaleOrderLineProductRequested>();
        }
    }
}
