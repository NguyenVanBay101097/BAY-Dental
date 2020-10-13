using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class IRSequenceProfile : Profile
    {
        public IRSequenceProfile()
        {
            CreateMap<IRSequence, IRSequenceDisplay>();
            CreateMap<IRSequence, IRSequenceViewModel>();
            CreateMap<IRSequenceSave, IRSequence>();
        }
    }
}
