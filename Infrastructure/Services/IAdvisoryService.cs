﻿using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;


namespace Infrastructure.Services
{
    public interface IAdvisoryService : IBaseService<Advisory>
    {
        Task<PagedResult2<AdvisoryBasic>> GetPagedResultAsync(AdvisoryPaged val);
        Task<AdvisoryDisplay> GetAdvisoryDisplay(Guid id);
        Task<PagedResult2<AdvisoryLine>> GetAdvisoryLines(AdvisoryLinePaged val);
        Task<Advisory> CreateAdvisory(AdvisorySave val);
        Task UpdateAdvisory(Guid id, AdvisorySave val);
        Task RemoveAdvisory(Guid id);
        Task<AdvisoryDisplay> DefaultGet(AdvisoryDefaultGet val);
        Task<ToothAdvised> GetToothAdvise(AdvisoryToothAdvise val);
        Task<AdvisoryPrintVM> Print(IEnumerable<Guid> ids);
        Task<SaleOrderSimple> CreateSaleOrder(CreateFromAdvisoryInput val);
        Task<QuotationSimple> CreateQuotation(CreateFromAdvisoryInput val);
    }
}
