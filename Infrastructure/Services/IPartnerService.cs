﻿using ApplicationCore.Entities;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IPartnerService: IBaseService<Partner>
    {
        Task<PagedResult2<Partner>> GetPagedResultAsync(int offset = 0, int limit = 20, string search = "", string searchBy = "", bool? customer = null);
        Task<PagedResult2<PartnerBasic>> GetPagedResultAsync(PartnerPaged val);
        Task<Partner> GetPartnerForDisplayAsync(Guid id);
        Task<IEnumerable<PartnerSimple>> SearchAutocomplete(string filter = "", bool? customer = null);
        Task<IEnumerable<PartnerSimple>> SearchPartnersCbx(PartnerPaged val);
        string GetFormatAddress(Partner partner);
        string GetGenderDisplay(Partner partner);
        Task<string> UploadImage(IFormFile file);
        Task<PartnerInfoViewModel> GetInfo(Guid id);
        Task<PagedResult2<AccountInvoiceDisplay>> GetCustomerInvoices(AccountInvoicePaged val);
        Dictionary<Guid, PartnerCreditDebitItem> CreditDebitGet(IEnumerable<Guid> ids = null,
       DateTime? fromDate = null,
       DateTime? toDate = null);
    }
}
