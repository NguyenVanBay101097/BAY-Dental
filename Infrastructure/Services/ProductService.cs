using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyERP.Utilities;
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
        private readonly IUoMService _uoMService;
        public ProductService(
            IAsyncRepository<Product> repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IUoMService uoMService
            )
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _uoMService = uoMService;
        }

        public override async Task<IEnumerable<Product>> CreateAsync(IEnumerable<Product> entities)
        {
            _SetNameNoSign(entities);

            await _GenerateCodeIfEmpty(entities);

            await _CheckProductExistCode(entities);

            await base.CreateAsync(entities);

            await _SetListPrice(entities);

            return entities;
        }

        private void _SetNameNoSign(IEnumerable<Product> self)
        {
            foreach (var product in self)
                product.NameNoSign = product.Name.RemoveSignVietnameseV2();
        }

        public override async Task UpdateAsync(IEnumerable<Product> entities)
        {
            _SetNameNoSign(entities);

            await _GenerateCodeIfEmpty(entities);

            await _CheckProductExistCode(entities);

            await base.UpdateAsync(entities);

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
            var seqObj = GetService<IIRSequenceService>();
            foreach(var product in self)
            {
                if (!string.IsNullOrWhiteSpace(product.DefaultCode))
                    continue;

                //Không phát sinh mã cho thuốc
                if (product.Type2 == "medicine")
                    continue;

                product.DefaultCode = await seqObj.NextByCode("product_seq");

                if (string.IsNullOrWhiteSpace(product.DefaultCode))
                {
                    await _InsertProductSequence();
                    product.DefaultCode = await seqObj.NextByCode("product_seq");
                }
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
            foreach(var product in self)
            {
                if (string.IsNullOrWhiteSpace(product.DefaultCode))
                    continue;

                var exist = await SearchQuery(x => x.DefaultCode == product.DefaultCode && x.Id != product.Id).FirstOrDefaultAsync();
                if (exist != null)
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
            var items =  await _mapper.ProjectTo<ProductBasic>(query.OrderBy(x => x.Name).Skip(val.Offset).Take(val.Limit)).ToListAsync();

            //Tính lại giá bán
            await _ProcessListPrice(items);

            var totalItems = await query.CountAsync();

            return new PagedResult2<ProductBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        private async Task _ProcessListPrice(List<ProductBasic> items)
        {
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
            var data = await quantObj.SearchQuery(x => ids.Contains(x.Id) && x.Location.Usage == "internal")
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

            var items = await query.OrderBy(x => x.Name).Skip(val.Offset).Take(val.Limit)
                .Select(x => new ProductLaboBasic
                {
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
                query = query.Where(x => x.Type2 == val.Type2);

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

        public void _SetStandardPrice(Product self, double value)
        {
            //Store the standard price change in order to be able to retrieve the cost of a product for a given date'''
            var list = new List<Product>() { self };
            var propertyObj = GetService<IIRPropertyService>();

            //Store the standard price change in order to be able to retrieve the cost of a product for a given date'''
            propertyObj.set_multi("standard_price", "product.product", list.ToDictionary(x => string.Format("product.product,{0}", x.Id), x => (object)value));

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

            _SaveProductSteps(product, val.StepList);

            _SaveUoMRels(product, val);

            return await CreateAsync(product);
        }

        public async Task UpdateProduct(Guid id, ProductSave val)
        {
            var product = await SearchQuery(x => x.Id == id).Include(x => x.Steps)
                .Include(x => x.ProductUoMRels).FirstOrDefaultAsync();

            product = _mapper.Map(val, product);
            product.NameNoSign = StringUtils.RemoveSignVietnameseV2(product.Name);

            _SaveProductSteps(product, val.StepList);
            _SaveUoMRels(product, val);

            //_SetStandardPrice(product, val.StandardPrice);

            await _SetListPrice(product, val.ListPrice);
            await UpdateAsync(product);
        }

        private void _SaveUoMRels(Product product, ProductSave val)
        {
            var rels_remove = new List<ProductUoMRel>();
            var uom_ids = new List<Guid>() { val.UOMId, val.UOMPOId };
            foreach(var rel in product.ProductUoMRels)
            {
                if (!uom_ids.Contains(rel.UoMId))
                    rels_remove.Add(rel);
            }

            foreach (var rel in rels_remove)
                product.ProductUoMRels.Remove(rel);

            foreach(var uom_id in uom_ids)
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

            //res.StandardPrice = _GetStandardPrice(product);
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

        public double _GetStandardPrice(Product self)
        {
            var propertyObj = GetService<IIRPropertyService>();
            var val = propertyObj.get("standard_price", "product.product", res_id: $"product.product,{self.Id.ToString()}");
            return Convert.ToDouble(val);
        }

        public async Task<double> GetStandardPrice(Guid id)
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
            Guid pricelistId,
            Guid? partnerId = null, decimal quantity = 1,
            DateTime? date = null)
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
    }
}
