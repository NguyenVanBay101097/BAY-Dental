using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IProductPricelistService: IBaseService<ProductPricelist>
    {
        Task<PagedResult2<ProductPricelistBasic>> GetPagedResultAsync(ProductPricelistPaged val);
        Task<ProductPricelist> GetPriceListForUpdate(Guid id);
        Task<IDictionary<Guid, ComputePriceRuleResValue>> _ComputePriceRule(IEnumerable<ProductQtyByPartner> products_qty_partner,
             DateTime? date = null);
    }
}
