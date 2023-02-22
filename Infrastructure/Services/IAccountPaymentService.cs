﻿using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IAccountPaymentService : IBaseService<AccountPayment>
    {
        Task Post(IEnumerable<Guid> ids);
        Task<PagedResult2<AccountPaymentBasic>> GetPagedResultAsync(AccountPaymentPaged val);
        Task CancelAsync(IEnumerable<Guid> ids);
        Task UnlinkAsync(IEnumerable<Guid> ids);
        void _ComputeResidualPayment(IEnumerable<SaleOrder> saleOrders, decimal amount);
        Task<IEnumerable<AccountPaymentBasic>> GetPaymentBasicList(AccountPaymentFilter val);
        Task<AccountRegisterPaymentDisplay> SaleDefaultGet(IEnumerable<Guid> saleOrderIds);
        Task<AccountPayment> CreateUI(AccountPaymentSave val);
        Task<AccountRegisterPaymentDisplay> PartnerDefaultGet(Guid partnerId);
        Task<AccountRegisterPaymentDisplay> ServiceCardOrderDefaultGet(IEnumerable<Guid> order_ids);
        Task<AccountPaymentPrintVM> GetPrint(Guid id);
        Task<IEnumerable<AccountPayment>> CreateMultipleAndConfirmUI(IEnumerable<AccountPaymentSave> vals);
        Task<AccountRegisterPaymentDisplay> PurchaseDefaultGet(IEnumerable<Guid> purchaseOrderIds);
        Task<AccountRegisterPaymentDisplay> PartnerDefaultGetV2(PartnerDefaultSearch val);
        Task<AccountPaymentDisplay> ThuChiDefaultGet(AccountPaymentThuChiDefaultGetRequest val);
        Task<AccountPaymentDisplay> SalaryPaymentDefaultGet();
        Task<AccountRegisterPaymentDisplay> DefaultGet(IEnumerable<Guid> invoice_ids);

        Task<AccountRegisterPaymentDisplay> InsurancePaymentDefaultGet(IEnumerable<Guid> invoice_ids);
        Task _ComputeDestinationAccount(IEnumerable<AccountPayment> self);
    }
}
