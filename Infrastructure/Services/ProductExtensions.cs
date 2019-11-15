using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Services
{
    public static class ProductExtensions
    {
        public static double StandardPrice(this Product product, Guid companyId)
        {
            var rel = product.ProductCompanyRels.FirstOrDefault(x => x.CompanyId == companyId);
            if (rel == null)
                return 0;
            return rel.StandardPrice;
        }

        public static void SetStandardPrice(this Product product, double price, Guid companyId)
        {
            var rel = product.ProductCompanyRels.FirstOrDefault(x => x.CompanyId == companyId);
            if (rel == null)
                product.ProductCompanyRels.Add(new ProductCompanyRel { CompanyId = companyId, StandardPrice = price });
            rel.StandardPrice = price;
        }
    }
}
