﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
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
            var res = SearchQuery(x => x.Id == id)
                .Include(x => x.Lines)
                .Include(x => x.Location)
                .Include(x => x.Moves)
                .Include(x => x.Product)
                .Include(x => x.Company)
                .Include(x => x.Category)
                .Include(x => x.Location)
                .AsQueryable();

            var display = await _mapper.ProjectTo<StockInventoryDisplay>(res).FirstOrDefaultAsync();

            return display;
        }

        public async Task<StockInventoryDisplay> DefaultGet(StockInventoryDefaultGet val)
        {
            ///lấy vị trí kho
            var location = await DefaultStockLocation(val.CompanyId.Value);

            var display = new StockInventoryDisplay();
            display.CompanyId = val.CompanyId.Value;
            display.LocationId = location.Id;
            display.Location = _mapper.Map<StockLocationSimple>(location);
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
            var inventory = await SearchQuery(x => x.Id == id).Include(x => x.Product).Include(x => x.Category).FirstOrDefaultAsync();

            inventory = _mapper.Map(val, inventory);

            await UpdateAsync(inventory);

        }

        public async Task PrepareInventory(IEnumerable<Guid> ids)
        {
            var inventoryLineObj = GetService<IStockInventoryLineService>();
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Location)
                .Include(x => x.Product)
                .Include(x => x.Category)
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
            var stockQuantObj = GetService<IStockQuantService>();
            IList<Guid> locationIds = null; //lấy tất cả các location 

            //Xác định những sản phẩm sẽ filter, ví dụ lọc theo nhóm sản phẩm thì chỉ lấy những sản phẩm thuộc nhóm sản phẩm
            var products_to_filter = new List<Product>().AsEnumerable();
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

            var group = await stockQuantObj.SearchQuery(x => x.LocationId == self.LocationId &&
                x.CompanyId == self.CompanyId &&
                (!self.ProductId.HasValue || x.ProductId == self.ProductId.Value) &&
                 (!self.CategoryId.HasValue || x.Product.CategId == self.CategoryId.Value))
                .GroupBy(x => new { x.ProductId, x.LocationId })
                .Select(x => new
                {
                    ProductId = x.Key.ProductId,
                    LocationId = x.Key.LocationId,
                    ProductQty = x.Any() ? x.Sum(s => s.Qty) : 0,
                }).ToListAsync();

            foreach (var item in group)
            {
                var product = productObj.GetById(item.ProductId);
                quant_products = quant_products.Union(new List<Product>() { product });
                var line = new StockInventoryLine
                {
                    CompanyId = self.CompanyId,
                    InventoryId = self.Id,
                    Inventory = self,
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
            var types = new List<string>() { "service", "consu" };
            Expression<Func<Product, bool>> exhausted_domain = x => !types.Contains(x.Type);
            if (products.Any())
            {
                var exhausted_products = products.Except(quant_products);
                var exhausted_product_ids = exhausted_products.Select(x => x.Id);
                //exhausted_domain = exhausted_domain.And(x => exhausted_product_ids.Contains(x.Id));
            }
            else
            {
                var quant_product_ids = quant_products.Select(x => x.Id);
                //exhausted_domain = exhausted_domain.And(x => !quant_product_ids.Contains(x.Id));
            }

            var exhausted_products2 = productObj.SearchQuery(exhausted_domain).Select(x => new
            {
                Id = x.Id,
                UOMId = x.UOMId,
            }).ToList();

            var vals = new List<StockInventoryLine>();
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

        public async Task<StockLocation> DefaultStockLocation(Guid companyId)
        {
            var whObj = GetService<IStockWarehouseService>();
            var wh = await whObj.SearchQuery(x => x.CompanyId == companyId).Include(x => x.Location).FirstOrDefaultAsync();
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

        //public void ActionDone(IEnumerable<long> ids)
        //{
        //    var self = SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Lines).Include(x => x.Moves).ToList();
        //    ActionDone(self);
        //}

        //public void ActionDone(IEnumerable<StockInventory> self)
        //{
        //    var lineObj = GetService<StockInventoryLineService>();
        //    //foreach (var inventory in self)
        //    //{
        //    //    foreach (var line in inventory.Lines)
        //    //    {
        //    //        //if (line.ProductQty < 0 && line.ProductQty != line.TheoreticalQty)
        //    //        //{
        //    //        //    throw new Exception(string.Format("Bạn không được gán số lượng âm cho chi tiết điều chỉnh: {0} - số lượng: {1}", line.Product.NameGet(), line.ProductQty));
        //    //        //}
        //    //    }
        //    //}

        //    ActionCheck(self);
        //    foreach (var inventory in self)
        //        inventory.State = "done";
        //    //Update(self);

        //    PostInventory(self);
        //}

        //public void ActionCancel(string uid, IList<long> ids)
        //{
        //    ActionCancelDraft(uid, ids);
        //}

        private void PostInventory(IEnumerable<StockInventory> self)
        {
            var moveObj = GetService<IStockMoveService>();
            moveObj.ActionDone(self.SelectMany(x => x.Moves).Where(x => x.State != "done").ToList());
        }

        //private void ActionCheck(IEnumerable<StockInventory> self)
        //{
        //    //Checks the inventory and computes the stock move to do
        //    var inventoryLineObj = GetService<StockInventoryLineService>();
        //    var moveObj = GetService<StockMoveService>();
        //    foreach (var inventory in self.Where(x => x.State != "done" && x.State != "cancel"))
        //    {
        //        moveObj.Unlink(inventory.Moves);
        //        //var lines = inventoryLineObj.SearchQuery(x => x.InventoryId == inventory.Id).Include(x => x.Product)
        //        //    .Include(x => x.Inventory).Include(x => x.Location).Include(x => x.ProductUOM).Include(x => x.ProdLot).ToList();
        //        inventoryLineObj._GenerateMoves(inventory.Lines);
        //        //inventoryLineObj._GenerateMoves(lines);
        //    }
        //}

        //private void ActionCheck(string uid, IList<StockInventory> inventories)
        //{
        //    //Checks the inventory and computes the stock move to do
        //    var inventoryLineObj = DependencyResolver.Current.GetService<StockInventoryLineService>();
        //    var moveObj = DependencyResolver.Current.GetService<StockMoveService>();
        //    foreach (var inventory in inventories)
        //    {
        //        moveObj.Delete(inventory.Moves.ToList());
        //        foreach (var line in inventory.Lines)
        //        {
        //            inventoryLineObj.ResolveInventoryLine(line);
        //        }
        //    }
        //}

        //public void ActionCancelDraft(string uid, IList<long> ids)
        //{
        //    foreach (var inv in Search(x => ids.Contains(x.Id)))
        //    {
        //        inv.Lines.Clear();
        //        DependencyResolver.Current.GetService<StockMoveService>().ActionCancel(uid, inv.Moves.Select(x => x.Id));
        //        inv.State = "draft";
        //        Update(inv);
        //    }
        //}

        //public override Expression<Func<StockInventory, bool>> RuleDomainGet(IRRule rule)
        //{
        //    var companyObj = DependencyResolver.Current.GetService<CompanyService>();
        //    var companyIds = companyObj.GetAllCompanyChildren(CompanyId);
        //    switch (rule.Code)
        //    {
        //        case "stock.stock_inventory_comp_rule":
        //            return x => companyIds.Contains(x.CompanyId);
        //        default:
        //            return null;
        //    }
        //}
    }
}
