using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class AccountInvoiceProfile : Profile
    {
        public AccountInvoiceProfile()
        {
            CreateMap<AccountInvoice, AccountInvoiceBasic>();

            CreateMap<AccountInvoice, AccountInvoiceDisplay>();
            CreateMap<AccountInvoiceDisplay, AccountInvoice>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.Journal, x => x.Ignore())
                .ForMember(x => x.Account, x => x.Ignore())
                .ForMember(x => x.User, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.State, x => x.Ignore())
                .ForMember(x => x.Name, x => x.Condition(s => s.State == "draft"))
                .ForMember(x => x.DateInvoice, x => x.Condition(s => s.State == "draft"))
                .ForMember(x => x.PartnerId, x => x.Condition(s => s.State == "draft"))
                .ForMember(x => x.Origin, x => x.Condition(s => s.State == "draft"))
                .ForMember(x => x.Type, x => x.Condition(s => s.State == "draft"))
                .ForMember(x => x.Number, x => x.Ignore())
                .ForMember(x => x.Reference, x => x.Condition(s => s.State == "draft"))
                .ForMember(x => x.AmountTax, x => x.Ignore())
                .ForMember(x => x.InvoiceLines, x => x.Ignore())
                .ForMember(x => x.AmountTotal, x => x.Ignore())
                .ForMember(x => x.AmountUntaxed, x => x.Ignore())
                .ForMember(x => x.Reconciled, x => x.Ignore())
                .ForMember(x => x.Residual, x => x.Ignore());

            CreateMap<AccountInvoice, AccountInvoiceCbx>();

            CreateMap<AccountInvoice, AccountInvoicePrint>();

            CreateMap <AccountInvoice, AccountInvoiceSimple>();
            //báo cáo doanh thu
            CreateMap<RevenuePartnerReportDisplay, RevenuePartnerReportPrint>();
            CreateMap<RevenueTimeReportDisplay, RevenueTimeReportPrint>();
            CreateMap<RevenueServiceReportDisplay, RevenueServiceReportPrint>();
            CreateMap<RevenueEmployeeReportDisplay, RevenueEmployeeReportPrint>();

        }
    }
}
