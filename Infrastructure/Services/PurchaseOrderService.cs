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
    public class PurchaseOrderService : BaseService<PurchaseOrder>, IPurchaseOrderService
    {
        private readonly IMapper _mapper;
        private readonly IUoMService _uoMService;
        public PurchaseOrderService(IAsyncRepository<PurchaseOrder> repository, IHttpContextAccessor httpContextAccessor, IUoMService uoMService,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _uoMService = uoMService;
        }

        public async Task<PagedResult2<PurchaseOrderBasic>> GetPagedResultAsync(PurchaseOrderPaged val)
        {
            ISpecification<PurchaseOrder> spec = new InitialSpecification<PurchaseOrder>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<PurchaseOrder>(x => x.Name.Contains(val.Search) ||
                x.Partner.Name.Contains(val.Search) ||
                x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Phone.Contains(val.Search)));

            string[] typeArr = null;
            if (!string.IsNullOrEmpty(val.Type))
            {
                typeArr = (val.Type).Split(",");
                spec = spec.And(new InitialSpecification<PurchaseOrder>(x => typeArr.Contains(x.Type)));
            }
            if (val.PartnerId.HasValue)
                spec = spec.And(new InitialSpecification<PurchaseOrder>(x => x.PartnerId.Equals(val.PartnerId)));
            spec = spec.And(new InitialSpecification<PurchaseOrder>(x => x.Type == val.Type));
            if (val.DateOrderFrom.HasValue)
            {
                var dateFrom = val.DateOrderFrom.Value.AbsoluteBeginOfDate();
                spec = spec.And(new InitialSpecification<PurchaseOrder>(x => x.DateOrder >= dateFrom));
            }
            if (val.DateOrderTo.HasValue)
            {
                var dateTo = val.DateOrderTo.Value.AbsoluteEndOfDate();
                spec = spec.And(new InitialSpecification<PurchaseOrder>(x => x.DateOrder <= dateTo));
            }
            if (!string.IsNullOrEmpty(val.State))
            {
                var states = val.State.Split(",");
                spec = spec.And(new InitialSpecification<PurchaseOrder>(x => states.Contains(x.State)));
            }

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await query.Select(x => new PurchaseOrderBasic
            {
                Id = x.Id,
                AmountTotal = x.AmountTotal,
                DateOrder = x.DateOrder,
                Name = x.Name,
                PartnerName = x.Partner.Name,
                Type = x.Type,
                State = x.State,
            }).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<PurchaseOrderBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<PurchaseOrderDisplay> GetPurchaseDisplay(Guid id)
        {
            var labo = await SearchQuery(x => x.Id == id).Include(x => x.Partner)
                .Include(x => x.OrderLines)
                .Include("OrderLines.Product")
                .Include("OrderLines.ProductUOM")
                .FirstOrDefaultAsync();
            var res = _mapper.Map<PurchaseOrderDisplay>(labo);
            res.OrderLines = res.OrderLines.OrderBy(x => x.Sequence);
            return res;
        }

        public async Task<PurchaseOrder> CreateLabo(PurchaseOrderDisplay val)
        {
            var purchase = _mapper.Map<PurchaseOrder>(val);
            purchase.CompanyId = CompanyId;
            SaveOrderLines(val, purchase);
            var purchaseLineObj = GetService<IPurchaseOrderLineService>();
            purchaseLineObj._ComputeAmount(purchase.OrderLines);

            _AmountAll(new List<PurchaseOrder>() { purchase });
            await CreateAsync(purchase);
            return purchase;
        }

        public async Task UpdateLabo(Guid id, PurchaseOrderDisplay val)
        {
            var purchase = await SearchQuery(x => x.Id == id).Include(x => x.OrderLines)
                .FirstOrDefaultAsync();
            purchase = _mapper.Map(val, purchase);
            SaveOrderLines(val, purchase);

            var purchaseLineObj = GetService<IPurchaseOrderLineService>();
            purchaseLineObj._ComputeAmount(purchase.OrderLines);

            _AmountAll(new List<PurchaseOrder>() { purchase });
            await UpdateAsync(purchase);
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            var states = new string[] { "draft", "cancel" };
            foreach (var order in self)
            {
                if (!states.Contains(order.State))
                    throw new Exception("Chỉ có thể xóa phiếu mua hàng ở trạng thái nháp hoặc hủy bỏ.");
            }

            await DeleteAsync(self);
        }

        public override async Task<PurchaseOrder> CreateAsync(PurchaseOrder purchase)
        {
            if (string.IsNullOrEmpty(purchase.Name) || purchase.Name == "/")
            {
                var sequenceObj = GetService<IIRSequenceService>();
                if (purchase.Type == "refund")
                    purchase.Name = await sequenceObj.NextByCode("purchase.refund");
                else
                    purchase.Name = await sequenceObj.NextByCode("purchase.order");
            }
            return await base.CreateAsync(purchase);
        }

        public override ISpecification<PurchaseOrder> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "purchase.purchase_order_comp_rule":
                    return new InitialSpecification<PurchaseOrder>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }

        public PurchaseOrderDisplay DefaultGet(PurchaseOrderDefaultGet val)
        {
            var res = new PurchaseOrderDisplay();
            res.Type = val.Type;
            var companyId = CompanyId;
            var pickingTypeObj = GetService<IStockPickingTypeService>();
            var pickingType = pickingTypeObj.SearchQuery(x => x.Code == "incoming" && x.Warehouse.CompanyId == companyId).FirstOrDefault();
            if (pickingType == null)
                pickingType = pickingTypeObj.SearchQuery(x => x.Code == "incoming" && x.Warehouse == null).FirstOrDefault();
            if (pickingType == null)
                throw new Exception("Không tìm thấy hoạt động nhập kho nào");
            res.PickingTypeId = pickingType.Id;

            return res;
        }

        public void _AmountAll(IEnumerable<PurchaseOrder> orders)
        {
            foreach (var order in orders)
            {
                var totalAmountUntaxed = 0M;

                foreach (var orderLine in order.OrderLines)
                {
                    totalAmountUntaxed += (orderLine.PriceSubtotal ?? 0);
                }

                order.AmountTotal = totalAmountUntaxed;
            }
        }

        private void SaveOrderLines(PurchaseOrderDisplay val, PurchaseOrder order)
        {
            var existLines = order.OrderLines.ToList();
            var lineToRemoves = new List<PurchaseOrderLine>();
            foreach (var existLine in existLines)
            {
                bool found = false;
                foreach (var item in val.OrderLines)
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
            {
                if (line.State != "draft")
                    continue;
                order.OrderLines.Remove(line);
            }

            //Cập nhật sequence cho tất cả các line của val
            int sequence = 0;
            foreach (var line in val.OrderLines)
            {
                line.Sequence = sequence++;
            }

            foreach (var line in val.OrderLines)
            {
                if (line.Id == Guid.Empty)
                {
                    var lbLine = _mapper.Map<PurchaseOrderLine>(line);
                    lbLine.CompanyId = order.CompanyId;
                    lbLine.PartnerId = order.PartnerId;
                    lbLine.State = order.State;
                    order.OrderLines.Add(lbLine);
                }
                else
                {
                    var lbLine = order.OrderLines.SingleOrDefault(c => c.Id == line.Id);
                    if (lbLine != null)
                    {
                        _mapper.Map(line, lbLine);
                    }
                }
            }
        }

        public async Task ButtonConfirm(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.OrderLines).Include("OrderLines.Product")
                .Include("OrderLines.ProductUOM")
                .Include(x => x.PickingType).Include(x => x.PickingType.DefaultLocationDest)
                .Include(x => x.PickingType.DefaultLocationSrc)
                .ToListAsync();
            foreach (var order in self)
            {
                if (order.State != "draft" && order.State != "sent")
                    continue;

                order.State = "purchase";
                foreach (var line in order.OrderLines)
                    line.State = "purchase";
            }

            var invoices = await _CreateInvoices(self);
            var moveObj = GetService<IAccountMoveService>();
            await moveObj.ActionPost(invoices);

            var poLineObj = GetService<IPurchaseOrderLineService>();
            poLineObj._ComputeQtyInvoiced(self.SelectMany(x => x.OrderLines));

            await _CreatePicking(self);

            _GetInvoiced(self);
            await UpdateAsync(self);
        }

        public void _GetInvoiced(IEnumerable<PurchaseOrder> self)
        {
            foreach (var order in self)
            {
                if (order.State != "purchase" && order.State != "done")
                {
                    order.InvoiceStatus = "no";
                    continue;
                }

                if (order.OrderLines.Any(x => (x.QtyInvoiced ?? 0) < x.ProductQty))
                    order.InvoiceStatus = "to invoice";
                else if (order.OrderLines.All(x => (x.QtyInvoiced ?? 0) >= x.ProductQty))
                    order.InvoiceStatus = "invoiced";
                else
                    order.InvoiceStatus = "no";
            }
        }

        public async Task<IEnumerable<AccountMove>> _CreateInvoices(IEnumerable<PurchaseOrder> self)
        {
            var poLineObj = GetService<IPurchaseOrderLineService>();
            var invoice_vals_list = new List<AccountMove>();
            foreach (var order in self)
            {
                var invoice_vals = await PrepareInvoice(order);
                foreach (var poLine in order.OrderLines)
                {

                    invoice_vals.InvoiceLines.Add(poLineObj._PrepareAccountMoveLine(poLine, invoice_vals));
                }

                if (!invoice_vals.InvoiceLines.Any())
                    throw new Exception("There is no invoiceable line. If a product has a Delivered quantities invoicing policy, please make sure that a quantity has been delivered.");

                invoice_vals_list.Add(invoice_vals);
            }

            var in_invoice_vals_list = invoice_vals_list.Where(x => x.Type == "in_invoice").ToList();
            var refund_invoice_vals_list = invoice_vals_list.Where(x => x.Type == "in_refund").ToList();

            var moveObj = GetService<IAccountMoveService>();
            var moves = await moveObj.CreateMoves(in_invoice_vals_list, default_type: "in_invoice");
            moves = moves.Concat(await moveObj.CreateMoves(refund_invoice_vals_list, default_type: "in_refund"));

            return moves;
        }

        private async Task<AccountMove> PrepareInvoice(PurchaseOrder self)
        {
            var accountMoveObj = GetService<IAccountMoveService>();
            var journal = await accountMoveObj.GetDefaultJournalAsync(default_type: "in_invoice");
            if (journal == null)
                throw new Exception($"Please define an accounting purchase journal for the company {CompanyId}.");

            return new AccountMove()
            {
                Type = self.Type == "refund" ? "in_refund" : "in_invoice",
                PartnerId = self.PartnerId,
                JournalId = journal.Id,
                Journal = journal,
                CompanyId = self.CompanyId,
                InvoiceUserId = UserId,
                InvoiceOrigin = self.Name,
            };
        }

        public async Task _CreatePicking(IEnumerable<PurchaseOrder> orders)
        {
            var pickingObj = GetService<IStockPickingService>();
            var moveObj = GetService<IStockMoveService>();
            foreach (var order in orders)
            {
                if (order.OrderLines.Any(x => x.Product.Type == "product" || x.Product.Type == "consu"))
                {

                    StockPicking picking = _PreparePicking(order);
                    await pickingObj.CreateAsync(picking);

                    var moves = await CreateStockMoves(order.OrderLines, picking);

                    await pickingObj.ActionDone(new List<Guid>() { picking.Id });
                }
            }
        }

        public async Task CreateReturns(IEnumerable<Guid> pickingIds)
        {
            var moveObj = GetService<IStockMoveService>();
            var pickObj = GetService<IStockPickingService>();
            var returnedLines = new List<StockMove>();
            var pickings = await pickObj.SearchQuery(x => pickingIds.Contains(x.Id)).Include(x => x.MoveLines)
                .Include(x => x.PickingType)
                .Include("MoveLines.Location")
                .Include("MoveLines.LocationDest")
                .Include("MoveLines.Product")
                .ToListAsync();
            foreach (var pick in pickings)
            {
                if (!pick.PickingType.ReturnPickingTypeId.HasValue)
                    throw new Exception("Không tìm thấy hoạt động để trả hàng");
                var newPicking = new StockPicking(pick)
                {
                    PickingTypeId = pick.PickingType.ReturnPickingTypeId.Value,
                    State = "draft",
                    Origin = pick.Name,
                    LocationId = pick.LocationDestId,
                    LocationDestId = pick.LocationId,
                    CompanyId = pick.CompanyId,
                };

                await pickObj.CreateAsync(newPicking);

                foreach (var move in pick.MoveLines)
                {
                    var location = move.Location;
                    var returnMove = new StockMove(move)
                    {
                        ProductId = move.ProductId,
                        Product = move.Product,
                        ProductUOMQty = move.ProductUOMQty,
                        PickingId = newPicking.Id,
                        Picking = newPicking,
                        State = "draft",
                        LocationId = move.LocationDestId,
                        Location = move.LocationDest,
                        LocationDestId = location.Id,
                        LocationDest = location,
                        PickingTypeId = pick.PickingTypeId,
                        WarehouseId = pick.PickingType.WarehouseId,
                        //OriginReturnedMoveId = move.Id,
                        CompanyId = newPicking.CompanyId,
                    };

                    returnedLines.Add(returnMove);
                }

                if (!returnedLines.Any())
                    throw new Exception("Không có dòng nào có thể trả hàng.");
                await moveObj.CreateAsync(returnedLines);

                await pickObj.ActionDone(new List<Guid>() { newPicking.Id });
            }
        }

        public async Task ButtonCancel(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.OrderLines)
                .Include("OrderLines.InvoiceLines")
                .ToListAsync();
            var invoiceIds = new List<Guid>().AsEnumerable();
            foreach (var order in self)
            {
                invoiceIds = invoiceIds.Union(order.OrderLines.SelectMany(x => x.InvoiceLines)
                    .Where(x => x.InvoiceId.HasValue).Select(x => x.InvoiceId.Value).Distinct().ToList());
            }
            if (invoiceIds.Any())
            {
                var invObj = GetService<IAccountInvoiceService>();
                await invObj.ActionCancel(invoiceIds);
            }

            foreach (var order in self)
            {
                order.State = "cancel";
                foreach (var line in order.OrderLines)
                    line.State = "cancel";

                //tìm tất cả những phiếu nhập để tạo phiếu trả hàng
                var pickingIds = GetPickingIds(order.Id);
                if (pickingIds.Any())
                {
                    await CreateReturns(pickingIds);
                }
            }

            _GetInvoiced(self);
            await UpdateAsync(self);
        }

        public IEnumerable<Guid> GetPickingIds(Guid orderId)
        {
            var moveObj = GetService<IStockMoveService>();
            return moveObj.SearchQuery(x => x.PurchaseLine.OrderId == orderId && x.PickingId.HasValue)
                .Select(x => x.PickingId.Value).Distinct().ToList();
        }


        public async Task<IEnumerable<StockMove>> CreateStockMoves(IEnumerable<PurchaseOrderLine> self, StockPicking picking)
        {
            var moveObj = GetService<IStockMoveService>();
            var done = new List<StockMove>();
            foreach (var line in self)
            {
                var moves = _PrepareStockMoves(line, picking);
                if (moves.Any())
                    done.AddRange(moves);
            }

            await moveObj.CreateAsync(done);
            return done;
        }

        private IEnumerable<StockMove> _PrepareStockMoves(PurchaseOrderLine self, StockPicking picking)
        {
            var res = new List<StockMove>();
            var productTypes = new string[] { "product", "consu" };
            if (!productTypes.Contains(self.Product.Type))
                return res;
            var priceUnit = _GetStockMovePriceUnit(self);

            var template = new StockMove
            {
                Name = self.Name,
                ProductId = self.ProductId.Value,
                Product = self.Product,
                ProductUOMId = self.ProductUOMId,
                ProductUOM = self.ProductUOM,
                DateExpected = self.DatePlanned,
                Date = self.Order.DateOrder,
                LocationId = picking.LocationId,
                LocationDestId = picking.LocationDestId,
                State = "draft",
                Origin = self.Order.Name,
                PickingId = picking.Id,
                PartnerId = self.Order.PartnerId,
                PurchaseLineId = self.Id,
                PriceUnit = (double)priceUnit,
                PickingTypeId = self.Order.PickingTypeId,
                WarehouseId = self.Order.PickingType.WarehouseId,
                CompanyId = self.Order.CompanyId,
                Sequence = self.Sequence ?? 0,
                ProductUOMQty = Math.Round(self.ProductQty * decimal.Parse((self.ProductUOM != null && self.ProductUOM.Factor != 0 ? 1 / self.ProductUOM.Factor : 0).ToString()), 0)
            };
            res.Add(template);
            return res;
        }
        private decimal _GetStockMovePriceUnit(PurchaseOrderLine line)
        {
            var priceUnit = line.PriceUnit;
            return priceUnit;
        }

        private StockPicking _PreparePicking(PurchaseOrder order)
        {
            var locationObj = GetService<IStockLocationService>();
            var location = locationObj.SearchQuery(x => x.Usage == "supplier").FirstOrDefault();
            if (location == null)
                throw new Exception("Không tìm thấy địa điểm nhà cung cấp");

            var locationDest = _GetDestinationLocation(order);
            var res = new StockPicking
            {
                PickingTypeId = order.PickingTypeId,
                PartnerId = order.PartnerId,
                Date = order.DateOrder,
                Origin = order.Name,
                CompanyId = order.CompanyId,
            };

            if (order.Type == "refund")
            {
                res.LocationId = locationDest.Id;
                res.LocationDestId = location.Id;
                res.PickingTypeId = order.PickingType.ReturnPickingTypeId.Value;
            }
            else
            {
                res.LocationId = location.Id;
                res.LocationDestId = locationDest.Id;
            }

            return res;
        }

        public StockLocation _GetDestinationLocation(PurchaseOrder order)
        {
            if (order.PickingType.DefaultLocationDest == null)
                throw new Exception("Vui lòng xác định địa điểm đến cho hoạt động " + order.PickingType.Name);
            return order.PickingType.DefaultLocationDest;
        }
    }
}
