﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyERP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly IMapper _mapper;
        private readonly IUoMService _uoMService;
        private readonly CatalogDbContext _context;
        public ProductService(
            IAsyncRepository<Product> repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IUoMService uoMService,
            CatalogDbContext context
            )
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _uoMService = uoMService;
            _context = context;
        }

        public override async Task<IEnumerable<Product>> CreateAsync(IEnumerable<Product> entities)
        {
            _SetNameNoSign(entities);

            await _GenerateCodeIfEmpty(entities);

            await base.CreateAsync(entities);

            await _CheckProductExistCode(entities);

            await _SetListPrice(entities);

            return entities;
        }

        private void _SetNameNoSign(IEnumerable<Product> self)
        {
            foreach (var product in self)
                product.NameNoSign = string.Join("|", product.Name, product.Name.RemoveSignVietnameseV2(), product.Name.GetAllFirstChar());
        }

        public override async Task UpdateAsync(IEnumerable<Product> entities)
        {
            _SetNameNoSign(entities);

            await _GenerateCodeIfEmpty(entities);

            await base.UpdateAsync(entities);

            await _CheckProductExistCode(entities);

            await _SetListPrice(entities);
        }

        /// <summary>
        /// Kiểm tra xem nếu có sản phẩm đã tồn tại mã thì báo lỗi
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        private async Task _CheckProductExistCode(IEnumerable<Product> self)
        {
            var exists = await _GetProductsExistCode(self);
            if (exists.Any())
                throw new Exception($"Đã tồn tại sản phẩm với mã {string.Join(", ", exists.Select(x => x.DefaultCode))}");
        }

        private async Task _GenerateCodeIfEmpty(IEnumerable<Product> self)
        {
            //generate ma code duy nhat
            var seqObj = GetService<IIRSequenceService>();
            foreach (var product in self)
            {
                if (!string.IsNullOrWhiteSpace(product.DefaultCode))
                    continue;

                do
                {
                    product.DefaultCode = await seqObj.NextByCode("product_seq");

                    if (string.IsNullOrWhiteSpace(product.DefaultCode))
                    {
                        await _InsertProductSequence();
                        product.DefaultCode = await seqObj.NextByCode("product_seq");
                    }
                } while (SearchQuery(x => x.DefaultCode == product.DefaultCode).Any());
            }
        }

        /// <summary>
        /// Tìm những product đã tồn tại mã trong db
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        private async Task<IEnumerable<Product>> _GetProductsExistCode(IEnumerable<Product> self)
        {
            var list = new List<Product>();
            foreach (var product in self)
            {
                if (string.IsNullOrWhiteSpace(product.DefaultCode))
                    continue;

                var count = await SearchQuery(x => x.DefaultCode == product.DefaultCode).CountAsync();
                if (count >= 2)
                    list.Add(product);
            }

            return list;
        }

        private async Task _InsertProductSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Name = "Product Sequence",
                Code = "product_seq",
                Prefix = "SP",
                Padding = 4
            });
        }

        private async Task _SetListPrice(IEnumerable<Product> self)
        {
            var irConfigParameter = GetService<IIrConfigParameterService>();
            var value = await irConfigParameter.GetParam("product.listprice_restrict_company");
            if (string.IsNullOrEmpty(value) || !Convert.ToBoolean(value))
                return;

            var propertyObj = GetService<IIRPropertyService>();
            var list = self;
            propertyObj.set_multi("list_price", "product.product", list.ToDictionary(x => string.Format("product.product,{0}", x.Id), x => (object)x.ListPrice));
        }

        public async Task<Product> GetProductForDisplayAsync(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.Categ)
                .Include(x => x.UOM)
                .Include(x => x.UOMPO)
                .FirstOrDefaultAsync();
        }

        public override ISpecification<Product> RuleDomainGet(IRRule rule)
        {
            switch (rule.Code)
            {
                case "product.product_comp_rule":
                    return new InitialSpecification<Product>(x => !x.CompanyId.HasValue || x.CompanyId == CompanyId);
                default:
                    return null;
            }
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

        public async Task<PagedResult2<ProductBasic>> GetPagedResultAsync(ProductPaged val)
        {
            var query = GetQueryPaged(val);

            if (val.Type2 == "labo" || val.Type2 == "labo_attach")
                query = query.OrderByDescending(x => x.DateCreated);
            else
                query = query.OrderBy(x => x.Name);

            var totalItems = await query.CountAsync();
            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var items = await _mapper.ProjectTo<ProductBasic>(query).ToListAsync();

            //Tính lại giá bán
            await _ProcessListPrice(items);

            //Tính tồn kho
            _CalcQtyAvailable(items);

            return new PagedResult2<ProductBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<List<ProductServiceExportExcel>> GetServiceExportExcel(ProductPaged val)
        {
            var query = SearchQuery(x => x.Active && x.Type2 == "service");
            if (!string.IsNullOrWhiteSpace(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            if (val.CategId.HasValue)
                query = query.Where(x => x.CategId == val.CategId);
            var products = await query.OrderBy(x => x.Name).Skip(val.Offset).Take(val.Limit)
                .Include(x => x.ProductCompanyRels)
                .Include(x => x.Categ)
                .Include(x => x.Steps)
                .ToListAsync();

            //check listPrice product
            foreach (var product in products)
                product.ListPrice = await _GetListPrice(product);

            var res = products.Select(x => new ProductServiceExportExcel
            {
                Id = x.Id,
                CategName = x.Categ.Name,
                DefaultCode = x.DefaultCode,
                Name = x.Name,
                IsLabo = x.IsLabo,
                ListPrice = x.ListPrice,
                LaboPrice = x.LaboPrice,
                PurchasePrice = x.PurchasePrice,
                Firm = x.Firm,
                StandardPrice = Convert.ToDecimal(_GetStandardPrice(x.Id)),
                StepList = x.Steps.Select(s => new ProductStepSimple
                {
                    Id = s.Id,
                    Name = s.Name
                })
            }).ToList();

            return res;

        }

        public async Task<IEnumerable<ProductComingEnd>> GetProductsComingEnd(ProductGetProductsComingEndRequest val)
        {
            var quantObj = GetService<IStockQuantService>();
            var productQuery = SearchQuery(x => x.Active && x.Type == "product");
            var quantQuery = quantObj.SearchQuery(x => x.Location.Usage == "internal" && (!val.CompanyId.HasValue || x.CompanyId == val.CompanyId))
                .GroupBy(x => x.ProductId)
                .Select(x => new
                {
                    ProductId = x.Key,
                    QtyAvailable = x.Sum(x => x.Qty)
                });

            var result = from p in productQuery
                         from q in quantQuery.Where(x => x.ProductId == p.Id).DefaultIfEmpty()
                         where q.QtyAvailable < p.MinInventory
                         select new ProductComingEnd
                         {
                             Id = p.Id,
                             Inventory = q.QtyAvailable,
                             MinInventory = p.MinInventory,
                             Name = p.Name,
                             PurchasePrice = p.PurchasePrice,
                             Type2 = p.Type2,
                             NameNoSign = p.NameNoSign
                         };

            return await result.ToListAsync();

        }

        private void _CalcQtyAvailable(IEnumerable<ProductBasic> items)
        {
            var compute_items = items.Where(x => x.Type == "product");
            if (!compute_items.Any())
                return;

            var company_id = CompanyId;
            var qty_available_dict = ProductAvailable(ids: compute_items.Select(x => x.Id).ToList(), company_id: company_id);

            foreach (var item in compute_items)
                item.QtyAvailable = qty_available_dict[item.Id].QtyAvailable;
        }

        private async Task _ProcessListPriceExport(List<ProductServiceExportExcel> items)
        {
            var userObj = GetService<IUserService>();
            var hasGroup = await userObj.HasGroup("base.group_multi_company");
            if (!hasGroup)
                return;

            var modelDataObj = GetService<IIRModelDataService>();
            var productRule = await modelDataObj.GetRef<IRRule>("product.product_comp_rule");
            var companyShareProduct = !productRule.Active;
            if (!companyShareProduct)
                return;

            var irConfigParameter = GetService<IIrConfigParameterService>();
            var value = await irConfigParameter.GetParam("product.listprice_restrict_company");
            if (string.IsNullOrEmpty(value) || !Convert.ToBoolean(value))
                return;

            var propertyObj = GetService<IIRPropertyService>();
            var dict = propertyObj.get_multi("list_price", "product.product", items.Select(x => x.Id.ToString()).ToList());
            foreach (var item in items)
                item.ListPrice = Convert.ToDecimal(dict[item.Id.ToString()]);
        }
        private void _CalcQtyAvailableExport(IEnumerable<ProductServiceExportExcel> items)
        {
            var compute_items = items.Where(x => x.Type == "product");
            if (!compute_items.Any())
                return;

            var company_id = CompanyId;
            var qty_available_dict = ProductAvailable(ids: compute_items.Select(x => x.Id).ToList(), company_id: company_id);

            foreach (var item in compute_items)
                item.QtyAvailable = qty_available_dict[item.Id].QtyAvailable;
        }

        private async Task _ProcessListPrice(List<ProductBasic> items)
        {
            var userObj = GetService<IUserService>();
            var hasGroup = await userObj.HasGroup("base.group_multi_company");
            if (!hasGroup)
                return;

            var modelDataObj = GetService<IIRModelDataService>();
            var productRule = await modelDataObj.GetRef<IRRule>("product.product_comp_rule");
            var companyShareProduct = !productRule.Active;
            if (!companyShareProduct)
                return;

            var irConfigParameter = GetService<IIrConfigParameterService>();
            var value = await irConfigParameter.GetParam("product.listprice_restrict_company");
            if (string.IsNullOrEmpty(value) || !Convert.ToBoolean(value))
                return;

            var propertyObj = GetService<IIRPropertyService>();
            var dict = propertyObj.get_multi("list_price", "product.product", items.Select(x => x.Id.ToString()).ToList());
            foreach (var item in items)
                item.ListPrice = Convert.ToDecimal(dict[item.Id.ToString()]);
        }

        public async Task<IDictionary<Guid, decimal>> GetQtyAvailableDict(IEnumerable<Guid> ids)
        {
            var res = ids.ToDictionary(x => x, x => 0M);
            var quantObj = GetService<IStockQuantService>();
            var data = await quantObj.SearchQuery(x => ids.Contains(x.ProductId) && x.Location.Usage == "internal")
                .GroupBy(x => x.ProductId)
                .Select(x => new
                {
                    ProductId = x.Key,
                    QtyAvailable = x.Sum(s => s.Qty)
                }).ToListAsync();
            foreach (var item in data)
                res[item.ProductId] = item.QtyAvailable;
            return res;
        }

        public async Task<PagedResult2<ProductLaboBasic>> GetLaboPagedResultAsync(ProductPaged val)
        {
            var query = SearchQuery(x => x.Active && x.Type2 == "labo");
            if (!string.IsNullOrWhiteSpace(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.NameNoSign.Contains(val.Search));

            var items = await query.OrderBy(x => x.Name).OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit)
                .Select(x => new ProductLaboBasic
                {
                    Id = x.Id,
                    Name = x.Name,
                    PurchasePrice = x.PurchasePrice,
                    LaboPrice = x.LaboPrice
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

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.NameNoSign.Contains(val.Search) || x.DefaultCode.Contains(val.Search));

            if (val.CategId.HasValue)
                query = query.Where(x => x.CategId == val.CategId);

            if (!string.IsNullOrEmpty(val.Type))
            {
                var types = val.Type.Split(",");
                query = query.Where(x => types.Contains(x.Type));
            }

            if (!string.IsNullOrEmpty(val.Type2))
            {
                var types = val.Type2.Split(",");
                query = query.Where(x => types.Contains(x.Type2));
            }

            if (val.PurchaseOK.HasValue)
                query = query.Where(x => x.PurchaseOK == val.PurchaseOK);

            if (val.SaleOK.HasValue)
                query = query.Where(x => x.SaleOK == val.SaleOK);

            return query;
        }

        public async Task<IEnumerable<ProductSimple>> GetProductsAutocomplete(string filter = "")
        {
            return await SearchQuery(domain: x => x.Active && (string.IsNullOrEmpty(filter) || x.Name.Contains(filter) ||
            x.NameNoSign.Contains(filter) || x.DefaultCode.Contains(filter)), orderBy: x => x.OrderBy(s => s.Name), limit: 10)
                .Select(x => new ProductSimple
                {
                    Id = x.Id,
                    Name = x.Name,
                    ListPrice = x.ListPrice
                }).ToListAsync();
        }

        public async Task<IEnumerable<ProductSimple>> GetProductsAutocomplete2(ProductPaged val)
        {
            var query = SearchQuery(x => x.Active);

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.NameNoSign.Contains(val.Search));
            if (val.KeToaOK.HasValue)
                query = query.Where(x => x.KeToaOK == val.KeToaOK);
            if (val.IsLabo.HasValue)
                query = query.Where(x => x.IsLabo == val.IsLabo);
            if (val.PurchaseOK.HasValue)
                query = query.Where(x => x.PurchaseOK == val.PurchaseOK);
            if (val.SaleOK.HasValue)
                query = query.Where(x => x.SaleOK == val.SaleOK);
            if (!string.IsNullOrEmpty(val.Type))
            {
                var types = val.Type.Split(",");
                query = query.Where(x => types.Contains(x.Type));
            }

            if (!string.IsNullOrEmpty(val.Type2))
            {
                var types = val.Type2.Split(",");
                query = query.Where(x => types.Contains(x.Type2));
            }

            query = query.OrderBy(x => x.Name);
            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);
            var items = await query.Include(x => x.UOM).ToListAsync();

            var res = _mapper.Map<IEnumerable<ProductSimple>>(items);

            foreach (var item in res.Where(x => x.Type2 == "medicine"))
                item.StandardPrice = _GetStandardPrice(item.Id);

            return res;
        }

        public async Task<Product> CreateProduct(ProductDisplay val)
        {
            var product = _mapper.Map<Product>(val);
            if (!product.CompanyId.HasValue)
                product.CompanyId = CompanyId;
            if (val.StepList.Any())
            {
                var order = 1;
                foreach (var step in val.StepList)
                {
                    product.Steps.Add(new ProductStep
                    {
                        Order = order++,
                        Name = step.Name
                    });
                }
            }

            //_SetStandardPrice(product, val.StandardPrice);

            await _SetListPrice(product, val.ListPrice);

            return await CreateAsync(product);
        }

        public void SetStandardPrice(Product self, double value, Guid? force_company = null)
        {
            //Store the standard price change in order to be able to retrieve the cost of a product for a given date'''
            var list = new List<Product>() { self };
            var propertyObj = GetService<IIRPropertyService>();

            //Store the standard price change in order to be able to retrieve the cost of a product for a given date'''
            propertyObj.set_multi("standard_price", "product.product", list.ToDictionary(x => string.Format("product.product,{0}", x.Id), x => (object)value), force_company: force_company);

            var priceHistoryObj = GetService<IProductPriceHistoryService>();
            priceHistoryObj.Create(new ProductPriceHistory
            {
                ProductId = self.Id,
                Cost = value,
                CompanyId = CompanyId
            });
        }

        public async Task UpdateProduct(Guid id, ProductDisplay val)
        {
            var product = await SearchQuery(x => x.Id == id).Include(x => x.Steps).FirstOrDefaultAsync();
            var companyId = CompanyId;
            product = _mapper.Map(val, product);
            product.NameNoSign = StringUtils.RemoveSignVietnameseV2(product.Name);

            _SaveProductSteps(product, val.StepList);
            await UpdateAsync(product);
        }

        public async Task<Product> CreateProduct(ProductSave val)
        {
            var product = _mapper.Map<Product>(val);
            //if (string.IsNullOrEmpty(product.DefaultCode))
            //{
            //    product.DefaultCode = await GenerateCodeIfEmpty();
            //}
            //else
            //{
            //    if (await IsExistProductCode(product.DefaultCode))
            //        throw new Exception($"Đã tồn tại sản phầm với mã {val.DefaultCode}");
            //}

            _SaveProductSteps(product, val.StepList);

            _SaveProductBoms(product, val.Boms);

            _SaveUoMRels(product, val);

            UpdateProductCriteriaRel(product, val);

            product = await CreateAsync(product);

            SetStandardPrice(product, val.StandardPrice, force_company: product.CompanyId);
            //if (IsCorrectFormat(product.DefaultCode))
            //    await UpdateNextCode(product.DefaultCode);
            return product;
        }

        public async Task UpdateProduct(Guid id, ProductSave val)
        {
            var product = await SearchQuery(x => x.Id == id).Include(x => x.Steps).Include(x => x.Boms)
                .Include(x => x.ProductUoMRels).Include(x => x.ProductStockInventoryCriteriaRels).FirstOrDefaultAsync();

            product = _mapper.Map(val, product);
            //if (string.IsNullOrEmpty(product.DefaultCode))
            //{
            //    product.DefaultCode = await GenerateCodeIfEmpty();
            //}
            //else
            //{
            //    if (await IsExistProductCode(product.DefaultCode))
            //        throw new Exception($"Đã tồn tại sản phầm với mã ${val.DefaultCode}");
            //}
            product.NameNoSign = StringUtils.RemoveSignVietnameseV2(product.Name);

            _SaveProductSteps(product, val.StepList);

            _SaveProductBoms(product, val.Boms);

            _SaveUoMRels(product, val);

            SetStandardPrice(product, val.StandardPrice, force_company: product.CompanyId);

            await _SetListPrice(product, val.ListPrice);

            UpdateProductCriteriaRel(product, val);

            await UpdateAsync(product);
            //if (IsCorrectFormat(product.DefaultCode))
            //    await UpdateNextCode(product.DefaultCode);
        }

        private void UpdateProductCriteriaRel(Product product, ProductSave val)
        {
            var toRemove = product.ProductStockInventoryCriteriaRels.Where(x => !val.ProductCriteriaIds.Contains(x.StockInventoryCriteriaId)).ToList();
            foreach (var hist in toRemove)
            {
                product.ProductStockInventoryCriteriaRels.Remove(hist);
            }
            if (val.ProductCriteriaIds != null)
            {
                foreach (var hist in val.ProductCriteriaIds)
                {
                    if (product.ProductStockInventoryCriteriaRels.Any(x => x.StockInventoryCriteriaId == hist))
                        continue;

                    product.ProductStockInventoryCriteriaRels.Add(new ProductStockInventoryCriteriaRel
                    {
                        StockInventoryCriteriaId = hist
                    });

                }
            }
        }

        private void _SaveUoMRels(Product product, ProductSave val)
        {
            var rels_remove = new List<ProductUoMRel>();
            var uom_ids = new List<Guid>() { val.UOMId, val.UOMPOId };
            foreach (var rel in product.ProductUoMRels)
            {
                if (!uom_ids.Contains(rel.UoMId))
                    rels_remove.Add(rel);
            }

            foreach (var rel in rels_remove)
                product.ProductUoMRels.Remove(rel);

            foreach (var uom_id in uom_ids)
            {
                if (!product.ProductUoMRels.Any(x => x.UoMId == uom_id))
                    product.ProductUoMRels.Add(new ProductUoMRel() { UoMId = uom_id });
            }
        }

        private void _SaveProductSteps(Product product, IEnumerable<ProductStepDisplay> val)
        {
            var existLines = product.Steps.ToList();
            var lineToRemoves = new List<ProductStep>();
            foreach (var existLine in existLines)
            {
                bool found = false;
                foreach (var item in val)
                {
                    if (item.Id == existLine.Id)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
                product.Steps.Remove(line);

            int sequence = 1;
            foreach (var line in val)
            {
                if (line.Id == Guid.Empty)
                {
                    product.Steps.Add(new ProductStep
                    {
                        Order = sequence++,
                        Name = line.Name
                    });
                }
                else
                {
                    var sl = product.Steps.SingleOrDefault(c => c.Id == line.Id);
                    if (sl != null)
                    {
                        sl.Order = sequence++;
                        sl.Name = line.Name;
                    }
                }
            }
        }

        private void _SaveProductBoms(Product product, IEnumerable<ProductBomSave> val)
        {
            var existBoms = product.Boms.ToList();
            var ToRemoves = new List<ProductBom>();

            var valIds = val.Select(x => x.Id).ToList();
            ToRemoves = existBoms.Where(x => !valIds.Contains(x.Id)).ToList();

            foreach (var bom in ToRemoves)
                product.Boms.Remove(bom);

            int sequence = 0;
            foreach (var line in val)
            {
                if (line.Id == Guid.Empty)
                {
                    var item = _mapper.Map<ProductBom>(line);
                    item.Sequence = sequence++;
                    product.Boms.Add(item);
                }
                else
                {
                    var item = product.Boms.SingleOrDefault(c => c.Id == line.Id);
                    if (item != null)
                    {
                        _mapper.Map(line, item);
                        item.Sequence = sequence++;
                    }
                }

            }
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
            var criteriaObj = GetService<IStockInventoryCriteriaService>();
            //tách ra nhiều câu query nhỏ hơn và map view model trả về
            var product = await SearchQuery(x => x.Id == id)
                .Include(x => x.Categ)
                .Include(x => x.UOM)
                .Include(x => x.UOMPO)
                .Include(x => x.ProductStockInventoryCriteriaRels)
                .ThenInclude(x => x.StockInventoryCriteria)
                .FirstOrDefaultAsync();
            var res = _mapper.Map<ProductDisplay>(product);

            var bomObj = GetService<IProductBomService>();
            var boms = await bomObj.SearchQuery(x => x.ProductId == id, orderBy: x => x.OrderBy(s => s.Sequence))
                .Include(x => x.MaterialProduct)
                .Include(x => x.ProductUOM)
                .ToListAsync();

            res.Boms = _mapper.Map<IEnumerable<ProductBomBasic>>(boms);

            res.StandardPrice = _GetStandardPrice(product.Id);
            res.ListPrice = await _GetListPrice(product);

            return res;
        }

        public async Task<ProductDisplay> GetProductExport(Guid id)
        {
            var product = await SearchQuery(x => x.Id == id).Include(x => x.ProductCompanyRels)
                .Include(x => x.Categ)
                .Include(x => x.UOM)
                .Include(x => x.UOMPO).Include(x => x.Steps).FirstOrDefaultAsync();
            var res = _mapper.Map<ProductDisplay>(product);
            res.StandardPrice = _GetStandardPrice(product.Id);
            res.ListPrice = await _GetListPrice(product);
            return res;
        }

        public async Task _SetListPrice(Product self, decimal list_price)
        {
            var irConfigParameter = GetService<IIrConfigParameterService>();
            var value = await irConfigParameter.GetParam("product.listprice_restrict_company");
            if (string.IsNullOrEmpty(value) || !Convert.ToBoolean(value))
                return;

            var propertyObj = GetService<IIRPropertyService>();
            var list = new List<Product>() { self };
            propertyObj.set_multi("list_price", "product.product", list.ToDictionary(x => string.Format("product.product,{0}", x.Id), x => (object)list_price));
        }

        private async Task<decimal> _GetListPrice(Product self)
        {
            var irConfigParameter = GetService<IIrConfigParameterService>();
            var value = await irConfigParameter.GetParam("product.listprice_restrict_company");
            if (string.IsNullOrEmpty(value) || !Convert.ToBoolean(value))
                return self.ListPrice;

            var propertyObj = GetService<IIRPropertyService>();
            var val = propertyObj.get("list_price", "product.product", res_id: $"product.product,{self.Id.ToString()}");
            return Convert.ToDecimal(val);
        }

        public double _GetStandardPrice(Guid id)
        {
            var propertyObj = GetService<IIRPropertyService>();
            var val = propertyObj.get("standard_price", "product.product", res_id: $"product.product,{id.ToString()}");
            return Convert.ToDouble(val);
        }

        public double GetStandardPrice(Guid id, Guid? force_company_id = null)
        {
            var propertyObj = GetService<IIRPropertyService>();
            var val = propertyObj.get("standard_price", "product.product", res_id: $"product.product,{id}", force_company: force_company_id);
            return Convert.ToDouble(val);
        }

        public async Task<ProductDisplay> DefaultGet()
        {
            var uomObj = GetService<IUoMService>();
            var uom = await uomObj.DefaultUOM();
            var res = new ProductDisplay();
            if (uom != null)
            {
                res.UOMId = uom.Id;
                res.UOM = _mapper.Map<UoMBasic>(uom);

                res.UOMPOId = uom.Id;
                res.UOMPO = _mapper.Map<UoMBasic>(uom);
            }

            res.CompanyId = CompanyId;

            return res;
        }

        public async Task<ProductDisplay> GetDefaultProductMedicine()
        {
            var uomObj = GetService<IUoMService>();
            var userObj = GetService<IUserService>();
            var uom = await uomObj.DefaultUOM();
            var isGroupMedicine = await userObj.HasGroup("medicineOrder.group_medicine");
            var res = new ProductDisplay();
            if (uom != null)
            {
                res.UOMId = uom.Id;
                res.UOM = _mapper.Map<UoMBasic>(uom);

                res.UOMPOId = uom.Id;
                res.UOMPO = _mapper.Map<UoMBasic>(uom);
            }

            if (isGroupMedicine)
                res.Type = "product";
            else
                res.Type = "consu";

            res.CompanyId = CompanyId;

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

        public async Task<IDictionary<Guid, decimal>> _ComputeProductPrice(IEnumerable<Product> self, Guid pricelistId, Guid? partnerId = null, decimal quantity = 1, DateTime? date = null)
        {
            var res = new Dictionary<Guid, decimal>();
            foreach (var product in self)
            {
                res.Add(product.Id, 0);
            }

            var plobj = GetService<IProductPricelistService>();
            var pricelist = await plobj.GetByIdAsync(pricelistId);
            var qtys = new List<ProductQtyByPartner>();
            foreach (var product in self)
            {
                qtys.Add(new ProductQtyByPartner
                {
                    Product = product,
                    Quantity = quantity,
                    PartnerId = partnerId,
                });
            }
            var prices = await plobj._ComputePriceRule(pricelist, qtys, date: date);
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

        public IDictionary<Guid, ProductAvailableRes> ProductAvailable(IEnumerable<Guid> ids = null, Guid? location_id = null, Guid? warehouse_id = null, Guid? company_id = null,
        DateTime? from_date = null, DateTime? to_date = null)
        {
            return _ComputeQuantitiesDict(ids: ids, from_date: from_date, to_date: to_date, location_id: location_id, warehouse_id: warehouse_id, company_id: company_id);
        }

        public IDictionary<Guid, ProductAvailableRes> _ComputeQuantitiesDict(IEnumerable<Guid> ids = null, DateTime? from_date = null,
   DateTime? to_date = null, Guid? location_id = null, Guid? warehouse_id = null, Guid? company_id = null)
        {
            var locationObj = GetService<IStockLocationService>();
            var quantObj = GetService<IStockQuantService>();
            var moveObj = GetService<IStockMoveService>();

            var domain_res = _GetDomainLocations(location_id: location_id, warehouse_id: warehouse_id, company_id: company_id);
            var domain_quant_loc = domain_res.domain_quant_loc;
            var domain_move_in_loc = domain_res.domain_move_in_loc;
            var domain_move_out_loc = domain_res.domain_move_out_loc;

            var domain_quant = domain_quant_loc;
            var domain_move_in = domain_move_in_loc;
            var domain_move_out = domain_move_out_loc;
            if (ids != null)
            {
                domain_quant = domain_quant_loc.And(new InitialSpecification<StockQuant>(x => ids.Contains(x.ProductId)));
                domain_move_in = domain_move_in_loc.And(new InitialSpecification<StockMove>(x => ids.Contains(x.ProductId)));
                domain_move_out = domain_move_out_loc.And(new InitialSpecification<StockMove>(x => ids.Contains(x.ProductId)));
            }

            var dates_in_the_past = false;
            if (to_date.HasValue && to_date < DateTime.Now)
                dates_in_the_past = true;

            if (from_date.HasValue)
            {
                domain_move_in_loc = domain_move_in_loc.And(new InitialSpecification<StockMove>(x => x.Date >= from_date));
                domain_move_out_loc = domain_move_out_loc.And(new InitialSpecification<StockMove>(x => x.Date >= from_date));
            }

            if (to_date.HasValue)
            {
                domain_move_in_loc = domain_move_in_loc.And(new InitialSpecification<StockMove>(x => x.Date <= to_date));
                domain_move_out_loc = domain_move_out_loc.And(new InitialSpecification<StockMove>(x => x.Date <= to_date));
            }

            var states = new string[] { "done", "cancel", "draft" };
            var domain_move_in_todo = domain_move_in.And(new InitialSpecification<StockMove>(x => !states.Contains(x.State)));
            var domain_move_out_todo = domain_move_out.And(new InitialSpecification<StockMove>(x => !states.Contains(x.State)));

            var quants_res = quantObj.SearchQuery(domain_quant.AsExpression()).GroupBy(x => x.ProductId).Select(x => new
            {
                ProductId = x.Key,
                Qty = x.Sum(s => s.Qty),
            }).ToDictionary(x => x.ProductId, x => x.Qty);

            var movesIn = moveObj.SearchQuery(domain: domain_move_in_todo.AsExpression()).GroupBy(x => x.ProductId).Select(x => new
            {
                ProductId = x.Key,
                Qty = x.Sum(s => s.ProductQty),
            }).ToDictionary(x => x.ProductId, x => x.Qty);

            var movesOut = moveObj.SearchQuery(domain_move_out_todo.AsExpression()).GroupBy(x => x.ProductId).Select(x => new
            {
                ProductId = x.Key,
                Qty = x.Sum(s => s.ProductQty),
            }).ToDictionary(x => x.ProductId, x => x.Qty);

            var moves_in_res_past = new Dictionary<Guid, decimal?>();
            var moves_out_res_past = new Dictionary<Guid, decimal?>();
            if (dates_in_the_past)
            {
                //Calculate the moves that were done before now to calculate back in time (as most questions will be recent ones)
                var domain_move_in_done = domain_move_in.And(new InitialSpecification<StockMove>(x => x.State == "done" && x.Date > to_date));
                var domain_move_out_done = domain_move_out.And(new InitialSpecification<StockMove>(x => x.State == "done" && x.Date > to_date));

                moves_in_res_past = moveObj.SearchQuery(domain_move_in_done.AsExpression()).GroupBy(x => x.ProductId).Select(x => new
                {
                    ProductId = x.Key,
                    Qty = x.Sum(s => s.ProductQty),
                }).ToDictionary(x => x.ProductId, x => x.Qty);

                moves_out_res_past = moveObj.SearchQuery(domain_move_out_done.AsExpression()).GroupBy(x => x.ProductId).Select(x => new
                {
                    ProductId = x.Key,
                    Qty = x.Sum(s => s.ProductQty),
                }).ToDictionary(x => x.ProductId, x => x.Qty);
            }

            var res = new Dictionary<Guid, ProductAvailableRes>();

            ISpecification<Product> product_domain = new InitialSpecification<Product>(x => true);
            if (ids != null)
                product_domain = product_domain.And(new InitialSpecification<Product>(x => ids.Contains(x.Id)));

            var self = SearchQuery(product_domain.AsExpression()).Select(x => new
            {
                Id = x.Id,
                UOMRounding = x.UOM.Rounding
            }).ToList();

            foreach (var product in self)
            {
                res.Add(product.Id, new ProductAvailableRes());
                decimal qty_available = 0;
                if (dates_in_the_past)
                {
                    qty_available = (quants_res.ContainsKey(product.Id) ? quants_res[product.Id] : 0) -
                        (moves_in_res_past.ContainsKey(product.Id) ? (moves_in_res_past[product.Id] ?? 0) : 0) +
                        (moves_out_res_past.ContainsKey(product.Id) ? (moves_out_res_past[product.Id] ?? 0) : 0);
                }
                else
                    qty_available = (quants_res.ContainsKey(product.Id) ? quants_res[product.Id] : 0);
                res[product.Id].QtyAvailable = qty_available; //(decimal)FloatUtils.FloatRound((double)qty_available, precisionRounding: (double?)product.UOMRounding);
                res[product.Id].IncomingQty = movesIn.ContainsKey(product.Id) ? (movesIn[product.Id] ?? 0) : 0;//(decimal)FloatUtils.FloatRound((double)(movesIn.ContainsKey(product.Id) ? (movesIn[product.Id] ?? 0) : 0), precisionRounding: (double?)product.UOMRounding);
                res[product.Id].OutgoingQty = movesOut.ContainsKey(product.Id) ? (movesOut[product.Id] ?? 0) : 0; //(decimal)FloatUtils.FloatRound((double)(movesOut.ContainsKey(product.Id) ? (movesOut[product.Id] ?? 0) : 0), precisionRounding: (double?)product.UOMRounding);
                res[product.Id].VirtualAvailable = qty_available + res[product.Id].IncomingQty - res[product.Id].OutgoingQty;//(decimal)FloatUtils.FloatRound((double)(qty_available + res[product.Id].IncomingQty - res[product.Id].OutgoingQty), precisionRounding: (double?)product.UOMRounding);
            }

            return res;
        }

        private GetDomainLoctionsRes _GetDomainLocations(Guid? location_id = null, Guid? warehouse_id = null, Guid? company_id = null)
        {
            var locationObj = GetService<StockLocationService>();
            var warehouseObj = GetService<IStockWarehouseService>();
            var location_ids = new List<Guid>();
            if (location_id.HasValue)
            {
                location_ids.Add(location_id.Value);
            }
            else
            {
                if (warehouse_id.HasValue)
                {
                    var wh = warehouseObj.GetById(warehouse_id.Value);
                    location_ids.Add(wh.LocationId);
                }
                else if (company_id.HasValue)
                {
                    var whs = warehouseObj.SearchQuery(x => x.CompanyId == company_id).ToList();
                    foreach (var wh in whs)
                        location_ids.Add(wh.ViewLocationId);
                }
                else
                {
                    var whs = warehouseObj.SearchQuery().ToList();
                    foreach (var wh in whs)
                        location_ids.Add(wh.ViewLocationId);
                }
            }

            return _GetDomainLocationsNew(location_ids, company_id: company_id);
        }

        private GetDomainLoctionsRes _GetDomainLocationsNew(IList<Guid> location_ids, Guid? company_id = null, bool compute_child = true)
        {
            var locObj = GetService<IStockLocationService>();
            var opt = compute_child ? "child_of" : "in";
            ISpecification<StockQuant> quant_domain = new InitialSpecification<StockQuant>(x => true);
            ISpecification<StockMove> move_domain = new InitialSpecification<StockMove>(x => true);

            if (company_id.HasValue)
            {
                quant_domain = quant_domain.And(new InitialSpecification<StockQuant>(x => x.CompanyId == company_id));
                move_domain = move_domain.And(new InitialSpecification<StockMove>(x => x.CompanyId == company_id));
            }

            ISpecification<StockQuant> loc_domain = new InitialSpecification<StockQuant>(x => false);
            ISpecification<StockMove> move_loc_domain = new InitialSpecification<StockMove>(x => false);
            ISpecification<StockMove> move_dest_loc_domain = new InitialSpecification<StockMove>(x => false);

            ISpecification<StockMove> neg_move_loc_domain = new InitialSpecification<StockMove>(x => true); //phủ định
            ISpecification<StockMove> neg_move_dest_loc_domain = new InitialSpecification<StockMove>(x => true);


            var locations = locObj.SearchQuery(x => location_ids.Contains(x.Id)).ToList();
            var hierarchical_locations = locations.Where(x => x.ParentLeft != 0 && opt == "child_of");
            var other_locations = locations.Except(hierarchical_locations);

            foreach (var location in hierarchical_locations)
            {
                loc_domain = loc_domain.Or(new InitialSpecification<StockQuant>(x => x.Location.ParentLeft >= location.ParentLeft && x.Location.ParentLeft < location.ParentRight));
                move_loc_domain = move_loc_domain.Or(new InitialSpecification<StockMove>(x => x.Location.ParentLeft >= location.ParentLeft && x.Location.ParentLeft < location.ParentRight));
                move_dest_loc_domain = move_dest_loc_domain.Or(new InitialSpecification<StockMove>(x => x.LocationDest.ParentLeft >= location.ParentLeft && x.LocationDest.ParentLeft < location.ParentRight));

                neg_move_loc_domain = neg_move_loc_domain.And(new InitialSpecification<StockMove>(x => !(x.Location.ParentLeft >= location.ParentLeft && x.Location.ParentLeft < location.ParentRight)));
                neg_move_dest_loc_domain = neg_move_dest_loc_domain.And(new InitialSpecification<StockMove>(x => !(x.LocationDest.ParentLeft >= location.ParentLeft && x.LocationDest.ParentLeft < location.ParentRight)));
            }

            if (other_locations.Any())
            {
                var other_location_ids = other_locations.Select(x => x.Id);
                loc_domain = loc_domain.And(new InitialSpecification<StockQuant>(x => other_location_ids.Contains(x.LocationId)));
                move_loc_domain = move_loc_domain.And(new InitialSpecification<StockMove>(x => other_location_ids.Contains(x.LocationId)));
                move_dest_loc_domain = move_dest_loc_domain.And(new InitialSpecification<StockMove>(x => other_location_ids.Contains(x.LocationDestId)));

                neg_move_loc_domain = neg_move_loc_domain.And(new InitialSpecification<StockMove>(x => !other_location_ids.Contains(x.LocationId)));
                neg_move_dest_loc_domain = neg_move_dest_loc_domain.And(new InitialSpecification<StockMove>(x => !other_location_ids.Contains(x.LocationDestId)));
            }

            return new GetDomainLoctionsRes
            {
                domain_quant_loc = quant_domain.And(loc_domain),
                domain_move_in_loc = move_domain.And(move_dest_loc_domain).And(neg_move_loc_domain),
                domain_move_out_loc = move_domain.And(move_loc_domain).And(neg_move_dest_loc_domain)
            };
        }

        public async Task<IEnumerable<ProductProductExportExcel>> GetProductExportExcel(ProductPaged val)
        {
            var query = SearchQuery(x => x.Active && x.Type2 == "product");
            if (!string.IsNullOrWhiteSpace(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.NameNoSign.Contains(val.Search));
            if (val.CategId.HasValue)
                query = query.Where(x => x.CategId == val.CategId);
            var res = await query.OrderBy(x => x.Name).Select(x => new ProductProductExportExcel
            {
                CategName = x.Categ.Name,
                DefaultCode = x.DefaultCode,
                Name = x.Name,
                PurchasePrice = x.PurchasePrice,
                Type = x.Type,
                UomName = x.UOM.Name,
                UoMPOName = x.UOMPO.Name,
                MinInventory = x.MinInventory,
                Origin = x.Origin,
                Expiry = x.Expiry
            }).ToListAsync();

            return res;
        }

        public async Task<IEnumerable<ProductProductExportExcel>> GetMedicineExportExcel(ProductPaged val)
        {
            var query = SearchQuery(x => x.Active && x.Type2 == "medicine");
            query.Include(x => x.UOM);
            if (!string.IsNullOrWhiteSpace(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.NameNoSign.Contains(val.Search));
            if (val.CategId.HasValue)
                query = query.Where(x => x.CategId == val.CategId);
            var res = await query.OrderBy(x => x.Name).Select(x => new ProductProductExportExcel
            {
                DefaultCode = x.DefaultCode,
                CategName = x.Categ.Name,
                Name = x.Name,
                Type = x.Type,
                ListPrice = x.ListPrice,
                UomName = x.UOM.Name,
                MinInventory = x.MinInventory,
                Origin = x.Origin,
                Expiry = x.Expiry
            }).ToListAsync();

            return res;
        }
        private async Task<bool> IsExistProductCode(string productCode)
        {
            var product = await SearchQuery(x => x.DefaultCode == productCode).FirstOrDefaultAsync();
            if (product == null)
                return false;
            return true;
        }
        private bool IsCorrectFormat(string productCode)
        {
            string pattern = @"^SP\d{1,}$";
            Regex rg = new Regex(pattern, RegexOptions.IgnoreCase);
            var isMatching = rg.IsMatch(productCode);
            return isMatching;
        }
        private async Task<string> GenerateCodeIfEmpty()
        {
            var seqObj = GetService<IIRSequenceService>();

            var code = await seqObj.NextByCode("product_seq");

            if (string.IsNullOrWhiteSpace(code))
            {
                await _InsertProductSequence();
                code = await seqObj.NextByCode("product_seq");
            }
            return code;
        }
        private async Task UpdateNextCode(string productCode)
        {
            Regex rgx = new Regex(@"SP", RegexOptions.IgnoreCase);
            var result = rgx.Split(productCode, 2, 0);
            var numberCode = result[1];
            int number = Int32.Parse(numberCode.ToString());
            var sequenceObj = GetService<IIRSequenceService>();
            var sequence = await sequenceObj.SearchQuery(x => x.Code == "product_seq").FirstOrDefaultAsync();
            if (sequence == null)
            {
                await _InsertProductSequence();
                sequence = await sequenceObj.SearchQuery(x => x.Code == "product_seq").FirstOrDefaultAsync();
            }

            if (number > sequence.NumberNext)
            {
                sequence.NumberNext = number + sequence.NumberIncrement;
                await sequenceObj.UpdateAsync(sequence);
            }

        }
    }

    public class ProductAvailableRes
    {
        public decimal QtyAvailable { get; set; }

        public decimal IncomingQty { get; set; }

        public decimal OutgoingQty { get; set; }

        public decimal VirtualAvailable { get; set; }
    }

    public class GetDomainLoctionsRes
    {
        public ISpecification<StockQuant> domain_quant_loc { get; set; }

        public ISpecification<StockMove> domain_move_in_loc { get; set; }

        public ISpecification<StockMove> domain_move_out_loc { get; set; }
    }


}
