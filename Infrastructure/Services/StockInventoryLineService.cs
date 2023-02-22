﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
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
    public class StockInventoryLineService : BaseService<StockInventoryLine>, IStockInventoryLineService
    {
        private readonly IMapper _mapper;
        public StockInventoryLineService(IAsyncRepository<StockInventoryLine> repository, IHttpContextAccessor httpContextAccessor , IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public override async Task<StockInventoryLine> CreateAsync(StockInventoryLine entity)
        {
            var res = await base.CreateAsync(entity);
            CheckConstraints(entity);
            return res;
        }

        public override async Task UpdateAsync(StockInventoryLine entity)
        {
            await base.UpdateAsync(entity);
            CheckConstraints(entity);
        }

        private void CheckConstraints(StockInventoryLine entity)
        {
            var res = SearchQuery(x => x.ProductId == entity.ProductId && x.Inventory.State == "confirmed" && x.LocationId == entity.LocationId);
            if (res.Count() > 1)
            {
                var productObj = GetService<IProductService>();
                var locationObj = GetService<IStockLocationService>();
                var location = locationObj.GetById(entity.LocationId);
                var product = productObj.GetById(entity.ProductId);
                throw new Exception(string.Format("Bạn không thể có 2 điều chỉnh tồn kho ở trạng thái đang xử lý với cùng sản phẩm {0}, địa điểm {1}", product.Name, location.NameGet));
            }
        }

        public IDictionary<Guid, decimal> GetTheoreticalQty(IEnumerable<Guid> ids)
        {
            var quantObj = GetService<IStockQuantService>();
            var uomObj = GetService<IUoMService>();
            var res = new Dictionary<Guid, decimal>();
            foreach (var line in SearchQuery(domain: x => ids.Contains(x.Id)))
            {
                var quants = GetQuants(line);
                var totQty = quants.Sum(x => x.Qty);
                if (line.ProductUOM != null && line.Product.UOM.Id != line.ProductUOM.Id)
                {
                    totQty = uomObj.ComputeQtyObj(line.Product.UOM, totQty, line.ProductUOM);
                }
                res.Add(line.Id, totQty);
            }

            return res;
        }

        private IEnumerable<StockQuant> GetQuants(StockInventoryLine line)
        {
            var quantObj = GetService<StockQuantService>();
            return quantObj.SearchQuery(domain: x => x.LocationId == line.LocationId && x.ProductId == line.ProductId);
        }

        public async Task<StockInventoryLineDisplay> OnChangeCreateLine(StockInventoryLineOnChangeCreateLine val)
        {
            var productObj = GetService<IProductService>();
            var quantObj = GetService<IStockQuantService>();
            var uomObj = GetService<IUoMService>();
            var res = new StockInventoryLineDisplay();
            var product = await productObj.SearchQuery(x => x.Id == val.ProductId).Include(x => x.UOM).FirstOrDefaultAsync();
            res.Product = _mapper.Map<ProductSimple>(product);
            res.ProductId = product.Id;
            res.LocationId = val.LocationId;
            res.ProductUOM = _mapper.Map<UoMBasic>(product.UOM);
            res.ProductUOMId = product.UOMId;
            var quants = quantObj.SearchQuery(x => x.ProductId == val.ProductId && x.LocationId == val.LocationId);
            var thtQty = quants.Sum(x => x.Qty);
            res.TheoreticalQty = thtQty;
            res.ProductQty = thtQty;

            return res;
        }

        public Dictionary<Guid, decimal> GetQuantities(List<Guid> ids, Guid locationId)
        {
            var quantObj = GetService<IStockQuantService>();
            var dict_quants = quantObj.SearchQuery(x => ids.Contains(x.ProductId) && x.LocationId == locationId).GroupBy(x => x.ProductId).ToDictionary(x => x.Key, x => x.Sum(y => y.Qty));
            return dict_quants;
        }

        public async Task<IEnumerable<StockMove>> ResolveInventoryLine(StockInventoryLine line)
        {
            var stockMoveObj = GetService<IStockMoveService>();
            var locationObj = GetService<IStockLocationService>();
            var quantObj = GetService<IStockQuantService>();
            var diff = (line.TheoreticalQty - line.ProductQty) ?? 0;
            var res = new List<StockMove>();
            var movesToAssign = new List<StockMove>();
            var move = new StockMove()
            {
                Name = "Kiểm kho: " + line.Inventory.Name,
                ProductId = line.ProductId,
                ProductUOMId = line.ProductUOMId,
                Date = line.Inventory.Date,
                InventoryId = line.InventoryId,
                State = "confirmed",
                CompanyId = line.Inventory.CompanyId,
                Sequence = line.Sequence ?? 0,
            };

            var inventoryLocation = await locationObj.SearchQuery(domain: x => x.Usage == "inventory" && x.ScrapLocation == false).FirstOrDefaultAsync();
            if (inventoryLocation == null)
                throw new Exception("Không có địa điểm mất mát tồn kho");
            if (diff < 0)
            {
                move.LocationId = inventoryLocation.Id;
                move.Location = inventoryLocation;
                move.LocationDestId = line.Location.Id;
                move.LocationDest = line.Location;
                move.ProductUOMQty = -diff;

                await stockMoveObj.CreateAsync(move);
                res.Add(move);
            }
            else if(diff > 0)
            {
                move.Location = line.Location;
                move.LocationId = line.LocationId;
                move.LocationDest = inventoryLocation;
                move.LocationDestId = inventoryLocation.Id;
                move.ProductUOMQty = diff;
                await stockMoveObj.CreateAsync(move);
            }


            //if (movesToAssign.Any())
            //{
            //    //Expression<Func<StockQuant, bool>> domain = x => x.Qty > 0 && x.LocationId == line.LocationId && x.LotId == line.ProdLotId;
            //    //var preferedDomainList = new List<Expression<Func<StockQuant, bool>>>()
            //    //{
            //    //    x => !x.ReservationId.HasValue,
            //    //    x => x.Reservation.InventoryId == line.InventoryId
            //    //};

            //    foreach (var moveToAssign in movesToAssign)
            //    {
            //        var quants = await quantObj.QuantsGetReservation(moveToAssign.ProductQty ?? 0, moveToAssign);
            //        await quantObj.QuantsMove(quants, moveToAssign,moveToAssign.LocationDest);
            //    }
            //}

            return res;
        }


        public async Task _GenerateMoves(IEnumerable<StockInventoryLine> self)
        {
            var moveObj = GetService<IStockMoveService>();
            var quantObj = GetService<IStockQuantService>();
            var locObj = GetService<IStockLocationService>();
            var propertyObj = GetService<IIRPropertyService>();

            var inventory_loc = locObj.SearchQuery(x => x.Usage == "inventory" && x.ScrapLocation == false).FirstOrDefault();
            if (inventory_loc == null)
                throw new Exception("Không tìm thấy địa điểm điều chỉnh tồn kho nào.");

            var to_create = new List<StockMove>();
            foreach (var line in self)
            {
                if (line.TheoreticalQty == line.ProductQty)
                    continue;

                var diff = (line.TheoreticalQty - line.ProductQty) ?? 0;
                StockMove move = null;
                if (diff < 0)
                    move = _get_move_values(line, Math.Abs(diff), inventory_loc, line.Location);
                else
                    move = _get_move_values(line, Math.Abs(diff), line.Location, inventory_loc);

                moveObj._Compute(new List<StockMove> { move });
                to_create.Add(move);
            }

            moveObj._Compute(to_create);
            await moveObj.CreateAsync(to_create);
        }

        private StockMove _get_move_values(StockInventoryLine self, decimal qty, StockLocation location, StockLocation locationDest)
        {
            return new StockMove
            {
                Name = "INV:" + self.Inventory.Name,
                Product = self.Product,
                ProductId = self.ProductId,
                ProductUOM = self.ProductUOM,
                ProductUOMId = self.ProductUOMId,
                Date = self.Inventory.Date,
                InventoryId = self.InventoryId,
                ProductUOMQty = qty,
                //State = "confirmed",
                CompanyId = self.Inventory.CompanyId,
                Company = self.Inventory.Company,
                Location = location,
                LocationId = location.Id,
                LocationDestId = locationDest.Id,
                LocationDest = locationDest,
            };
        }

        public override ISpecification<StockInventoryLine> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "stock.stock_inventory_line_comp_rule":
                    return new InitialSpecification<StockInventoryLine>(x => !x.CompanyId.HasValue || companyIds.Contains(x.CompanyId.Value));
                default:
                    return null;
            }
        }
    }
}
