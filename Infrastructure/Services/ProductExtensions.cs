using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            else
                rel.StandardPrice = price;
        }

        public static async Task<decimal> GetListPrice(this Product self, IProductService productService, IIrConfigParameterService irConfigParameter,
            IIRPropertyService propertyObj)
        {
            var value = await irConfigParameter.GetParam("product.listprice_restrict_company");
            if (string.IsNullOrEmpty(value) || !Convert.ToBoolean(value))
                return self.ListPrice;

            var val = propertyObj.get("list_price", "product.product", res_id: $"product.product,{self.Id.ToString()}");
            return Convert.ToDecimal(val);
        }
    }
}
