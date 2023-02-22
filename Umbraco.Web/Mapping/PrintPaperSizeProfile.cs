using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class PrintPaperSizeProfile : Profile
    {
        public PrintPaperSizeProfile()
        {
            CreateMap<PrintPaperSize, PrintPaperSizeBasic>();

            CreateMap<PrintPaperSize, PrintPaperSizeSave>();

            CreateMap<PrintPaperSize, PrintPaperSizeDisplay>();

            CreateMap<PrintPaperSizeSave, PrintPaperSize>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<PrintPaperSizeDisplay, PrintPaperSize>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
