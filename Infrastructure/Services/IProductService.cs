﻿using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IProductService: IBaseService<Product>
    {
        Task<PagedResult<Product>> GetPagedResultAsync(int pageIndex = 0, int pageSize = 20, string orderBy = "name", string orderDirection = "asc", string filter = "");
        Task<Product> GetProductForDisplayAsync(Guid id);
        Task<IEnumerable<ProductSimple>> GetProductsAutocomplete(string filter = "");
        Task<IEnumerable<ProductSimple>> GetProductsAutocomplete2(ProductPaged val);
        Task<Product> CreateProduct(ProductDisplay val);
        Task UpdateProduct(Guid id, ProductDisplay val);
        Task<ProductDisplay> GetProductDisplay(Guid id);
        Task<decimal> GetStandardPrice(Guid id);
        Task<PagedResult2<ProductBasic2>> GetPagedResultAsync(ProductPaged val);

        Task<ProductDisplay> DefaultGet();

        Task CreateProduct(IEnumerable<ProductDisplay> vals);

        Task<ProductDisplay> DefaultProductStepGet();
    }
}
