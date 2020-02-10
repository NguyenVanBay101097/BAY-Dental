using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class ProductPriceHistoryService : BaseService<ProductPriceHistory>, IProductPriceHistoryService
    {
        public ProductPriceHistoryService(IAsyncRepository<ProductPriceHistory> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }
    }
}
