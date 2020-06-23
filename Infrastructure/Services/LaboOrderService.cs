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
    public class LaboOrderService : BaseService<LaboOrder>, ILaboOrderService
    {
        private readonly IMapper _mapper;
        public LaboOrderService(IAsyncRepository<LaboOrder> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<LaboOrderBasic>> GetPagedResultAsync(LaboOrderPaged val)
        {
            ISpecification<LaboOrder> spec = new InitialSpecification<LaboOrder>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.Name.Contains(val.Search) ||
                x.Partner.Name.Contains(val.Search) ||
                x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Phone.Contains(val.Search) ||
                x.SaleOrder.Name.Contains(val.Search)));
            if (val.PartnerId.HasValue)
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.PartnerId == val.PartnerId));

            if (val.SaleOrderId.HasValue)
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.SaleOrderId == val.SaleOrderId));

            if (val.DateOrderFrom.HasValue)
            {
                var dateFrom = val.DateOrderFrom.Value.AbsoluteBeginOfDate();
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.DateOrder >= dateFrom));
            }
            if (val.DateOrderTo.HasValue)
            {
                var dateTo = val.DateOrderTo.Value.AbsoluteEndOfDate();
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.DateOrder <= dateTo));
            }

            if (val.DatePlannedFrom.HasValue)
            {
                var dateFrom = val.DatePlannedFrom.Value.AbsoluteBeginOfDate();
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.DatePlanned >= dateFrom));
            }
            if (val.DatePlannedTo.HasValue)
            {
                var dateTo = val.DatePlannedTo.Value.AbsoluteEndOfDate();
                spec = spec.And(new InitialSpecification<LaboOrder>(x => x.DatePlanned <= dateTo));
            }

            if (!string.IsNullOrEmpty(val.State))
            {
                var states = val.State.Split(",");
                spec = spec.And(new InitialSpecification<LaboOrder>(x => states.Contains(x.State)));
            }

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await _mapper.ProjectTo<LaboOrderBasic>(query).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<LaboOrderBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<PagedResult2<LaboOrderStatisticsBasic>> GetStatisticsPaged(LaboOrderStatisticsPaged val)
        {
            ISpecification<LaboOrderLine> spec = new InitialSpecification<LaboOrderLine>(x => true);
           
            if (val.PartnerId.HasValue)
                spec = spec.And(new InitialSpecification<LaboOrderLine>(x => x.PartnerId == val.PartnerId));

            if (val.ProductId.HasValue)
                spec = spec.And(new InitialSpecification<LaboOrderLine>(x => x.ProductId == val.ProductId));

            if (val.DateOrderFrom.HasValue)
                spec = spec.And(new InitialSpecification<LaboOrderLine>(x => x.Order.DateOrder >= val.DateOrderFrom));

            if (val.DateOrderTo.HasValue)
            {
                var dateOrderTo = val.DateOrderTo.Value.AbsoluteEndOfDate();
                spec = spec.And(new InitialSpecification<LaboOrderLine>(x => x.Order.DateOrder <= dateOrderTo));
            }

            if (val.DatePlannedFrom.HasValue)
                spec = spec.And(new InitialSpecification<LaboOrderLine>(x => x.Order.DatePlanned >= val.DatePlannedFrom));

            if (val.DatePlannedTo.HasValue)
            {
                var datePlannedTo = val.DatePlannedTo.Value.AbsoluteEndOfDate();
                spec = spec.And(new InitialSpecification<LaboOrderLine>(x => x.Order.DatePlanned <= datePlannedTo));
            }

            var lineObj = GetService<ILaboOrderLineService>();

            var query = lineObj.SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await _mapper.ProjectTo<LaboOrderStatisticsBasic>(query).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<LaboOrderStatisticsBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public override ISpecification<LaboOrder> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "labo.labo_order_comp_rule":
                    return new InitialSpecification<LaboOrder>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }

        public async Task<IEnumerable<LaboOrderBasic>> GetAllForDotKham(Guid dotKhamId)
        {
            var res = await SearchQuery(x => x.DotKhamId == dotKhamId).Select(x => new LaboOrderBasic
            {
                Id = x.Id,
                AmountTotal = x.AmountTotal,
                DateOrder = x.DateOrder,
                Name = x.Name,
                PartnerName = x.Partner.Name,
                State = x.State,
                CustomerName = x.Customer.Name
            }).ToListAsync();
            return res;
        }

        public async Task<LaboOrderDisplay> GetLaboDisplay(Guid id)
        {
            //var res = await SearchQuery(x => x.Id == id).Select(x => new LaboOrderDisplay
            //{
            //    Id = x.Id,
            //    AmountTotal = x.AmountTotal,
            //    DateOrder = x.DateOrder,
            //    DatePlanned = x.DatePlanned,
            //    DotKhamId = x.DotKhamId,
            //    Name = x.Name,
            //    PartnerId = x.PartnerId,
            //    PartnerRef = x.PartnerRef,
            //    State = x.State
            //}).FirstOrDefaultAsync();

            //var partnerObj = GetService<IPartnerService>();
            //res.Partner = await partnerObj.SearchQuery(x => x.Id == res.PartnerId).Select(x => new PartnerSimple
            //{
            //    Id = x.Id,
            //    Name = x.Name
            //}).FirstOrDefaultAsync();
            var labo = await SearchQuery(x => x.Id == id).Include(x => x.Partner)
                .Include(x => x.DotKham).Include(x => x.Customer).Include(x => x.SaleOrder)
                .Include(x => x.OrderLines)
                .Include("OrderLines.Product")
                .Include("OrderLines.ToothCategory")
                .Include("OrderLines.LaboOrderLineToothRels")
                .Include("OrderLines.LaboOrderLineToothRels.Tooth").FirstOrDefaultAsync();
            var res = _mapper.Map<LaboOrderDisplay>(labo);
            res.OrderLines = res.OrderLines.OrderBy(x => x.Sequence);
            return res;
        }

        public async Task<LaboOrder> CreateLabo(LaboOrderDisplay val)
        {
            var labo = _mapper.Map<LaboOrder>(val);
            labo.CompanyId = CompanyId;
            SaveOrderLines(val, labo);

            var lbLineObj = GetService<ILaboOrderLineService>();
            lbLineObj._ComputeAmount(labo.OrderLines);

            _AmountAll(new List<LaboOrder>() { labo });
            await CreateAsync(labo);
            return labo;
        }

        public void _AmountAll(IEnumerable<LaboOrder> orders)
        {
            foreach (var order in orders)
            {
                var totalAmountUntaxed = 0M;

                foreach (var orderLine in order.OrderLines)
                {
                    totalAmountUntaxed += orderLine.PriceSubtotal;
                }

                order.AmountTotal = totalAmountUntaxed;
            }
        }

        public async Task UpdateLabo(Guid id, LaboOrderDisplay val)
        {
            var labo = await SearchQuery(x => x.Id == id).Include(x => x.OrderLines)
                .Include("OrderLines.LaboOrderLineToothRels")
                .FirstOrDefaultAsync();
            labo = _mapper.Map(val, labo);
            SaveOrderLines(val, labo);

            var lbLineObj = GetService<ILaboOrderLineService>();
            lbLineObj._ComputeAmount(labo.OrderLines);

            _AmountAll(new List<LaboOrder>() { labo });
            await UpdateAsync(labo);
        }

        public async Task<IEnumerable<AccountMove>> _CreateInvoices(IEnumerable<LaboOrder> self)
        {
            var lbLineObj = GetService<ILaboOrderLineService>();
            var invoice_vals_list = new List<AccountMove>();
            foreach (var order in self)
            {
                var invoice_vals = await PrepareInvoice(order);
                foreach (var poLine in order.OrderLines)
                {
                    invoice_vals.InvoiceLines.Add(lbLineObj._PrepareAccountMoveLine(poLine, invoice_vals));
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

        private async Task<AccountMove> PrepareInvoice(LaboOrder self)
        {
            var accountMoveObj = GetService<IAccountMoveService>();
            var journal = await accountMoveObj.GetDefaultJournalAsync(default_type: "in_invoice");
            if (journal == null)
                throw new Exception($"Please define an accounting purchase journal for the company {CompanyId}.");

            return new AccountMove()
            {
                Type = "in_invoice",
                PartnerId = self.PartnerId,
                JournalId = journal.Id,
                Journal = journal,
                CompanyId = self.CompanyId,
                InvoiceUserId = UserId,
                InvoiceOrigin = self.Name,
            };
        }

        public async Task ButtonConfirm(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.OrderLines).Include("OrderLines.Product")
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

            var lbLineObj = GetService<ILaboOrderLineService>();
            lbLineObj._ComputeQtyInvoiced(self.SelectMany(x => x.OrderLines));

            await UpdateAsync(self);
        }

        public async Task ButtonCancel(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.OrderLines)
                .Include("OrderLines.MoveLines")
                .ToListAsync();

            var move_ids = new List<Guid>().AsEnumerable();
            foreach (var order in self)
                move_ids = move_ids.Union(order.OrderLines.SelectMany(x => x.MoveLines).Select(x => x.MoveId).Distinct().ToList());

            if (move_ids.Any())
            {
                var moveObj = GetService<IAccountMoveService>();
                await moveObj.ButtonDraft(move_ids);

                await moveObj.Unlink(move_ids);
            }

            foreach (var order in self)
            {
                order.State = "draft";
                foreach (var line in order.OrderLines)
                    line.State = "draft";
            }

            var lbLineObj = GetService<ILaboOrderLineService>();
            lbLineObj._ComputeQtyInvoiced(self.SelectMany(x => x.OrderLines));

            await UpdateAsync(self);
        }

        public async Task ActionDone(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.OrderLines).ToListAsync();
            foreach (var order in self)
            {
                foreach (var line in order.OrderLines)
                    line.State = "done";

                order.State = "done";
            }

            await UpdateAsync(self);
        }

        public async Task<LaboOrderDisplay> DefaultGet(LaboOrderDefaultGet val)
        {
            var res = new LaboOrderDisplay();
            if (val.DotKhamId.HasValue)
            {
                var dkObj = GetService<IDotKhamService>();
                var dk = await dkObj.SearchQuery(x => x.Id == val.DotKhamId).Include(x => x.Partner).FirstOrDefaultAsync();
                if (dk != null)
                    res.Customer = _mapper.Map<PartnerSimple>(dk.Partner);
                res.CustomerId = dk.PartnerId;
                res.DotKhamId = dk.Id;
            }

            if (val.SaleOrderId.HasValue)
            {
                var saleObj = GetService<ISaleOrderService>();
                var order = await saleObj.GetByIdAsync(val.SaleOrderId);
                if (order != null)
                    res.SaleOrder = _mapper.Map<SaleOrderBasic>(order);
            }
            return res;
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            var states = new string[] { "draft", "cancel" };
            foreach (var order in self)
            {
                if (!states.Contains(order.State))
                    throw new Exception("Chỉ có thể xóa phiếu labo ở trạng thái nháp hoặc hủy bỏ.");
            }

            await DeleteAsync(self);
        }

        private void SaveOrderLines(LaboOrderDisplay val, LaboOrder order)
        {
            var existLines = order.OrderLines.ToList();
            var lineToRemoves = new List<LaboOrderLine>();
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
                    var lbLine = _mapper.Map<LaboOrderLine>(line);
                    lbLine.CompanyId = order.CompanyId;
                    lbLine.PartnerId = order.PartnerId;
                    lbLine.CustomerId = order.CustomerId;
                    foreach (var tooth in line.Teeth)
                    {
                        lbLine.LaboOrderLineToothRels.Add(new LaboOrderLineToothRel
                        {
                            ToothId = tooth.Id
                        });
                    }
                    order.OrderLines.Add(lbLine);
                }
                else
                {
                    var lbLine = order.OrderLines.SingleOrDefault(c => c.Id == line.Id);
                    if (lbLine != null)
                    {
                        _mapper.Map(line, lbLine);
                        lbLine.LaboOrderLineToothRels.Clear();
                        foreach (var tooth in line.Teeth)
                        {
                            lbLine.LaboOrderLineToothRels.Add(new LaboOrderLineToothRel
                            {
                                ToothId = tooth.Id
                            });
                        }
                    }
                }
            }
        }

        public async Task<LaboOrderPrintVM> GetPrint(Guid id)
        {
            var order = await SearchQuery(x => x.Id == id)
               .Include(x => x.Partner)
               .Include(x => x.Company)
               .Include(x => x.Company.Partner)
               .Include(x => x.OrderLines)
               .Include("OrderLines.Product")
               .FirstOrDefaultAsync();
            var res = _mapper.Map<LaboOrderPrintVM>(order);
            var partnerObj = GetService<IPartnerService>();
            res.CompanyAddress = partnerObj.GetFormatAddress(order.Company.Partner);
            res.PartnerAddress = partnerObj.GetFormatAddress(order.Partner);
            return res;
        }

        public override async Task<IEnumerable<LaboOrder>> CreateAsync(IEnumerable<LaboOrder> entities)
        {
            await _UpdateProperties(entities);
            return await base.CreateAsync(entities);
        }

        public override async Task UpdateAsync(IEnumerable<LaboOrder> entities)
        {
            await _UpdateProperties(entities);
            await base.UpdateAsync(entities);
        }

        private async Task _UpdateProperties(IEnumerable<LaboOrder> self)
        {
            var saleObj = GetService<ISaleOrderService>();
            foreach(var labo in self)
            {
                if (string.IsNullOrEmpty(labo.Name))
                {
                    var sequenceObj = GetService<IIRSequenceService>();
                    labo.Name = await sequenceObj.NextByCode("labo.order");
                }

                var sale = labo.SaleOrderId.HasValue ? await saleObj.GetByIdAsync(labo.SaleOrderId.Value) : null;
                var partnerId = sale != null ? sale.PartnerId : (Guid?)null;
                labo.CustomerId = partnerId;

                foreach (var line in labo.OrderLines)
                {
                    line.CustomerId = partnerId;
                    line.PartnerId = labo.PartnerId;
                }
            }
        }
    }
}
