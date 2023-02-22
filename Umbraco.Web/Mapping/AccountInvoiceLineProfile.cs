using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class AccountInvoiceLineProfile : Profile
    {
        public AccountInvoiceLineProfile()
        {
            CreateMap<AccountInvoiceLine, AccountInvoiceLineDisplay>()
                .ForMember(x => x.Teeth, x => x.MapFrom(s => s.AccountInvoiceLineToothRels.Select(m => m.Tooth)));
            CreateMap<AccountInvoiceLineDisplay, AccountInvoiceLine>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.UoM, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore())
                .ForMember(x => x.Account, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.AccountInvoiceLineToothRels, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.Employee, x => x.Ignore())
                .ForMember(x => x.ToothCategory, x => x.Ignore())
                .ForMember(x => x.Invoice, x => x.Ignore());

            CreateMap<AccountInvoiceLine, AccountInvoiceLinePrint>();
            CreateMap<AccountInvoiceLine, AccountInvoiceLineSimple>();
        }
    }
}
