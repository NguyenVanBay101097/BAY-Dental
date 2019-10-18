using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class ProductPricelistItemService : BaseService<ProductPricelistItem>, IProductPricelistItemService
    {
        public ProductPricelistItemService(IAsyncRepository<ProductPricelistItem> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }
    }
}
