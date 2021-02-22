using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class StockInventoryLineService : BaseService<StockInventoryLine>, IStockInventoryLineService
    {
        public StockInventoryLineService(IAsyncRepository<StockInventoryLine> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
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

        public StockInventoryLine OnChangeCreateLine(Product product = null, Guid? productId = null, Guid? locationId = null, Guid? uomId = null)
        {
            var productObj = GetService<ProductService>();
            var quantObj = GetService<StockQuantService>();
            var uomObj = GetService<IUoMService>();
            var res = new StockInventoryLine();
            if (product == null && productId.HasValue)
                product = productObj.GetById(productId.Value);
            if (product != null && locationId.HasValue)
            {
                res.ProductUOM = product.UOM;
                res.ProductUOMId = product.UOMId;
                var quants = quantObj.SearchQuery(x => x.ProductId == productId.Value && x.LocationId == locationId.Value);
                var thtQty = quants.Sum(x => x.Qty);
                if (productId.HasValue && uomId.HasValue && product.UOMId != uomId.Value)
                    thtQty = uomObj.ComputeQty(product.UOMId, thtQty, uomId.Value);
                res.TheoreticalQty = thtQty;
                res.ProductQty = thtQty;
            }
            return res;
        }

        public Dictionary<Guid, decimal> GetQuantities(List<Guid> ids, Guid locationId)
        {
            var quantObj = GetService<IStockQuantService>();
            var dict_quants = quantObj.SearchQuery(x => ids.Contains(x.ProductId) && x.LocationId == locationId).GroupBy(x => x.ProductId).ToDictionary(x => x.Key, x => x.Sum(y => y.Qty));
            return dict_quants;
        }

        public IList<StockMove> ResolveInventoryLine(StockInventoryLine line)
        {
            var stockMoveObj = GetService<IStockMoveService>();
            var locationObj = GetService<IStockLocationService>();
            var quantObj = GetService<IStockQuantService>();
            var diff = (line.TheoreticalQty - line.ProductQty) ?? 0;
            var res = new List<StockMove>();
            var movesToAssign = new List<StockMove>();
            var move = new StockMove()
            {
                Name = "INV:" + line.Inventory.Name,
                ProductId = line.ProductId,
                ProductUOMId = line.ProductUOMId,
                Date = line.Inventory.Date,
                InventoryId = line.InventoryId,
                State = "confirmed",
                CompanyId = line.Inventory.CompanyId,
                Company = line.Inventory.Company,
                Sequence = line.Sequence ?? 0,
            };

            var inventoryLocation = locationObj.SearchQuery(domain: x => x.Usage == "inventory" && x.ScrapLocation == false).FirstOrDefault();
            if (inventoryLocation == null)
                throw new Exception("Không có địa điểm mất mát tồn kho");
            if (diff < 0)
            {
                move.LocationId = inventoryLocation.Id;
                move.Location = inventoryLocation;
                move.LocationDestId = line.Location.Id;
                move.LocationDest = line.Location;
                move.ProductUOMQty = -diff;

                stockMoveObj.CreateAsync(move);
                res.Add(move);
            }
            else
            {
                move.Location = line.Location;
                move.LocationId = line.LocationId;
                move.LocationDest = inventoryLocation;
                move.LocationDestId = inventoryLocation.Id;
                move.ProductUOMQty = diff;
                stockMoveObj.CreateAsync(move);
            }


            //if (movesToAssign.Any())
            //{
            //    Expression<Func<StockQuant, bool>> domain = x => x.Qty > 0 && x.LocationId == line.LocationId && x.LotId == line.ProdLotId;
            //    var preferedDomainList = new List<Expression<Func<StockQuant, bool>>>()
            //    {
            //        x => !x.ReservationId.HasValue,
            //        x => x.Reservation.InventoryId == line.InventoryId
            //    };

            //    foreach (var moveToAssign in movesToAssign)
            //    {
            //        var quants = quantObj.QuantsGetPreferedDomain(moveToAssign.ProductQty ?? 0, moveToAssign,
            //       domain: domain, preferedDomain: preferedDomainList);
            //        quantObj.QuantsReserve(quants, moveToAssign);
            //    }
            //}

            return res;
        }

        //public void _GenerateMoves(IEnumerable<StockInventoryLine> self)
        //{
        //    var moveObj = GetService<StockMoveService>();
        //    var quantObj = GetService<StockQuantService>();
        //    var locObj = GetService<StockLocationService>();
        //    var propertyObj = GetService<IRPropertyService>();

        //    //var inventory_loc_dict = propertyObj.get_multi("property_stock_inventory", "product.template", self.Select(x => x.Product.ProductTmplId));
        //    var inventory_loc = locObj.Table.FirstOrDefault(x => x.Usage == "inventory" && x.ScrapLocation == false);
        //    if (inventory_loc == null)
        //        throw new Exception("Không tìm thấy địa điểm điều chỉnh tồn kho nào.");

        //    var to_create = new List<StockMove>();
        //    foreach (var line in self)
        //    {
        //        if (FloatUtils.FloatCompare((double)(line.TheoreticalQty ?? 0), (double)(line.ProductQty ?? 0),
        //            precisionRounding: (double?)line.Product.ProductTmpl.UOM.Rounding) == 0)
        //            continue;
        //        var diff = (line.TheoreticalQty - line.ProductQty) ?? 0;
        //        StockMove move = null;
        //        //var inventory_loc = locObj.GetById(((StockLocation)inventory_loc_dict[line.Product.ProductTmplId]).Id);
        //        if (diff < 0)
        //            move = _get_move_values(line, Math.Abs(diff), inventory_loc, line.Location);
        //        else
        //            move = _get_move_values(line, Math.Abs(diff), line.Location, inventory_loc);

        //        moveObj._Compute(move);
        //        to_create.Add(move);
        //        //moveObj.Create(move);

        //        //if (diff > 0)
        //        //{
        //        //    Expression<Func<StockQuant, bool>> domain = x => x.Qty > 0 && x.LocationId == line.LocationId && x.LotId == line.ProdLotId;
        //        //    var preferedDomainList = new List<Expression<Func<StockQuant, bool>>>()
        //        //    {
        //        //        x => !x.ReservationId.HasValue,
        //        //        x => x.Reservation.InventoryId != line.InventoryId
        //        //    };
        //        //    var quants = quantObj.QuantsGetPreferedDomain(move.ProductQty ?? 0, move,
        //        //         domain: domain, preferedDomain: preferedDomainList);
        //        //    quantObj.QuantsReserve(quants, move);
        //        //}
        //    }

        //    //moveObj.Create(to_create);
        //    //moveObj._Compute(to_create);
        //    moveObj.Insert2(to_create);
        //    moveObj.SaveChanges();
        //}

        //private StockMove _get_move_values(StockInventoryLine self, decimal qty, StockLocation location, StockLocation locationDest)
        //{
        //    return new StockMove
        //    {
        //        Name = "INV:" + self.Inventory.Name,
        //        Product = self.Product,
        //        ProductId = self.ProductId,
        //        ProductUOM = self.ProductUOM,
        //        ProductUOMId = self.ProductUOMId,
        //        Date = self.Inventory.Date,
        //        InventoryId = self.InventoryId,
        //        ProductUOMQty = qty,
        //        State = "confirmed",
        //        RestrictLotId = self.ProdLotId,
        //        CompanyId = self.Inventory.CompanyId,
        //        Company = self.Inventory.Company,
        //        Sequence = self.Sequence,
        //        Location = location,
        //        LocationId = location.Id,
        //        LocationDestId = locationDest.Id,
        //        LocationDest = locationDest,
        //    };
        //}

        //public override Expression<Func<StockInventoryLine, bool>> RuleDomainGet(IRRule rule)
        //{
        //    var companyObj = GetService<ICompanyService>();
        //    var companyIds = companyObj.GetAllCompanyChildren(CompanyId);
        //    switch (rule.Code)
        //    {
        //        case "stock.stock_inventory_line_comp_rule":
        //            return x => companyIds.Contains(x.CompanyId.Value);
        //        default:
        //            return null;
        //    }
        //}
    }
}
