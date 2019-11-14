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
                x.Partner.Phone.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await query.Select(x => new LaboOrderBasic
            {
                Id = x.Id,
                AmountTotal = x.AmountTotal,
                DateOrder = x.DateOrder,
                Name = x.Name,
                PartnerName = x.Partner.Name,
                State = x.State,
                CustomerName = x.Customer.Name
            }).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<LaboOrderBasic>(totalItems, val.Offset, val.Limit)
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
                .Include(x => x.DotKham)
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
            if (val.DotKhamId.HasValue)
            {
                var dotKhamObj = GetService<IDotKhamService>();
                var dotKham = await dotKhamObj.GetByIdAsync(val.DotKhamId.Value);
                labo.CustomerId = dotKham.PartnerId;
            }
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

        public async Task<IList<AccountInvoice>> ActionInvoiceCreate(IEnumerable<LaboOrder> self)
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

                    var invLineData = await invObj.PrepareInvoiceLineFromLBLine(invData, poLine, journal: invData.Journal, type: "in_invoice");
                    invLines.Add(invLineData);
                }

                invData.InvoiceLines = invLines;
                invLineObj.ComputePrice(invData.InvoiceLines);
                invObj._ComputeAmount(invData);
                await invObj.CreateAsync(invData);
            }

            return res;
        }

        private async Task<AccountInvoice> PrepareInvoice(LaboOrder order)
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

           
            var invObj = GetService<IAccountInvoiceService>();
            var invoices = await ActionInvoiceCreate(self);
            await invObj.ActionInvoiceOpen(invoices.Select(x => x.Id).ToList());

            var lbLineObj = GetService<ILaboOrderLineService>();
            lbLineObj._ComputeQtyInvoiced(self.SelectMany(x => x.OrderLines));

            await UpdateAsync(self);
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

            foreach(var order in self)
            {
                order.State = "cancel";
                foreach (var line in order.OrderLines)
                    line.State = "cancel";
            }
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

        public LaboOrderDisplay DefaultGet(LaboOrderDefaultGet val)
        {
            var res = new LaboOrderDisplay();
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

        public override async Task<LaboOrder> CreateAsync(LaboOrder labo)
        {
            if (string.IsNullOrEmpty(labo.Name))
            {
                var sequenceObj = GetService<IIRSequenceService>();
                labo.Name = await sequenceObj.NextByCode("labo.order");
            }
            return await base.CreateAsync(labo);
        }
    }
}
