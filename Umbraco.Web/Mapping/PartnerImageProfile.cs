﻿using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
  public  class PartnerImageProfile : Profile
    {
        public PartnerImageProfile()
        {
            CreateMap<PartnerImage, PartnerImageBasic>();

            CreateMap<PartnerImageSave, PartnerImage>();

            CreateMap<PartnerImage, PartnerImageDisplay>();
            CreateMap<PartnerImageDisplay, PartnerImage>();
        }
    }
}
