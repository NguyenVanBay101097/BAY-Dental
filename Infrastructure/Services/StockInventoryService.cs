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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class StockInventoryService : BaseService<StockInventory>, IStockInventoryService
    {
        private readonly IMapper _mapper;
        public StockInventoryService(IAsyncRepository<StockInventory> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }



        public async Task<PagedResult2<StockInventoryBasic>> GetPagedResultAsync(StockInventoryPaged val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State == val.State);

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom);

            if (val.DateTo.HasValue)
            {
                var dateOrderTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateOrderTo);
            }

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);


            var totalItems = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var paged = new PagedResult2<StockInventoryBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<StockInventoryBasic>>(items)
            };

            return paged;
        }

        public async Task<StockInventoryDisplay> GetDisplay(Guid id)
        {
            var lineObj = GetService<IStockInventoryLineService>();
            var stockMoveObj = GetService<IStockMoveService>();
            var res = await SearchQuery(x => x.Id == id)
               .Include(x => x.Criteria)
                .Include(x => x.Location)
                .Include(x => x.Product)
                .Include(x => x.Category)
                .FirstOrDefaultAsync();

            //get inventorylines
            res.Lines = await lineObj.SearchQuery(x => x.InventoryId == res.Id).Include(x => x.Location).Include(x => x.Product).Include(x => x.ProductUOM).ToListAsync();

            //get stockmoves
            res.Moves = await stockMoveObj.SearchQuery(x => x.InventoryId == res.Id && x.State == "done").Include(x => x.Product).Include(x => x.ProductUOM).ToListAsync();

            var display = _mapper.Map<StockInventoryDisplay>(res);

            return display;
        }

        public async Task<StockInventoryDisplay> DefaultGet()
        {
            ///lấy vị trí kho
            var location = await DefaultStockLocation();
            var display = new StockInventoryDisplay();
            display.CompanyId = CompanyId;
            display.LocationId = location.Id;
            display.Filter = "none";
            display.Date = DateTime.Now;
            display.Exhausted = false;
            display.State = "draft";

            return display;
        }

        public async Task<StockInventory> CreateStockInventory(StockInventorySave val)
        {
            var inventory = _mapper.Map<StockInventory>(val);
            await CreateAsync(inventory);
            return inventory;
        }

        public async Task UpdateStockInventory(Guid id, StockInventorySave val)
        {
            var inventory = await SearchQuery(x => x.Id == id)
                .Include(x => x.Lines)
                .FirstOrDefaultAsync();

            inventory = _mapper.Map(val, inventory);
            SaveInventoryLines(val, inventory);
            await UpdateAsync(inventory);
        }

        public async Task PrepareInventory(IEnumerable<Guid> ids)
        {
            var inventoryLineObj = GetService<IStockInventoryLineService>();
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Location)
                .Include(x => x.Product)
                .Include(x => x.Category)
                .Include(x => x.Criteria)
                .Include(x => x.Lines)
                .ToListAsync();

            foreach (var inventory in self)
            {
                var lineIds = inventory.Lines.Select(x => x.Id).ToList();
                if (!inventory.Lines.Any() && inventory.Filter != "partial")
                {
                    var vals = await GetInventoryLines(inventory);
                    await inventoryLineObj.CreateAsync(vals);
                }

                inventory.State = "confirmed";

                await UpdateAsync(inventory);
            }
        }

        //public async Task PrepareInventory(IEnumerable<StockInventory> self)
        //{
        //    var inventoryLineObj = GetService<IStockInventoryLineService>();
        //    foreach (var inventory in self)
        //    {
        //        var lineIds = inventory.Lines.Select(x => x.Id).ToList();
        //        if (!inventory.Lines.Any() && inventory.Filter != "partial")
        //        {
        //            var vals = GetInventoryLines(inventory);
        //            await inventoryLineObj.CreateAsync(vals);
        //        }

        //        inventory.State = "confirmed";
        //        Update(inventory);
        //    }
        //}

        public async Task<IEnumerable<StockInventoryLine>> GetInventoryLines(StockInventory self)
        {
            var res = new List<StockInventoryLine>();
            var locationObj = GetService<IStockLocationService>();
            var productObj = GetService<IProductService>();
            var criteriaObj = GetService<IStockInventoryCriteriaService>();
            var stockQuantObj = GetService<IStockQuantService>();
            var types = new List<string>() { "service", "consu" };
            var locationIds = new List<Guid>() { self.LocationId }; //lấy tất cả các location 

            //Xác định những sản phẩm sẽ filter, ví dụ lọc theo nhóm sản phẩm thì chỉ lấy những sản phẩm thuộc nhóm sản phẩm
            var products_to_filter = new List<Product>().AsEnumerable();
            var productIds_to_Criteria = new List<Guid>().AsEnumerable();
            var quant_products = new List<Product>().AsEnumerable();
            if (self.Filter == "product" && self.Product != null)
            {
                products_to_filter = products_to_filter.Union(new List<Product>() { self.Product });
            }

            if (self.Filter == "category" && self.Category != null)
            {
                var categ_products = productObj.SearchQuery(x => x.CategId == self.Category.Id).ToList();
                products_to_filter = products_to_filter.Union(categ_products);
            }

            if (self.Filter == "criteria" && self.Criteria != null)
            {
                var criteria_products = productObj.SearchQuery(x => x.ProductStockInventoryCriteriaRels.Any(s => s.StockInventoryCriteriaId == self.CriteriaId)).ToList();
                productIds_to_Criteria = criteria_products.Select(x => x.Id).ToList();
                products_to_filter = products_to_filter.Union(criteria_products);
            }

            //không cần điều kiện product active
            var group = await stockQuantObj.SearchQuery(x => locationIds.Contains(x.LocationId) &&
                (!self.ProductId.HasValue || x.ProductId == self.ProductId.Value) &&
                (!self.CategoryId.HasValue || x.Product.CategId == self.CategoryId.Value) &&
                (!self.CriteriaId.HasValue || productIds_to_Criteria.Contains(x.ProductId)))
                .GroupBy(x => new { x.ProductId, x.LocationId })
                .Select(x => new
                {
                    ProductId = x.Key.ProductId,
                    LocationId = x.Key.LocationId,
                    ProductQty = x.Count() > 0 ? x.Sum(s => s.Qty) : 0,
                }).ToListAsync();

            //dictionary product

            foreach (var item in group)
            {
                var product = productObj.GetById(item.ProductId); //nên tối ưu
                quant_products = quant_products.Union(new List<Product>() { product });
                var line = new StockInventoryLine
                {
                    CompanyId = self.CompanyId,
                    InventoryId = self.Id,
                    LocationId = item.LocationId,
                    ProductId = item.ProductId,
                    ProductUOMId = product.UOMId,
                    TheoreticalQty = item.ProductQty,
                    ProductQty = item.ProductQty,
                };
                res.Add(line);
            }

            if (self.Exhausted == true)
            {
                var exhausted_vals = _GetExhaustedInventoryLine(self, products_to_filter, quant_products);
                res.AddRange(exhausted_vals);
            }

            return res;
        }

        private IList<StockInventoryLine> _GetExhaustedInventoryLine(StockInventory self, IEnumerable<Product> products, IEnumerable<Product> quant_products)
        {
            //Trả về những chi tiết cho sản phẩm hết hàng
            var productObj = GetService<IProductService>();
            var vals = new List<StockInventoryLine>();
            var types = new List<string>() { "service", "consu" };
            var query = productObj.SearchQuery(x => !types.Contains(x.Type)); //có cần search active true
            if (products.Any())
            {
                var exhausted_products = products.Except(quant_products);
                var exhausted_product_ids = exhausted_products.Select(x => x.Id);
                query = query.Where(x => exhausted_product_ids.Contains(x.Id));
            }
            else if (quant_products.Any() || self.Filter == "none")
            {
                var quant_product_ids = quant_products.Select(x => x.Id);
                query = query.Where(x => !quant_product_ids.Contains(x.Id));
            }
            else
            {
                return vals;
            }

            var exhausted_products2 = query.Select(x => new
            {
                Id = x.Id,
                UOMId = x.UOMId,
            }).ToList();


            foreach (var product in exhausted_products2)
            {
                vals.Add(new StockInventoryLine
                {
                    CompanyId = self.CompanyId,
                    InventoryId = self.Id,
                    Inventory = self,
                    LocationId = self.LocationId,
                    ProductId = product.Id,
                    ProductUOMId = product.UOMId,
                });
            }

            return vals;
        }

        public async Task<StockLocation> DefaultStockLocation()
        {
            var whObj = GetService<IStockWarehouseService>();
            var wh = await whObj.SearchQuery(x => x.CompanyId == CompanyId).Include(x => x.Location).FirstOrDefaultAsync();
            if (wh != null)
                return wh.Location;
            return null;
        }

        public override async Task<StockInventory> CreateAsync(StockInventory entity)
        {
            var sequenceService = (IIRSequenceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IIRSequenceService));
            entity.Name = await sequenceService.NextByCode("stock.inventory");
            if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
            {
                await _InsertStockInventorySequence();
                entity.Name = await sequenceService.NextByCode("stock.inventory");
            }

            await base.CreateAsync(entity);

            return entity;
        }

        public async Task<StockInventoryPrint> GetPrint(Guid id)
        {
            var inventory = await SearchQuery(x => x.Id == id)
                .Include(x => x.Company)
                .ThenInclude(s => s.Partner)
                .Include(x => x.Criteria)
                .Include(x => x.Location)
                .Include(x => x.Product)
                .Include(x => x.Category)
                .Include(x => x.Lines).ThenInclude(s => s.Product)
                .Include(x => x.Lines).ThenInclude(s => s.ProductUOM)
                .Include(x => x.CreatedBy)
                .FirstOrDefaultAsync();

            var res = _mapper.Map<StockInventoryPrint>(inventory);
            return res;
        }

        private async Task _InsertStockInventorySequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Name = "Phiếu kiểm kho",
                Code = "stock.inventory",
                Prefix = "KK",
                Padding = 5
            });
        }

        public async Task ActionDone(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
               .Include(x => x.Product)
               .Include(x => x.Category)
               .Include(x => x.Moves)
               .Include(x => x.Lines).ThenInclude(s => s.Location)
               .Include(s => s.Lines).ThenInclude(s => s.Product)
               .Include(s => s.Lines).ThenInclude(s => s.ProductUOM)
               .ToListAsync();

            foreach (var inventory in self)
            {
                foreach (var line in inventory.Lines)
                {
                    if (line.ProductQty < 0 && line.ProductQty != line.TheoreticalQty)
                    {
                        //thông báo như thế này rõ ràng hơn, biết sản phẩm nào? số lượng?
                        throw new Exception(string.Format("Bạn không được gán số lượng âm cho chi tiết điều chỉnh: {0} - số lượng: {1}", line.Product.Name, line.ProductQty));
                    }
                }
            }

            await ActionCheck(self);

            foreach (var inventory in self)
                inventory.State = "done";
            await UpdateAsync(self);

            await PostInventory(self);
        }

        private void SaveInventoryLines(StockInventorySave val, StockInventory inventory)
        {
            var lineToRemoves = new List<StockInventoryLine>();

            foreach (var existLine in inventory.Lines)
            {
                if (!val.Lines.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                inventory.Lines.Remove(line);
            }

            var sequence = 0;
            foreach (var line in val.Lines)
            {
                if (line.Id == Guid.Empty)
                {
                    var item = _mapper.Map<StockInventoryLine>(line);
                    item.CompanyId = inventory.CompanyId;
                    item.Sequence = sequence++;
                    inventory.Lines.Add(item);
                }
                else
                {
                    var l = inventory.Lines.SingleOrDefault(c => c.Id == line.Id);
                    _mapper.Map(line, l);
                    l.Sequence = sequence++;
                }
            }
        }

        public async Task<StockInventoryLineDisplay> InventoryLineByProductId(StockInventoryLineByProductId val)
        {
            var locationObj = GetService<IStockLocationService>();
            var productObj = GetService<IProductService>();
            var stockQuantObj = GetService<IStockQuantService>();
            var product = await productObj.SearchQuery(x => x.Id == val.ProductId).Include(x => x.UOM).FirstOrDefaultAsync();
            var locationIds = await locationObj.SearchQuery(x => x.CompanyId == product.CompanyId).Select(x => x.Id).ToListAsync();

            var stockQuant = await stockQuantObj.SearchQuery(x => locationIds.Contains(x.LocationId) &&
                 x.CompanyId == product.CompanyId && (x.ProductId == val.ProductId))
                 .GroupBy(x => new { x.ProductId, x.LocationId })
                 .Select(x => new
                 {
                     ProductId = x.Key.ProductId,
                     LocationId = x.Key.LocationId,
                     ProductQty = x.Count() > 0 ? x.Sum(s => s.Qty) : 0,
                 }).FirstOrDefaultAsync();


            var line = new StockInventoryLineDisplay
            {
                CompanyId = product.CompanyId,
                InventoryId = val.InventoryId,
                LocationId = stockQuant.LocationId,
                ProductId = product.Id,
                Product = _mapper.Map<ProductSimple>(product),
                ProductUOMId = product.UOMId,
                ProductUOM = _mapper.Map<UoMBasic>(product.UOM),
                TheoreticalQty = stockQuant.ProductQty,
                ProductQty = stockQuant.ProductQty,
            };

            return line;
        }

        public async Task<IEnumerable<ProductStockInventory>> GetListProductInventory(Guid id)
        {
            var locationObj = GetService<IStockLocationService>();
            var productObj = GetService<IProductService>();
            var stockQuantObj = GetService<IStockQuantService>();
            var self = await SearchQuery(x => x.Id == id)
               .Include(x => x.Lines).ThenInclude(s => s.Location)
               .Include(s => s.Lines).ThenInclude(s => s.Product)
               .Include(s => s.Lines).ThenInclude(s => s.ProductUOM)
                .Include(x => x.Location)
                .Include(x => x.Product)
                .Include(x => x.Company)
                .Include(x => x.Category)
                .Include(x => x.Location)
                .FirstOrDefaultAsync();

            var locationIds = await locationObj.SearchQuery(x => x.CompanyId == self.CompanyId).Select(x => x.Id).ToListAsync(); //lấy tất cả các location 
            var res = new List<ProductStockInventory>();
            var res1 = new Dictionary<Guid, ProductStockInventory>();
            var res2 = new Dictionary<Guid, ProductStockInventory>();
            var group = stockQuantObj.SearchQuery(x => locationIds.Contains(x.LocationId) &&
                x.CompanyId == self.CompanyId &&
                (!self.ProductId.HasValue || x.ProductId == self.ProductId.Value) &&
                 (!self.CategoryId.HasValue || x.Product.CategId == self.CategoryId.Value))
                .GroupBy(x => new { x.ProductId, x.LocationId })
                .Select(x => new
                {
                    ProductId = x.Key.ProductId,
                    LocationId = x.Key.LocationId,
                    ProductQty = x.Count() > 0 ? x.Sum(s => s.Qty) : 0,
                }).ToDictionary(t => t.ProductId, t => t);

            var groupkeys = group.Select(x => x.Key).ToList();
            var products1 = await productObj.SearchQuery(x => groupkeys.Contains(x.Id)).Select(x => new ProductStockInventory
            {
                ProductId = x.Id,
                Product = _mapper.Map<ProductDisplay>(x),
                LocationId = group[x.Id].LocationId,
                ProductUOMId = x.UOMId,
                ProductUOM = _mapper.Map<UoMBasic>(x.UOM),
                ProductQty = group[x.Id].ProductQty,
                TheoreticalQty = group[x.Id].ProductQty,
            }).ToListAsync();

            res.AddRange(products1);

            res1 = res.ToDictionary(x => x.ProductId, x => x);
            if (self.Exhausted == true)
            {
                //Trả về những chi tiết cho sản phẩm hết hàng
                var types = new List<string>() { "service", "consu" };
                Expression<Func<Product, bool>> exhausted_domain = x => !types.Contains(x.Type);
                res2 = productObj.SearchQuery(exhausted_domain).Select(x => new ProductStockInventory
                {
                    ProductId = x.Id,
                    Product = _mapper.Map<ProductDisplay>(x),
                    LocationId = self.LocationId,
                    ProductUOMId = x.UOMId,
                    ProductUOM = _mapper.Map<UoMBasic>(x.UOM),
                    ProductQty = 0,
                    TheoreticalQty = 0
                }).ToDictionary(x => x.ProductId, x => x);

            }

            foreach (var product in res2)
            {
                if (res1.ContainsKey(product.Key))
                    continue;
                res.Add(product.Value);
            }

            return res;
        }

        public async Task PostInventory(IEnumerable<StockInventory> self)
        {
            var moveObj = GetService<IStockMoveService>();
            await moveObj.ActionDone(self.SelectMany(x => x.Moves).Where(x => x.State != "done").ToList());
        }

        public async Task ActionCheck(IEnumerable<StockInventory> self)
        {
            var inventoryLineObj = GetService<IStockInventoryLineService>();
            foreach (var inventory in self.Where(x => x.State != "done" && x.State != "cancel"))
            {
                await inventoryLineObj._GenerateMoves(inventory.Lines);
            }
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var stockMoveObj = GetService<IStockMoveService>();
            var lineObj = GetService<IStockInventoryLineService>();
            var inventories = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Lines)
                .Include(x => x.Moves)
                .ToListAsync();

            foreach (var inv in inventories)
            {
                await lineObj.DeleteAsync(inv.Lines);
                await stockMoveObj.DeleteAsync(inv.Moves);
                inv.State = "draft";
                inv.Exhausted = false;
            }

            await UpdateAsync(inventories);
        }

        public override ISpecification<StockInventory> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "stock.stock_inventory_comp_rule":
                    return new InitialSpecification<StockInventory>(x => companyIds.Contains(x.CompanyId));
                default:
                    return null;
            }
        }
    }
}
