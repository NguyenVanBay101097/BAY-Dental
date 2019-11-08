using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
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
        public PurchaseOrderService(IAsyncRepository<PurchaseOrder> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<PurchaseOrderBasic>> GetPagedResultAsync(PurchaseOrderPaged val)
        {
            ISpecification<PurchaseOrder> spec = new InitialSpecification<PurchaseOrder>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<PurchaseOrder>(x => x.Name.Contains(val.Search) ||
                x.Partner.Name.Contains(val.Search) ||
                x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Phone.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await query.Select(x => new PurchaseOrderBasic
            {
                Id = x.Id,
                AmountTotal = x.AmountTotal,
                DateOrder = x.DateOrder,
                Name = x.Name,
                PartnerName = x.Partner.Name,
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
                purchase.Name = await sequenceObj.NextByCode("purchase.order");
            }
            return await base.CreateAsync(purchase);
        }

        public PurchaseOrderDisplay DefaultGet(PurchaseOrderDefaultGet val)
        {
            var res = new PurchaseOrderDisplay();
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


            var invObj = GetService<IAccountInvoiceService>();
            var invoices = await ActionInvoiceCreate(self);
            await invObj.ActionInvoiceOpen(invoices.Select(x => x.Id).ToList());

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
                else if (order.OrderLines.Any(x => (x.QtyInvoiced ?? 0) >= x.ProductQty))
                    order.InvoiceStatus = "invoiced";
                else
                    order.InvoiceStatus = "no";
            }
        }

        public async Task<IList<AccountInvoice>> ActionInvoiceCreate(IEnumerable<PurchaseOrder> self)
        {
            var invObj = GetService<IAccountInvoiceService>();
            var invLineObj = GetService<IAccountInvoiceLineService>();
            var res = new List<AccountInvoice>();
            foreach (var order in self)
            {
                var invData = await PrepareInvoice(order);
                res.Add(invData);
                var invLines = new List<AccountInvoiceLine>();
                foreach (var poLine in order.OrderLines)
                {
                    if (poLine.State == "cancel")
                        continue;

                    var invLineData = await invObj.PrepareInvoiceLineFromPOLine(invData, poLine, journal: invData.Journal, type: "in_invoice");
                    invLines.Add(invLineData);
                }

                invData.InvoiceLines = invLines;
                invLineObj.ComputePrice(invData.InvoiceLines);
                invObj._ComputeAmount(invData);
                await invObj.CreateAsync(invData);
            }

            return res;
        }

        private async Task<AccountInvoice> PrepareInvoice(PurchaseOrder order)
        {
            var journalObj = GetService<IAccountJournalService>();
            var journal = await journalObj.SearchQuery(x => x.Type == "purchase").Include(x => x.DefaultDebitAccount)
                .Include(x => x.DefaultCreditAccount).FirstOrDefaultAsync();
            if (journal == null)
                throw new Exception("Không tìm thấy nhật ký mua hàng");
            var accountObj = GetService<IAccountAccountService>();
            var account = await accountObj.GetAccountPayableCurrentCompany();

            return new AccountInvoice()
            {
                Name = !string.IsNullOrEmpty(order.PartnerRef) ? order.PartnerRef : order.Name,
                AccountId = account.Id,
                Type = "in_invoice",
                PartnerId = order.PartnerId,
                Origin = order.Name,
                JournalId = journal.Id,
                Journal = journal,
                CompanyId = order.CompanyId,
                UserId = UserId,
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
            }

            //tìm tất cả những phiếu nhập để tạo phiếu trả hàng

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
                LocationDestId = _GetDestinationLocation(self.Order).Id,
                LocationDest = _GetDestinationLocation(self.Order),
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
            var res = new StockPicking
            {
                PickingTypeId = order.PickingTypeId,
                PartnerId = order.PartnerId,
                Date = order.DateOrder,
                Origin = order.Name,
                CompanyId = order.CompanyId,
                LocationDest = _GetDestinationLocation(order),
                LocationId = location.Id,
            };

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
