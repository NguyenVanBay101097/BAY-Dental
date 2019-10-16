using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IProductPricelistService
    {
        Task<PagedResult2<ProductPricelistBasic>> GetPagedResultAsync(ProductPricelistPaged val);
        Task<ProductPricelist> CreateAsync(ProductPricelist pricelist);
        Task<ProductPricelist> GetPriceListForUpdate(Guid id);
        Task UpdateAsync(ProductPricelist pricelist);
        Task<ProductPricelist> GetByIdAsync(Guid id);
        Task DeleteAsync(ProductPricelist pricelist);

        Task<IDictionary<Guid, ComputePriceRuleResValue>> _ComputePriceRule(IEnumerable<ProductQtyByPartner> products_qty_partner,
             DateTime? date = null);
    }
}
