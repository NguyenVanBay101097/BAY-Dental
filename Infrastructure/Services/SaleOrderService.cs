using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class SaleOrderService: BaseService<SaleOrder>, ISaleOrderService
    {
        private readonly IMapper _mapper;

        public SaleOrderService(IAsyncRepository<SaleOrder> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<SaleOrder> CreateOrderAsync(SaleOrder order)
        {
            if (string.IsNullOrEmpty(order.Name) || order.Name == "/")
            {
                var sequenceService = (IIRSequenceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IIRSequenceService));
                order.Name = await sequenceService.NextByCode("sale.order");
            }

            var saleLineService = (ISaleOrderLineService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(ISaleOrderLineService));
            saleLineService.UpdateOrderInfo(order.OrderLines, order);
            saleLineService.ComputeAmount(order.OrderLines);
            saleLineService._GetInvoiceQty(order.OrderLines);
            saleLineService._GetToInvoiceQty(order.OrderLines);
            saleLineService._ComputeInvoiceStatus(order.OrderLines);

            _AmountAll(order);

            return await CreateAsync(order);
        }

        private void _AmountAll(SaleOrder order)
        {
            var totalAmountUntaxed = 0M;
            var totalAmountTax = 0M;

            foreach (var line in order.OrderLines)
            {
                totalAmountUntaxed += line.PriceSubTotal;
                totalAmountTax += line.PriceTax;
            }
            order.AmountTax = Math.Round(totalAmountTax);
            order.AmountUntaxed = Math.Round(totalAmountUntaxed);
            order.AmountTotal = order.AmountTax + order.AmountUntaxed;
        }

        public async Task<PagedResult<SaleOrder>> GetPagedResultAsync(int pageIndex = 0, int pageSize = 20, string orderBy = "name", string orderDirection = "asc", string filter = "")
        {
            Expression<Func<SaleOrder, object>> sort = null;
            switch (orderBy)
            {
                case "name":
                    sort = x => x.Name;
                    break;
                default:
                    break;
            }

            var filterSpecification = new SaleOrderFilterSpecification(filter: filter, orderBy: sort, orderDirection: orderDirection);
            var filterPaginatedSpecification = new SaleOrderFilterSpecification(filter: filter, skip: pageIndex * pageSize, take: pageSize, isPagingEnabled: true, orderBy: sort, orderDirection: orderDirection);
            var items = await base.ListAsync(filterPaginatedSpecification);
            var totalItems = await base.CountAsync(filterSpecification);

            return new PagedResult<SaleOrder>(totalItems, pageIndex + 1, pageSize)
            {
                Items = items
            };
        }

        public async Task<PagedResult2<SaleOrderBasic>> GetPagedResultAsync(SaleOrderPaged val)
        {
            ISpecification<SaleOrder> spec = new InitialSpecification<SaleOrder>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<SaleOrder>(x => x.Name.Contains(val.Search) ||
                x.Partner.Name.Contains(val.Search) ||
                x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Phone.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await query.Select(x => new SaleOrderBasic {
                Id = x.Id,
                AmountTotal = x.AmountTotal,
                DateOrder = x.DateOrder,
                Name = x.Name,
                PartnerName = x.Partner.Name,
                State = x.State,
                UserName = x.User != null ? x.User.Name: string.Empty
            }).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<SaleOrderBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.OrderLines)
                .Include(x => x.DotKhams)
                .Include("OrderLines.Order")
                .Include("OrderLines.SaleOrderLineInvoiceRels")
                .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine")
                .Include("OrderLines.SaleOrderLineInvoiceRels.InvoiceLine.Invoice")
                .ToListAsync();

            var saleLineObj = GetService<ISaleOrderLineService>();
            var invoiceIds = new List<Guid>().AsEnumerable();
            var dotKhamIds = new List<Guid>().AsEnumerable();
            foreach (var sale in self)
            {
                foreach (var line in sale.OrderLines)
                {
                    invoiceIds = invoiceIds.Union(line.SaleOrderLineInvoiceRels.Select(x => x.InvoiceLine.Invoice.Id).Distinct().ToList());
                }

                dotKhamIds = dotKhamIds.Union(sale.DotKhams.Select(x => x.Id).ToList());
            }

            await UpdateAsync(self);

            if (invoiceIds.Any())
            {
                var invObj = GetService<IAccountInvoiceService>();
                await invObj.ActionCancel(invoiceIds);
            }

            if (dotKhamIds.Any())
            {
                var dkObj = GetService<IDotKhamService>();
                await dkObj.ActionCancel(dotKhamIds);
            }

            foreach (var sale in self)
            {
                foreach (var line in sale.OrderLines)
                {
                    line.State = "cancel";
                }

                saleLineObj._GetInvoiceQty(sale.OrderLines);
                saleLineObj._GetToInvoiceQty(sale.OrderLines);
                saleLineObj._ComputeInvoiceStatus(sale.OrderLines);

                sale.State = "cancel";
                _GetInvoiced(new List<SaleOrder>() { sale });
            }

            await UpdateAsync(self);
        }


        public async Task<SaleOrder> GetSaleOrderForDisplayAsync(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .Include(x => x.User)
                .Include(x => x.OrderLines)
                .Include("OrderLines.Product")
                .Include("OrderLines.ToothCategory")
                .Include("OrderLines.SaleOrderLineToothRels")
                .Include("OrderLines.SaleOrderLineToothRels.Tooth")
                .FirstOrDefaultAsync();
        }

        public async Task<SaleOrder> GetSaleOrderWithLines(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.OrderLines)
                .Include("OrderLines.SaleOrderLineToothRels")
                .Include("OrderLines.DotKhamSteps")
                .FirstOrDefaultAsync();
        }

        public async Task UpdateOrderAsync(SaleOrder order)
        {
            var saleLineObj = GetService<ISaleOrderLineService>();
            saleLineObj.UpdateOrderInfo(order.OrderLines, order);
            saleLineObj.ComputeAmount(order.OrderLines);
            await UpdateAsync(order);

            var linesIds = order.OrderLines.Select(x => x.Id).ToList();
            var lines = await saleLineObj.SearchQuery(x => linesIds.Contains(x.Id))
                .Include(x => x.Order)
                .Include(x => x.Product)
               .Include(x => x.SaleOrderLineInvoiceRels)
               .Include("SaleOrderLineInvoiceRels.InvoiceLine")
               .Include("SaleOrderLineInvoiceRels.InvoiceLine.Invoice")
               .ToListAsync();
           
            saleLineObj._GetInvoiceQty(lines);
            saleLineObj._GetToInvoiceQty(lines);
            saleLineObj._ComputeInvoiceStatus(lines);
            await saleLineObj.UpdateAsync(lines);

            _AmountAll(order);
            _GetInvoiced(new List<SaleOrder>() { order });

            await UpdateAsync(order);

            if (order.InvoiceStatus == "to invoice")
            {
                var self = new List<SaleOrder>() { order };
                var invoices = await ActionInvoiceCreate(self);
                var invObj = GetService<IAccountInvoiceService>();
                await invObj.ActionInvoiceOpen(invoices.Select(x => x.Id).ToList());

                foreach (var saleOrder in self)
                {
                    saleLineObj._GetInvoiceQty(saleOrder.OrderLines);
                    saleLineObj._GetToInvoiceQty(saleOrder.OrderLines);
                    saleLineObj._ComputeInvoiceStatus(saleOrder.OrderLines);
                }

                _GetInvoiced(self);
                await UpdateAsync(self);

                await _GenerateDotKhamSteps(self);
            }
        }

        public async Task<SaleOrder> GetSaleOrderByIdAsync(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.DotKhams)
                .ToListAsync();
            var states = new string[] { "draft", "cancel" };
            foreach(var order in self)
            {
                if (!states.Contains(order.State))
                {
                    throw new Exception("Bạn chỉ có thể xóa phiếu ở trạng thái nháp hoặc hủy bỏ");
                }
            }

            var dkStepObj = GetService<IDotKhamStepService>();
            var dkSteps = await dkStepObj.SearchQuery(x => ids.Contains(x.SaleOrderId.Value)).ToListAsync();
            await dkStepObj.DeleteAsync(dkSteps);

            await DeleteAsync(self);
        }


        public async Task UnlinkSaleOrderAsync(SaleOrder order)
        {
            await DeleteAsync(order);
        }

        public async Task<SaleOrderLineDisplay> DefaultLineGet(SaleOrderLineDefaultGet val)
        {
            var productObj = GetService<IProductService>();
            var product = await productObj.GetByIdAsync(val.ProductId);
            var res = new SaleOrderLineDisplay()
            {
                Name = product.Name,
                ProductUOMQty = 1,
                ProductId = product.Id,
                Product = _mapper.Map<ProductSimple>(product),
                PriceUnit = product.ListPrice,
                PriceSubTotal = product.ListPrice,
                PriceTotal = product.ListPrice,
            };
            return res;
        }

        public async Task<SaleOrderDisplay> DefaultGet()
        {
            var userManager = (UserManager<ApplicationUser>)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>));
            var user = await userManager.FindByIdAsync(UserId);
            var res = new SaleOrderDisplay();
            res.CompanyId = CompanyId;
            res.UserId = UserId;
            res.User = _mapper.Map<ApplicationUserSimple>(user);
            return res;
        }

        public async Task ActionConfirm(IEnumerable<Guid> ids)
        {
            var saleLineObj = GetService<ISaleOrderLineService>();
            var self = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.OrderLines)
                .Include("OrderLines.DotKhamSteps")
                .ToListAsync();
            foreach (var order in self)
            {
                order.State = "sale";
                foreach (var line in order.OrderLines)
                {
                    line.State = "sale";
                }
                saleLineObj._GetToInvoiceQty(order.OrderLines);
                saleLineObj._ComputeInvoiceStatus(order.OrderLines);
            }

            _GetInvoiced(self);
            await UpdateAsync(self);

            var invoices = await ActionInvoiceCreate(self);
            var invObj = GetService<IAccountInvoiceService>();
            await invObj.ActionInvoiceOpen(invoices.Select(x => x.Id).ToList());

            foreach (var order in self)
            {
                saleLineObj._GetInvoiceQty(order.OrderLines);
                saleLineObj._GetToInvoiceQty(order.OrderLines);
                saleLineObj._ComputeInvoiceStatus(order.OrderLines);
            }

            _GetInvoiced(self);
            await UpdateAsync(self);

            await _GenerateDotKhamSteps(self);
        }

        private async Task _GenerateDotKhamSteps(IEnumerable<SaleOrder> self)
        {
            var dotKhamStepService = GetService<IDotKhamStepService>();
            var productStepObj = GetService<IProductStepService>();
            foreach (var order in self)
            {
                foreach (var line in order.OrderLines)
                {
                    if (!line.ProductId.HasValue || line.Product == null)
                        continue;
                    if (line.DotKhamSteps.Any())
                        continue;

                    var steps = await productStepObj.SearchQuery(x => x.ProductId == line.ProductId).ToListAsync();
                    var list = new List<DotKhamStep>();
                    if (steps.Any())
                    {
                        foreach (var step in steps)
                        {
                            list.Add(new DotKhamStep
                            {
                                Name = step.Name,
                                ProductId = line.ProductId.Value,
                                Order = step.Order,
                                SaleOrderId = order.Id,
                                SaleLineId = line.Id,
                            });
                        }
                    }
                    else
                    {
                        list.Add(new DotKhamStep
                        {
                            Name = line.Product.Name,
                            SaleLineId = line.Id,
                            ProductId = line.ProductId.Value,
                            Order = 0,
                            SaleOrderId = order.Id,
                        });
                    }
                    await dotKhamStepService.CreateAsync(list);
                }
            }
        }

        public void _GetInvoiced(IEnumerable<SaleOrder> orders)
        {
            foreach (var order in orders)
            {
                var lineInvoiceStatus = order.OrderLines.Select(x => x.InvoiceStatus);
                if (order.State != "sale" && order.State != "done")
                    order.InvoiceStatus = "no";
                else if (lineInvoiceStatus.Any(x => x == "to invoice"))
                    order.InvoiceStatus = "to invoice";
                else if (lineInvoiceStatus.All(x => x == "invoiced"))
                    order.InvoiceStatus = "invoiced";
                else if (lineInvoiceStatus.All(x => x == "invoiced" || x == "upselling"))
                    order.InvoiceStatus = "upselling";
                else
                    order.InvoiceStatus = "no";
            }
        }

        public async Task<IEnumerable<AccountInvoice>> ActionInvoiceCreate(IEnumerable<SaleOrder> orders)
        {
            var saleLineObj = GetService<ISaleOrderLineService>();
            var invLineObj = GetService<IAccountInvoiceLineService>();
            var invObj = GetService<IAccountInvoiceService>();

            var companyId = CompanyId;
            var journalObj = GetService<IAccountJournalService>();
            var journal = journalObj.SearchQuery(x => x.Type == "sale" && x.CompanyId == companyId)
                .Include(x => x.DefaultCreditAccount).Include(x => x.DefaultDebitAccount).FirstOrDefault();
            if (journal == null)
                throw new Exception("Vui lòng tạo nhật ký bán hàng cho công ty này.");

            var invoices = new Dictionary<Guid, AccountInvoice>();
            var orderToUpdate = new HashSet<SaleOrder>();
        
            foreach (var order in orders)
            {
                var groupKey = order.Id;
                var invLines = new List<AccountInvoiceLine>();
                foreach (var line in order.OrderLines)
                {
                    if (FloatUtils.FloatIsZero((double)(line.QtyToInvoice ?? 0), precisionDigits: 5))
                        continue;
                    if (!invoices.ContainsKey(groupKey))
                    {
                        var inv = await PrepareInvoice(order, journal);
                        await invObj.CreateAsync(inv);
                        invoices.Add(groupKey, inv);
                    }

                    if (line.QtyToInvoice > 0)
                    {
                        var invLine = saleLineObj._PrepareInvoiceLine(line, line.QtyToInvoice ?? 0, journal.DefaultCreditAccount);
                        invLine.InvoiceId = invoices[groupKey].Id;
                        invLine.Invoice = invoices[groupKey];
                        invLine.SaleLines.Add(new SaleOrderLineInvoiceRel { OrderLineId = line.Id });
                        invLines.Add(invLine);
                    }
                }

                if (invLines.Any() && invoices.ContainsKey(groupKey))
                {
                    await invLineObj.CreateAsync(invLines);
                    var inv = invoices[groupKey];
                    invObj._ComputeAmount(inv);
                    await invObj.UpdateAsync(inv);
                }
            }

            if (!invoices.Any())
                throw new Exception("Không có dòng nào có thể tạo hóa đơn");

            foreach (var invoice in invoices.Values)
            {
                if (!invoice.InvoiceLines.Any())
                    throw new Exception("Không có dòng nào có thể tạo hóa đơn");
            }

            return invoices.Values.ToList();
        }

        public async Task<AccountInvoice> PrepareInvoice(SaleOrder order, AccountJournal journal)
        {
            var accountObj = GetService<IAccountAccountService>();
            var accountReceivable = await accountObj.GetAccountReceivableCurrentCompany();

            var vals = new AccountInvoice
            {
                Name = order.Name,
                Origin = order.Name,
                Type = "out_invoice",
                Reference = order.Name,
                PartnerId = order.PartnerId,
                Comment = order.Note,
                AccountId = accountReceivable.Id,
                JournalId = journal.Id,
                UserId = order.UserId,
                CompanyId = order.CompanyId,
                Company = order.Company,
            };

            return vals;
        }
    }
}
