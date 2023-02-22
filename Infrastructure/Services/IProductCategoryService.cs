using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IProductCategoryService: IBaseService<ProductCategory>
    {
        Task<PagedResult<ProductCategory>> GetPagedResultAsync(int pageIndex = 0, int pageSize = 20,
            string orderBy = "name",
            string orderDirection = "asc",
            string filter = "");

        Task<ProductCategory> DefaultCategory();
        Task<ProductCategory> GetCategoryForDisplay(Guid id);
        Task<IEnumerable<ProductCategoryBasic>> GetAutocompleteAsync(ProductCategoryPaged val);
        Task<PagedResult2<ProductCategoryBasic>> GetPagedResultAsync(ProductCategoryPaged val);
        Task Write(ProductCategory self);

        Task<ProductCategory> CreateCategByCompleteName(string completeName);
    }
}
