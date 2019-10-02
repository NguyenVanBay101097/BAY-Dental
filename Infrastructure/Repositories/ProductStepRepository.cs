using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class ProductStepRepository : EfRepository<ProductStep>, IProductStepRepository
    {
        public ProductStepRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        public void GetAllByProductId(Guid productId)
        {
        }
    }


}
