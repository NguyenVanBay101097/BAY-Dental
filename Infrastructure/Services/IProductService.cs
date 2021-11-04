﻿using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IProductService : IBaseService<Product>
    {
        Task<Product> GetProductForDisplayAsync(Guid id);
        Task<IEnumerable<ProductSimple>> GetProductsAutocomplete(string filter = "");
        Task<IEnumerable<ProductSimple>> GetProductsAutocomplete2(ProductPaged val);

        Task<Product> CreateProduct(ProductSave val);
        Task UpdateProduct(Guid id, ProductSave val);

        Task<ProductDisplay> GetProductDisplay(Guid id);
        double GetStandardPrice(Guid id, Guid? force_company_id = null);
        Task<PagedResult2<ProductBasic>> GetPagedResultAsync(ProductPaged val);

        Task<ProductDisplay> DefaultGet();

        Task<ProductDisplay> DefaultProductStepGet();

        Task<IDictionary<Guid, decimal>> _ComputeProductPrice(IEnumerable<Product> self,
            Guid pricelistId,
            Guid? partnerId = null, decimal quantity = 1,
            DateTime? date = null);
        Task<List<ProductServiceExportExcel>> GetServiceExportExcel(ProductPaged val);
        Task<PagedResult2<ProductLaboBasic>> GetLaboPagedResultAsync(ProductPaged val);
        Task<IDictionary<Guid, decimal>> GetQtyAvailableDict(IEnumerable<Guid> ids);
        Task<ProductDisplay> GetProductExport(Guid id);

        Task<IEnumerable<ProductProductExportExcel>> GetProductExportExcel(ProductPaged val);
        Task<IEnumerable<ProductProductExportExcel>> GetMedicineExportExcel(ProductPaged val);
        void SetStandardPrice(Product self, double value, Guid? force_company = null);
        Task<ProductDisplay> GetDefaultProductMedicine();
        Task<IEnumerable<ProductComingEnd>> GetProductsComingEnd(ProductGetProductsComingEndRequest val);
        void _ComputeUoMRels(IEnumerable<Product> self);
    }
}
