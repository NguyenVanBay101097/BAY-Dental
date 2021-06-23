﻿using ApplicationCore.Entities;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IPartnerService : IBaseService<Partner>
    {
        Task<PagedResult2<Partner>> GetPagedResultAsync(int offset = 0, int limit = 20, string search = "", string searchBy = "", bool? customer = null);
        Task<PagedResult2<PartnerBasic>> GetPagedResultAsync(PartnerPaged val);
        Task<Partner> GetPartnerForDisplayAsync(Guid id);
        Task<IEnumerable<PartnerSimple>> SearchAutocomplete(string filter = "", bool? customer = null);
        Task<IEnumerable<PartnerSimple>> SearchPartnersCbx(PartnerPaged val);
        Task<IEnumerable<PartnerSimpleInfo>> SearchPartnerInfosCbx(PartnerPaged val);
        Task<IEnumerable<PartnerSimpleContact>> SearchPartnersConnectSocial(PartnerPaged val);
        string GetFormatAddress(Partner partner);
        string GetGenderDisplay(Partner partner);
        Task<string> UploadImage(IFormFile file);
        Task<PartnerInfoViewModel> GetInfo(Guid id);
        Task<PagedResult2<AccountInvoiceDisplay>> GetCustomerInvoices(AccountInvoicePaged val);
        Task ImportExcel2(IFormFile file, Ex_ImportExcelDirect dir);
        Dictionary<Guid, PartnerCreditDebitItem> CreditDebitGet(IEnumerable<Guid> ids = null,
       DateTime? fromDate = null,
       DateTime? toDate = null);
        Task<PartnerDisplay> GetInfoPartner(Guid id);
        Task<IEnumerable<PartnerReportLocationItem>> ReportLocationCompanyDistrict(PartnerReportLocationCompanySearch val);
        Task<IEnumerable<PartnerReportLocationItem>> ReportLocationCompanyWard(PartnerReportLocationCompanySearch val);
        Task<IEnumerable<PartnerReportLocationCity>> ReportLocationCity(ReportLocationCitySearch val);
        Task<IEnumerable<PartnerReportLocationDistrict>> ReportLocationDistrict(PartnerReportLocationCity val);
        Task<IEnumerable<PartnerReportLocationWard>> ReportLocationWard(PartnerReportLocationDistrict val);
        IQueryable<Partner> GetQueryPaged(PartnerPaged val);
        Task UpdateCustomersZaloId();
        void FetchAllPSIDFromFacebookFanpage();

        Task<IEnumerable<PartnerInfoChangePhone>> OnChangePartner(string phone);
        //Task<PartnerInfoViewModel> CheckPartner(CheckMergeFacebookPage val);

        Task<PartnerImportResponse> ActionImport(PartnerImportExcelViewModel val);

        Task<PartnerImportResponse> ActionImportUpdate(PartnerImportExcelViewModel val);

        Task<PartnerImportResponse> ImportSupplier(PartnerImportExcelViewModel val);

        Task<PartnerPrintProfileVM> GetPrint(Guid id);

        Task<AppointmentBasic> GetNextAppointment(Guid id);

        Task<IEnumerable<PartnerCustomerExportExcelVM>> GetExcel(PartnerPaged val);
        Task AddOrRemoveTags(PartnerAddRemoveTagsVM val, bool isAdd);

        Task<IQueryable<PartnerViewModel>> GetViewModelsAsync();
        Task<IQueryable<GridPartnerViewModel>> GetGridViewModelsAsync();
        Task UpdateTags(PartnerAddRemoveTagsVM val);

        Task<PartnerCustomerReportOutput> GetPartnerCustomerReport(PartnerCustomerReportInput val);

        Task<PartnerCustomerReportOutput> GetPartnerCustomerReportV2(PartnerCustomerReportInput val);
        Task<CustomerStatisticsOutput> GetCustomerStatistics(CustomerStatisticsInput val);
        Task<IEnumerable<AccountMove>> GetUnreconcileInvoices(Guid id, string search = "");
        Task<List<SearchAllViewModel>> SearchAll(PartnerPaged val);
        Task<PagedResult2<PartnerGetDebtPagedItem>> GetDebtPaged(Guid id, PartnerGetDebtPagedFilter val);
        Task<decimal> GetAmountAdvanceBalance(Guid id);
        Task<decimal> GetAmountAdvanceUsed(Guid id);

        Task<IEnumerable<PartnerBasic>> GetCustomerBirthDay(PartnerPaged val);
        Task<IEnumerable<PartnerBasic>> GetCustomerAppointments(PartnerPaged val);
        Task<IEnumerable<Guid>> GetPartnerForTCare(PartnerForTCarePaged val);
        Task<IEnumerable<PartnerSaleOrderDone>> GetPartnerOrderDone(PartnerPaged val);
        Task<PagedResult2<PartnerInfoDisplay>> GetPartnerInfoPaged(PartnerInfoPaged val);

    }
}
