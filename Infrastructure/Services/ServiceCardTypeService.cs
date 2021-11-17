using ApplicationCore.Entities;
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
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ServiceCardTypeService : BaseService<ServiceCardType>, IServiceCardTypeService
    {
        private readonly IMapper _mapper;
        public ServiceCardTypeService(IAsyncRepository<ServiceCardType> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<ServiceCardTypeBasic>> GetPagedResultAsync(ServiceCardTypePaged val)
        {
            ISpecification<ServiceCardType> spec = new InitialSpecification<ServiceCardType>(x => x.Active);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<ServiceCardType>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var totalItems = await query.CountAsync();
            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);
            var items = await _mapper.ProjectTo<ServiceCardTypeBasic>(query.OrderByDescending(x => x.DateCreated)).ToListAsync();

            return new PagedResult2<ServiceCardTypeBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<ServiceCardType> CreateUI(ServiceCardTypeSave val)
        {
            var type = _mapper.Map<ServiceCardType>(val);
            type.CompanyId = CompanyId;

            var product = await CreateProduct(type);
            type.ProductId = product.Id;

            return await CreateAsync(type);
        }

        public async Task UpdateUI(Guid id, ServiceCardTypeSave val)
        {
            var type = await SearchQuery(x => x.Id == id).Include(x => x.Product).FirstOrDefaultAsync();
            type = _mapper.Map(val, type);

            if (type.Product != null)
            {
                type.Product.Name = val.Name;
                type.Product.NameNoSign = val.Name.RemoveSignVietnameseV2();
            }

            await UpdateAsync(type);
        }

        public DateTime GetPeriodEndDate(ServiceCardType self, DateTime? dStart = null)
        {
            if (dStart == null)
                dStart = DateTime.Today;
            var nb = self.NbrPeriod ?? 0;
            if (self.Period == "year")
                nb = nb * 12;
            return dStart.Value.AddMonths(nb);
        }

        public override ISpecification<ServiceCardType> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "service_card.service_card_type_comp_rule":
                    return new InitialSpecification<ServiceCardType>(x => !x.CompanyId.HasValue || x.CompanyId == companyId);
                default:
                    return null;
            }
        }

        public async Task<Product> CreateProduct(ServiceCardType self)
        {
            var productObj = GetService<IProductService>();
            var reward_string = self.Name;
            var categObj = GetService<IProductCategoryService>();
            var categ = await categObj.DefaultCategory();
            var uomObj = GetService<IUoMService>();
            var uom = await uomObj.SearchQuery().FirstOrDefaultAsync();

            var product = new Product()
            {
                PurchaseOK = false,
                SaleOK = false,
                Type = "service",
                Name = reward_string,
                UOMId = uom.Id,
                UOMPOId = uom.Id,
                CategId = categ.Id,
                ListPrice = 0,
            };

            await productObj.CreateAsync(product);
            return product;
        }

        public void SaveProductPricelistItem(ServiceCardType self, IEnumerable<ProductPricelistItem> listItems)
        {
            if (!listItems.Any())
            {
                self.ProductPricelistId = null;
                self.ProductPricelist = null;
                return;
            }    

            if (self.ProductPricelist == null)
            {
                var prList = new ProductPricelist()
                {
                    CompanyId = self.CompanyId,
                    Name = "Bảng giá của ưu đãi " + self.Name
                };
                self.ProductPricelist = prList;
            }

            //var lineToRemoves = new List<ProductPricelistItem>();
            //lineToRemoves.AddRange(self.ProductPricelist.Items.Where(x => listItems.All(z => z.Id != x.Id)));
            ////check delete item
            //foreach (var line in lineToRemoves)
            //{
            //    self.ProductPricelist.Items.Remove(line);
            //}

            self.ProductPricelist.Items.Clear();
            foreach (var item in listItems)
            {
                item.CompanyId = self.ProductPricelist.CompanyId;
                self.ProductPricelist.Items.Add(item);
            }
        }

        public async Task<IEnumerable<ServiceCardTypeSimple>> AutoCompleteSearch(string search)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.Contains(search));
            }
            var res = await query.ToListAsync();
            return _mapper.Map<IEnumerable<ServiceCardTypeSimple>>(res);
        }

        public override async Task UpdateAsync(IEnumerable<ServiceCardType> entities)
        {
            await base.UpdateAsync(entities);
            foreach (var entity in entities)
            {
                await _CheckNameUnique(entity.Name);
            }
        }
        private async Task _CheckNameUnique(string name)
        {
            var count = await SearchQuery(x => x.Name == name).CountAsync();
            if (count >= 2)
                throw new Exception($"Trùng tên hạng thẻ khác");
        }

        private void ValidateApplyPriceListItem(ServiceCardType self)
        {
            if (self == null)
                throw new Exception("Không tìm thấy hạng thẻ");
            if (self.ProductPricelist == null)
                throw new Exception("Không tìm thấy bảng giá của hạng thẻ");
        }
        public override async Task<IEnumerable<ServiceCardType>> CreateAsync(IEnumerable<ServiceCardType> entities)
        {
            await base.CreateAsync(entities);
            foreach (var entity in entities)
            {
                await _CheckNameUnique(entity.Name);
            }
            return entities;
        }

        public async Task<IEnumerable<ProductPricelistItem>> AddProductPricelistItem(Guid id, IEnumerable<Guid> productIds)
        {
            var priceListItemObj = GetService<IProductPricelistItemService>();
            var productObj = GetService<IProductService>();
            var self = await SearchQuery(x=> x.Id == id).Include(x=> x.ProductPricelist.Items).FirstOrDefaultAsync();
            ValidateApplyPriceListItem(self);

            var products = await productObj.SearchQuery(x => productIds.Contains(x.Id)).ToListAsync();
            var priceListItems = self.ProductPricelist.Items;
            var toAdds = new List<ProductPricelistItem>();
            foreach (var pro in products)
            {
                if (priceListItems.Any(x => x.ProductId == pro.Id))
                    continue;
                toAdds.Add(new ProductPricelistItem()
                {
                    AppliedOn = "0_product_variant",
                    ComputePrice = "percentage",
                    PercentPrice = 0,
                    ProductId = pro.Id,
                    PriceListId = self.ProductPricelistId
                });
            }

            await priceListItemObj.CreateAsync(toAdds);
            return toAdds;
        }

        public async Task UpdateProductPricelistItem(Guid id, IEnumerable<ProductPricelistItemCreate> items)
        {
            var priceListItemObj = GetService<IProductPricelistItemService>();
            var self = await SearchQuery(x => x.Id == id).Include(x => x.ProductPricelist.Items).ThenInclude(x=> x.Product).FirstOrDefaultAsync();
            ValidateApplyPriceListItem(self);

             var priceListItems = self.ProductPricelist.Items;
            var toUpdates = new List<ProductPricelistItem>();
            foreach (var item in items)
            {
                var existItem = priceListItems.FirstOrDefault(x=> x.Id == item.Id);
                if (item == null)
                    continue;
                existItem.ComputePrice = item.ComputePrice;
                existItem.PercentPrice = item.PercentPrice;
                existItem.FixedAmountPrice = item.FixedAmountPrice;

                toUpdates.Add(existItem);
            }
            priceListItemObj.ValidateBase(toUpdates);// validate before CU
            await priceListItemObj.UpdateAsync(toUpdates);
        }

        public async Task ApplyServiceCategories(Guid id, IEnumerable<ApplyServiceCategoryReq> vals)
        {
            var priceListItemObj = GetService<IProductPricelistItemService>();
            var productObj = GetService<IProductService>();
            var self = await SearchQuery(x => x.Id == id).Include(x => x.ProductPricelist.Items).ThenInclude(x=> x.Product).FirstOrDefaultAsync();
            ValidateApplyPriceListItem(self);
            var priceListItems = self.ProductPricelist.Items;
            //get list service
            var categIds = vals.Where(x=> x.CategId != Guid.Empty).Select(x=> x.CategId);
            var services = await productObj.SearchQuery(x => x.Active == true && x.Type2 == "service" && categIds.Contains(x.CategId.Value)).ToListAsync();
            // add and update pricelistitem
            var toAdds = new List<ProductPricelistItem>();
            var toUpdates = new List<ProductPricelistItem>();
            foreach (var service in services)
            {
                var cateVal = vals.FirstOrDefault(x => x.CategId == service.CategId);
                var existItem = priceListItems.FirstOrDefault(x => x.ProductId == service.Id);

                if (existItem != null)
                {
                    existItem.ComputePrice = cateVal.ComputePrice;
                    existItem.PercentPrice = cateVal.PercentPrice;
                    existItem.FixedAmountPrice = cateVal.FixedAmountPrice;
                    toUpdates.Add(existItem);
                }
                else
                {
                    toAdds.Add(new ProductPricelistItem()
                    {
                        AppliedOn = "0_product_variant",
                        ComputePrice = cateVal.ComputePrice,
                        PercentPrice = cateVal.PercentPrice,
                        FixedAmountPrice = cateVal.FixedAmountPrice,
                        ProductId = service.Id,
                        Product = service,
                        PriceListId = self.ProductPricelistId
                    });
                }
            }

            if (toAdds.Any())
            {
                priceListItemObj.ValidateBase(toAdds);// validate before CU
                await priceListItemObj.CreateAsync(toAdds);
            }
            if (toUpdates.Any())
            {
                priceListItemObj.ValidateBase(toUpdates);// validate before CU
                await priceListItemObj.UpdateAsync(toUpdates);
            }
        }

        public async Task ApplyAllServices(Guid id, ApplyAllServiceReq val)
        {
            var priceListItemObj = GetService<IProductPricelistItemService>();
            var productObj = GetService<IProductService>();
            var self = await SearchQuery(x => x.Id == id).Include(x => x.ProductPricelist.Items).ThenInclude(x => x.Product).FirstOrDefaultAsync();
            ValidateApplyPriceListItem(self);
            var priceListItems = self.ProductPricelist.Items;
            //get list service
            var services = await productObj.SearchQuery(x => x.Active == true && x.Type2 == "service").ToListAsync();
            // add and update pricelistitem
            var toAdds = new List<ProductPricelistItem>();
            var toUpdates = new List<ProductPricelistItem>();
            foreach (var service in services)
            {
                var existItem = priceListItems.FirstOrDefault(x => x.ProductId == service.Id);

                if (existItem != null)
                {
                    existItem.ComputePrice = val.ComputePrice;
                    existItem.PercentPrice = val.PercentPrice;
                    existItem.FixedAmountPrice = val.FixedAmountPrice;
                    toUpdates.Add(existItem);
                }
                else
                {
                    toAdds.Add(new ProductPricelistItem()
                    {
                        AppliedOn = "0_product_variant",
                        ComputePrice = val.ComputePrice,
                        PercentPrice = val.PercentPrice,
                        FixedAmountPrice = val.FixedAmountPrice,
                        ProductId = service.Id,
                        Product = service,
                        PriceListId = self.ProductPricelistId
                    });
                }
            }

            if (toAdds.Any())
            {
                priceListItemObj.ValidateBase(toAdds);// validate before CU
                await priceListItemObj.CreateAsync(toAdds);
            }
            if (toUpdates.Any())
            {
                priceListItemObj.ValidateBase(toUpdates);// validate before CU
                await priceListItemObj.UpdateAsync(toUpdates);
            }
        }
    }
}
