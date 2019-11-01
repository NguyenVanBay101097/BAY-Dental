﻿using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISaleOrderService: IBaseService<SaleOrder>
    {
        Task<PagedResult<SaleOrder>> GetPagedResultAsync(int pageIndex = 0, int pageSize = 20, string orderBy = "name", string orderDirection = "asc", string filter = "");
        Task<SaleOrder> CreateOrderAsync(SaleOrder order);
        Task<SaleOrder> GetSaleOrderForDisplayAsync(Guid id);
        Task<SaleOrder> GetSaleOrderWithLines(Guid id);
        Task UpdateOrderAsync(SaleOrder order);
        Task<SaleOrder> GetSaleOrderByIdAsync(Guid id);
        Task UnlinkSaleOrderAsync(SaleOrder order);

        Task<SaleOrderLineDisplay> DefaultLineGet(SaleOrderLineDefaultGet val);
        Task<SaleOrderDisplay> DefaultGet();
        Task<PagedResult2<SaleOrderBasic>> GetPagedResultAsync(SaleOrderPaged val);
        Task ActionConfirm(IEnumerable<Guid> ids);
        Task ActionCancel(IEnumerable<Guid> ids);
        Task Unlink(IEnumerable<Guid> ids);
        Task<SaleOrderPrintVM> GetPrint(Guid id);
        Task ActionDone(IEnumerable<Guid> ids);
    }
}
