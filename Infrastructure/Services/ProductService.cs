﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly IMapper _mapper;
        public ProductService(IAsyncRepository<Product> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<Product> GetProductForDisplayAsync(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.Categ)
                .Include(x => x.UOM)
                .Include(x => x.UOMPO)
                .FirstOrDefaultAsync();
        }

        public async Task<PagedResult<Product>> GetPagedResultAsync(int pageIndex = 0, int pageSize = 20, string orderBy = "name", string orderDirection = "asc", string filter = "")
        {
            Expression<Func<Product, object>> sort = null;
            switch (orderBy)
            {
                case "name":
                    sort = x => x.Name;
                    break;
                default:
                    break;
            }

            var filterSpecification = new ProductFilterSpecification(filter: filter, orderBy: sort, orderDirection: orderDirection);
            var filterPaginatedSpecification = new ProductFilterSpecification(filter: filter, skip: pageIndex * pageSize, take: pageSize, isPagingEnabled: true, orderBy: sort, orderDirection: orderDirection);
            var items = await base.ListAsync(filterPaginatedSpecification);
            var totalItems = await base.CountAsync(filterSpecification);

            return new PagedResult<Product>(totalItems, pageIndex + 1, pageSize)
            {
                Items = items
            };
        }

        public async Task<PagedResult2<ProductBasic2>> GetPagedResultAsync(ProductPaged val)
        {
            var query = GetQueryPaged(val);
            var query2 = query.Select(x => new ProductBasic2
            {
                CategName = x.Categ.CompleteName,
                DefaultCode = x.DefaultCode,
                ListPrice = x.ListPrice,
                Name = x.Name,
                SaleOK = x.SaleOK,
                PurchaseOK = x.PurchaseOK,
                KeToaOK = x.KeToaOK,
                IsLabo = x.IsLabo,
                NameNoSign = x.NameNoSign,
                Type = x.Type,
                Id = x.Id,
                PurchasePrice = x.PurchasePrice ?? 0,
                CategId = x.CategId,
                QtyAvailable = x.StockQuants.Where(s => s.Location.Usage == "internal").Sum(s => s.Qty),
                Type2 = x.Type2
            });

            if (!string.IsNullOrEmpty(val.Search))
                query2 = query2.Where(x => x.Name.Contains(val.Search) || x.NameNoSign.Contains(val.Search) || x.DefaultCode.Contains(val.Search));
            if (val.CategId.HasValue)
                query2 = query2.Where(x => x.CategId == val.CategId);
            if (!string.IsNullOrEmpty(val.Type))
            {
                var types = val.Type.Split(",");
                query2 = query2.Where(x => types.Contains(x.Type));
            }

            //chung 1 group thì dùng toán tử or
            if (val.SaleOK.HasValue || val.PurchaseOK.HasValue || val.KeToaOK.HasValue || val.IsLabo.HasValue)
                query2 = query2.Where(x => x.SaleOK == val.SaleOK || x.PurchaseOK == val.PurchaseOK || x.KeToaOK == val.KeToaOK || x.IsLabo == val.IsLabo);

            if (!string.IsNullOrEmpty(val.Type2))
                query2 = query2.Where(x => x.Type2 == val.Type2);

            var items = await query2.OrderBy(x => x.Name).Skip(val.Offset).Take(val.Limit)
                .ToListAsync();

            var totalItems = await query2.CountAsync();

            return new PagedResult2<ProductBasic2>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<PagedResult2<ProductLaboBasic>> GetLaboPagedResultAsync(ProductPaged val)
        {
            var query = SearchQuery(x => x.Active && x.Type2 == "labo");
            if (!string.IsNullOrWhiteSpace(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.NameNoSign.Contains(val.Search));

            var items = await query.OrderBy(x => x.Name).Skip(val.Offset).Take(val.Limit)
                .Select(x => new ProductLaboBasic {
                    Id = x.Id,
                    Name = x.Name,
                    PurchasePrice = x.PurchasePrice
                })
                .ToListAsync();

            var totalItems = await query.CountAsync();

            return new PagedResult2<ProductLaboBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<IEnumerable<ProductBasic>> GetAutocompleteAsync(ProductPaged val)
        {
            var query = GetQueryPaged(val);

            var items = await query.Skip(val.Offset).Take(val.Limit)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductBasic>>(items);
        }

        private IQueryable<Product> GetQueryPaged(ProductPaged val)
        {
            var query = SearchQuery(x => x.Active);
            //if (!string.IsNullOrEmpty(val.Search))
            //    query = query.Where(x => x.Name.Contains(val.Search) || x.DefaultCode.Contains(val.Search));

            return query;
        }

        public async Task<IEnumerable<ProductSimple>> GetProductsAutocomplete(string filter = "")
        {
            return await SearchQuery(domain: x => x.Active && (string.IsNullOrEmpty(filter) || x.Name.Contains(filter)), orderBy: x => x.OrderBy(s => s.Name), limit: 10).Select(x => new ProductSimple
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
        }

        public async Task<IEnumerable<ProductSimple>> GetProductsAutocomplete2(ProductPaged val)
        {
           var query = SearchQuery(domain: x => x.Active && (string.IsNullOrEmpty(val.Search) || x.Name.Contains(val.Search) ||
            x.NameNoSign.Contains(val.Search)));
            if (val.KeToaOK.HasValue)
                query = query.Where(x => x.KeToaOK == val.KeToaOK);
            if (val.IsLabo.HasValue)
                query = query.Where(x => x.IsLabo == val.IsLabo);
            if (val.PurchaseOK.HasValue)
                query = query.Where(x => x.PurchaseOK == val.PurchaseOK);
            if (val.SaleOK.HasValue)
                query = query.Where(x => x.SaleOK == val.SaleOK);
            if (!string.IsNullOrEmpty(val.Type))
                query = query.Where(x => x.Type == val.Type);
            if (!string.IsNullOrEmpty(val.Type2))
                query = query.Where(x => x.Type2 == val.Type2);

            return await query.OrderBy(x => x.Name).Skip(val.Offset).Take(val.Limit).Select(x => new ProductSimple
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
        }

        public async Task<Product> CreateProduct(ProductDisplay val)
        {
            var product = _mapper.Map<Product>(val);
            product.NameNoSign = StringUtils.RemoveSignVietnameseV2(product.Name);
            var companyId = CompanyId;
            product.ProductCompanyRels = new List<ProductCompanyRel>()
            {
                new ProductCompanyRel
                {
                    CompanyId = companyId,
                    ProductId = product.Id,
                    StandardPrice = val.StandardPrice
                }
            };

            return await CreateAsync(product);
        }

        public async Task<Product> CreateProduct(ProductLaboSave val)
        {
            var product = _mapper.Map<Product>(val);
            product.NameNoSign = StringUtils.RemoveSignVietnameseV2(product.Name);
            product.Type2 = "labo";
            product.SaleOK = false;

            var companyId = CompanyId;
            product.CompanyId = companyId;

            var uomObj = GetService<IUoMService>();
            var productCategObj = GetService<IProductCategoryService>();
            var uom = await uomObj.DefaultUOM();
            if (uom == null)
                throw new Exception("Not found default uom");
            product.UOMId = uom.Id;
            product.UOMPOId = uom.Id;

            var categ = await productCategObj.SearchQuery(x => x.Name == "Labo" && x.Type == "labo").FirstOrDefaultAsync();
            if (categ == null)
            {
                categ = new ProductCategory() { Name = "Labo", Type = "labo" };
                await productCategObj.CreateAsync(categ);
            }
            product.CategId = categ.Id;

            return await CreateAsync(product);
        }

        public async Task CreateProduct(IEnumerable<ProductDisplay> vals)
        {
            var self = new List<Product>();
            foreach(var val in vals)
            {
                var product = _mapper.Map<Product>(val);
                product.NameNoSign = StringUtils.RemoveSignVietnameseV2(product.Name);
                var companyId = CompanyId;
                product.ProductCompanyRels = new List<ProductCompanyRel>()
                {
                    new ProductCompanyRel
                    {
                        CompanyId = companyId,
                        ProductId = product.Id,
                        StandardPrice = val.StandardPrice
                    }
                };
                self.Add(product);
            }

            await CreateAsync(self);
        }

        public async Task UpdateProduct(Guid id, ProductDisplay val)
        {
            var product = await SearchQuery(x => x.Id == id).Include(x => x.ProductCompanyRels).FirstOrDefaultAsync();
            var companyId = CompanyId;
            product = _mapper.Map(val, product);
            product.NameNoSign = StringUtils.RemoveSignVietnameseV2(product.Name);

            if (product.ProductCompanyRels == null)
            {
                product.ProductCompanyRels = new List<ProductCompanyRel>()
                {
                    new ProductCompanyRel
                    {
                        CompanyId = companyId,
                        ProductId = product.Id,
                        StandardPrice = val.StandardPrice
                    }
                };
            }
            else
            {
                var pcRel = product.ProductCompanyRels.Where(x => x.CompanyId == companyId).FirstOrDefault();
                if (pcRel == null)
                {
                    product.ProductCompanyRels.Add(new ProductCompanyRel
                    {
                        CompanyId = companyId,
                        ProductId = product.Id,
                        StandardPrice = val.StandardPrice
                    });
                }
                else
                {
                    pcRel.StandardPrice = val.StandardPrice;
                }
            }

            await UpdateAsync(product);
        }

        public async Task UpdateProduct(Guid id, ProductLaboSave val)
        {
            var product = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            product = _mapper.Map(val, product);
            product.NameNoSign = StringUtils.RemoveSignVietnameseV2(product.Name);

            await UpdateAsync(product);
        }

        public async Task<ProductDisplay> GetProductDisplay(Guid id)
        {
            var product = await SearchQuery(x => x.Id == id).Include(x => x.ProductCompanyRels)
                .Include(x => x.Categ)
                .Include(x => x.UOM)
                .Include(x => x.UOMPO).FirstOrDefaultAsync();
            var res = _mapper.Map<ProductDisplay>(product);
            var companyId = CompanyId;
            var pcRel = product.ProductCompanyRels.FirstOrDefault(x => x.CompanyId == companyId);
            if (pcRel != null)
            {
                res.StandardPrice = pcRel.StandardPrice;
            }

            return res;
        }

        public async Task<decimal> GetStandardPrice(Guid id)
        {
            var product = await SearchQuery(x => x.Id == id).Include(x => x.ProductCompanyRels).FirstOrDefaultAsync();
            var companyId = CompanyId;
            var pcRel = product.ProductCompanyRels.FirstOrDefault(x => x.CompanyId == companyId);
            if (pcRel != null)
            {
                return pcRel.StandardPrice;
            }

            return 0;
        }

        public async Task<ProductDisplay> DefaultGet()
        {
            var uomObj = GetService<IUoMService>();
            var productCategObj = GetService<IProductCategoryService>();
            var uom = await uomObj.DefaultUOM();
            //var categ = await productCategObj.DefaultCategory();
            var res = new ProductDisplay();
            if (uom != null)
            {
                res.UOMId = uom.Id;
                res.UOMPOId = uom.Id;
            }

            res.CompanyId = CompanyId;

            //if (categ != null)
            //{
            //    res.Categ = _mapper.Map<ProductCategoryBasic>(categ);
            //}

            return res;
        }

        public async Task<ProductDisplay> DefaultProductStepGet()
        {
            var uomObj = GetService<IUoMService>();
            var productCategObj = GetService<IProductCategoryService>();
            var uom = await uomObj.DefaultUOM();
            var categ = await productCategObj.SearchQuery(x => x.StepCateg).FirstOrDefaultAsync();
            var res = new ProductDisplay();
            if (uom != null)
            {
                res.UOMId = uom.Id;
                res.UOMPOId = uom.Id;
            }

            res.CompanyId = CompanyId;

            if (categ == null)
            {
                categ = await productCategObj.CreateAsync(new ProductCategory
                {
                    Name = "Công đoạn",
                    StepCateg = true
                });
            }

            res.CategId = categ.Id;

            return res;
        }


        public async Task<IDictionary<Guid, decimal>> _ComputeProductPrice(IEnumerable<Product> self,
            Guid? partnerId = null, decimal quantity = 1,
            DateTime? date = null)
        {
            var res = new Dictionary<Guid, decimal>();
            foreach (var product in self)
            {
                res.Add(product.Id, 0);
            }

            var plobj = GetService<IProductPricelistService>();
            var qtys = new List<ProductQtyByPartner>();
            foreach (var product in self)
            {
                qtys.Add(new ProductQtyByPartner
                {
                    Product = product,
                    Quantity = quantity,
                    PartnerId = partnerId
                });
            }
            var prices = await plobj._ComputePriceRule(qtys, date: date);
            foreach (var product in self)
            {
                if (prices.ContainsKey(product.Id))
                    res[product.Id] = prices[product.Id].Price;
            }

            return res;
        }

        //public override ISpecification<Product> RuleDomainGet(IRRule rule)
        //{
        //    var companyId = CompanyId;
        //    switch (rule.Code)
        //    {
        //        case "product_comp_rule":
        //            return new InitialSpecification<Product>(x => x.CompanyId == companyId);
        //        default:
        //            return null;
        //    }
        //}
    }
}
