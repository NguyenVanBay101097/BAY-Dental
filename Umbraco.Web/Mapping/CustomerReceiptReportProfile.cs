using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class CustomerReceiptReportProfile : Profile
    {
        public CustomerReceiptReportProfile()
        {
            CreateMap<CustomerReceiptReport, CustomerReceiptReportBasic>();
       
        }
    }
}
