using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SalaryPaymentProfile : Profile
    {
        public SalaryPaymentProfile()
        {
            CreateMap<SalaryPayment, SalaryPaymentVm>();

            CreateMap<SalaryPayment, SalaryPaymentSave>();
            CreateMap<SalaryPaymentSave, SalaryPayment>();
        }
    }
}
