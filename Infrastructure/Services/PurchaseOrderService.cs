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
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(x.Name));

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateOrder >= val.DateFrom.Value);


            if (val.DateTo.HasValue)
            {
                var datetimeTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.DateOrder <= datetimeTo);
            }

            if (!string.IsNullOrEmpty(val.Type))
                query = query.Where(x => x.Type == val.Type);

            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State == val.State);

            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId);

            var totalItems = await query.CountAsync();

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var items = await query.ToListAsync();

            return new PagedResult2<PurchaseOrderBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<PurchaseOrderBasic>>(items)
            };
        }

        private async Task<IDictionary<Guid, decimal>> _ComputeAmountResidualDict(IEnumerable<Guid> ids)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var moveObj = GetService<IAccountMoveService>();

            var groups = await amlObj.SearchQuery(x => ids.Contains(x.PurchaseLine.OrderId))
                .Select(x => new
                {
                    OrderId = x.PurchaseLine.OrderId,
                    MoveId = x.MoveId
                }).Distinct().ToListAsync();

            var move_ids = groups.Select(x => x.MoveId).ToList();
            var moves = await moveObj.SearchQuery(x => move_ids.Contains(x.Id)).ToListAsync();
            var res = ids.ToDictionary(x => x, x => 0M);
            foreach (var group in groups)
            {
                var move = moves.FirstOrDefault(x => x.Id == group.MoveId);
                if (move != null)
                    res[group.OrderId] += (move.AmountResidual ?? 0);
            }

            return res;
        }

        public async Task<PurchaseOrderDisplay> GetPurchaseDisplay(Guid id)
        {
            var lineObj = GetService<IPurchaseOrderLineService>();
            var purchase = await SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .Include(x => x.Journal)
                .Include(x => x.User)
                .Include(x => x.PickingType)
                .FirstOrDefaultAsync();

            purchase.OrderLines = await lineObj.SearchQuery(x => x.OrderId == purchase.Id).Include(x => x.Product).Include(x => x.ProductUOM).OrderBy(x => x.Sequence).ToListAsync();
            var res = _mapper.Map<PurchaseOrderDisplay>(purchase);

            return res;
        }

        private async Task<decimal> _ComputeTotalPaid(PurchaseOrder self)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var moveObj = GetService<IAccountMoveService>();
            var line_ids = self.OrderLines.Select(x => x.Id);
            var move_ids = await amlObj.SearchQuery(x => line_ids.Contains(x.PurchaseLineId.Value)).Select(x => x.MoveId).Distinct().ToListAsync();
            var res = await moveObj.SearchQuery(x => move_ids.Contains(x.Id)).SumAsync(x => x.AmountResidual);
            return res ?? 0;
        }

        public async Task<PurchaseOrder> CreatePurchaseOrder(PurchaseOrderSave val)
        {
            var purchase = _mapper.Map<PurchaseOrder>(val);
            purchase.CompanyId = CompanyId;
            purchase.UserId = UserId;

            var lines = new List<PurchaseOrderLine>();
            var sequence = 0;
            foreach (var item in val.OrderLines)
            {
                var line = _mapper.Map<PurchaseOrderLine>(item);
                line.PartnerId = purchase.PartnerId;
                line.CompanyId = purchase.CompanyId;
                line.State = purchase.State;
                line.Sequence = sequence++;
                purchase.OrderLines.Add(line);
            }

            var purchaseLineObj = GetService<IPurchaseOrderLineService>();
            purchaseLineObj._ComputeAmount(purchase.OrderLines);

            _AmountAll(new List<PurchaseOrder>() { purchase });
            await CreateAsync(purchase);
            return purchase;
        }

        public async Task UpdatePurchaseOrder(Guid id, PurchaseOrderSave val)
        {
            var purchase = await SearchQuery(x => x.Id == id)
                .Include(x => x.OrderLines)
                .ThenInclude(x => x.MoveLines)
                .Include(x => x.Journal)
                .FirstOrDefaultAsync();

            purchase = _mapper.Map(val, purchase);
            SavePurchaseLines(val, purchase);

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
                    throw new Exception("Chỉ có thể xóa phiếu mua hàng ở trạng thái nháp.");
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
            var journalObj = GetService<IAccountJournalService>();
            var pickingTypeObj = GetService<IStockPickingTypeService>();
            var companyId = CompanyId;

            var pickingType = pickingTypeObj.SearchQuery(x => x.Code == "incoming" && x.Warehouse.CompanyId == CompanyId).FirstOrDefault();
            if (pickingType == null)
                pickingType = pickingTypeObj.SearchQuery(x => x.Code == "incoming" && x.Warehouse == null).FirstOrDefault();
            if (pickingType == null)
                throw new Exception("Không tìm thấy hoạt động nhập kho nào");

            var cashJournal = journalObj.SearchQuery(x => x.Type == "cash" && x.CompanyId == CompanyId).FirstOrDefault();
            if (cashJournal == null)
                throw new Exception("Không tìm thấy phương thức thanh toán tiền mặt");

            var res = new PurchaseOrderDisplay();
            res.Type = val.Type;                          
            res.PickingTypeId = pickingType.Id;                  
            res.JournalId = cashJournal.Id;
            res.Journal = _mapper.Map<AccountJournalSimple>(cashJournal);
            res.AmountPayment = 0;

            return res;
        }

        public void _AmountAll(IEnumerable<PurchaseOrder> orders)
        {
            var moveObj = GetService<IAccountMoveService>();
            foreach (var order in orders)
            {
                var totalAmountUntaxed = 0M;

                foreach (var orderLine in order.OrderLines)
                {
                    totalAmountUntaxed += (orderLine.PriceSubtotal ?? 0);

                }

                var move_ids = order.OrderLines.SelectMany(x => x.MoveLines).Select(x => x.MoveId).Distinct().ToList();
                var totalAmountResidual = moveObj.SearchQuery(x => move_ids.Contains(x.Id)).Sum(x => (x.AmountResidual ?? 0));

                order.AmountTotal = totalAmountUntaxed;
                order.AmountResidual = totalAmountResidual;
                if (order.AmountPayment.HasValue)
                    order.AmountPayment = order.AmountPayment > totalAmountUntaxed ? totalAmountUntaxed : order.AmountPayment;
            }
        }

        private void SavePurchaseLines(PurchaseOrderSave val, PurchaseOrder purchase)
        {
            var existLines = purchase.OrderLines.ToList();
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
                purchase.OrderLines.Remove(line);
            }

            //Cập nhật sequence cho tất cả các line của val
            int sequence = 0;

            foreach (var line in val.OrderLines)
            {
                if (line.Id == Guid.Empty)
                {
                    var lbLine = _mapper.Map<PurchaseOrderLine>(line);
                    lbLine.PartnerId = purchase.PartnerId;
                    lbLine.CompanyId = purchase.CompanyId;
                    lbLine.State = purchase.State;
                    lbLine.Sequence = sequence++;
                    purchase.OrderLines.Add(lbLine);
                }
                else
                {
                    var lbLine = purchase.OrderLines.SingleOrDefault(c => c.Id == line.Id);
                    if (lbLine != null)
                    {

                        _mapper.Map(line, lbLine);
                        lbLine.Sequence = sequence++;

                    }
                }
            }
        }

        public async Task ButtonConfirm(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.OrderLines).ThenInclude(x => x.MoveLines)
                .Include("OrderLines.Product")
                .Include("OrderLines.ProductUOM").Include("OrderLines.Product.UOM")
                .Include(x => x.PickingType).Include(x => x.PickingType.DefaultLocationDest)
                .Include(x => x.PickingType.DefaultLocationSrc)
                .Include(x => x.Journal)
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

            await _CreatePayment(self);

            _GetInvoiced(self);
            _AmountAll(self);
            ComputeState(self);

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

        public async Task _CreatePayment(IEnumerable<PurchaseOrder> self)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var paymentObj = GetService<IAccountPaymentService>();

            var payments = new List<AccountPayment>();
            foreach (var order in self)
            {
                if (!order.JournalId.HasValue || order.AmountPayment <= 0)
                    continue;

                var payment = new AccountPayment
                {
                    JournalId = order.JournalId.Value,
                    PartnerId = order.PartnerId,
                    Amount = (order.AmountPayment ?? 0),
                    PartnerType = "supplier",
                    PaymentDate = DateTime.Now,
                    PaymentType = order.Type == "order" ? "outbound" : "inbound",
                    CompanyId = order.CompanyId
                };
                var line_ids = order.OrderLines.Select(x => x.Id).ToList();
                var invoice_id = await amlObj.SearchQuery(x => x.PurchaseLineId.HasValue && line_ids.Contains(x.PurchaseLineId.Value)).Select(x => x.MoveId).Distinct().FirstOrDefaultAsync();
                payment.AccountMovePaymentRels.Add(new AccountMovePaymentRel { MoveId = invoice_id });
                payments.Add(payment);

            }

            await paymentObj.CreateAsync(payments);
            await paymentObj.Post(payments.Select(x => x.Id).ToList());

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
            _AmountAll(self);
            await UpdateAsync(self);
        }

        public IEnumerable<Guid> GetPickingIds(Guid orderId)
        {
            var moveObj = GetService<IStockMoveService>();
            return moveObj.SearchQuery(x => x.PurchaseLine.OrderId == orderId && x.PickingId.HasValue)
                .Select(x => x.PickingId.Value).Distinct().ToList();
        }

        public void ComputeState(IEnumerable<PurchaseOrder> self)
        {          
            var states = new string[] { "draft", "cancel" };
            foreach (var order in self)
            {
                if (states.Contains(order.State) || order.AmountResidual <= 0)
                    continue;            

                order.State = "done";
                foreach (var line in order.OrderLines)
                    line.State = "done";
            }
        }

        public async Task PreparePurchase(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
              .Include(x => x.OrderLines).ThenInclude(x => x.MoveLines)             
              .Include(x => x.Journal)
              .ToListAsync();

            var poLineObj = GetService<IPurchaseOrderLineService>();
            poLineObj._ComputeQtyInvoiced(self.SelectMany(x => x.OrderLines));

            _GetInvoiced(self);
            _AmountAll(self);
            ComputeState(self);

            await UpdateAsync(self);
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
                PriceUnit = priceUnit,
                PickingTypeId = self.Order.PickingTypeId,
                WarehouseId = self.Order.PickingType.WarehouseId,
                CompanyId = self.Order.CompanyId,
                Sequence = self.Sequence ?? 0,
                ProductUOMQty = self.ProductQty
            };
            res.Add(template);
            return res;
        }
        private double _GetStockMovePriceUnit(PurchaseOrderLine line)
        {
            var priceUnit = (double)line.PriceUnit;
            if (line.ProductUOM.Id != line.Product.UOMId)
            {
                priceUnit *= line.ProductUOM.Factor / line.Product.UOM.Factor;
            }
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
