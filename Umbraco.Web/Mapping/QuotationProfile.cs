﻿using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class QuotationProfile : Profile
    {
        public QuotationProfile()
        {
            CreateMap<Quotation, QuotationBasic>();
            CreateMap<Quotation, QuotationDisplay>();
            CreateMap<QuotationSave, Quotation>();
        }
    }
}
