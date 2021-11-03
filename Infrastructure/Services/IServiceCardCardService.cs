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
    public interface IServiceCardCardService : IBaseService<ServiceCardCard>
    {
        Task ActionActive(IEnumerable<Guid> ids);
        Task<PagedResult2<ServiceCardCardBasic>> GetPagedResultAsync(ServiceCardCardPaged val);
        void _ComputeResidual(IEnumerable<ServiceCardCard> self);
        Task<IEnumerable<ServiceCardCard>> _ComputeResidual(IEnumerable<Guid> ids);
        Task ButtonConfirm(IEnumerable<ServiceCardCard> self);
        Task Unlink(IEnumerable<Guid> ids);
        Task<ServiceCardCard> CheckCode(string code);

        Task ActionLock(IEnumerable<Guid> ids);
        Task ActionCancel(IEnumerable<Guid> ids);

        CheckPromoCodeMessage _CheckServiceCardCardApplySaleLine(ServiceCardCard self, SaleOrderLine line);
        Task<ImportExcelResponse> ActionImport(IFormFile formFile);
    }
}
