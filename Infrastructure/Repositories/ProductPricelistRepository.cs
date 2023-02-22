using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class ProductPricelistRepository : EfRepository<ProductPricelist>, IProductPricelistRepository
    {
        public ProductPricelistRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}
