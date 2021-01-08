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
    public class MedicineOrderService : BaseService<MedicineOrder>, IMedicineOrderService
    {
        private readonly IMapper _mapper;
        public MedicineOrderService(IAsyncRepository<MedicineOrder> repository, IHttpContextAccessor httpContextAccessor, IMapper mappner)
            : base(repository, httpContextAccessor)
        {
            _mapper = mappner;
        }


        public async Task<PagedResult2<MedicineOrderBasic>> GetPagedResultAsync(MedicineOrderPaged val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) ||
                   x.Partner.Name.Contains(val.Search));

            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State == val.State);

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.OrderDate >= val.DateFrom);

            if (val.DateTo.HasValue)
            {
                var dateOrderTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.OrderDate <= dateOrderTo);
            }


            var totalItems = await query.CountAsync();

            query = query.Include(x => x.Partner).Include(x => x.Employee).Include(x => x.ToaThuoc).ThenInclude(s => s.SaleOrder);

            query = query.OrderByDescending(x => x.DateCreated);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var paged = new PagedResult2<MedicineOrderBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<MedicineOrderBasic>>(items)
            };
            return paged;
        }

        public async Task<MedicineOrderDisplay> GetByIdDisplay(Guid id)
        {
            var medicineOrder = await SearchQuery(x => x.Id == id)
                .Include(x => x.MedicineOrderLines)
                .Include(x => x.AccountPayment)
                .Include(x => x.Company)
                .Include(x => x.Employee)
                .Include(x => x.Partner)
                .Include(x => x.ToaThuoc)
                .Include(x => x.Journal)
                .FirstOrDefaultAsync();

            var medicineOrderLineObj = GetService<IMedicineOrderLineService>();
            medicineOrder.MedicineOrderLines = await medicineOrderLineObj.SearchQuery(x => x.MedicineOrderId == medicineOrder.Id).Include(x => x.ToaThuocLine).ThenInclude(s => s.Product).ToListAsync();
            var display = _mapper.Map<MedicineOrderDisplay>(medicineOrder);

            return display;
        }

        public async Task<MedicineOrderDisplay> DefaultGet(DefaultGet val)
        {
            var toathuocObj = GetService<IToaThuocService>();
            var toathuoLinecObj = GetService<IToaThuocLineService>();
            var toathuoc = await toathuocObj.SearchQuery(x => x.Id == val.ToaThuocId).Include(x => x.Employee).Include(x => x.Partner).Include(x => x.Lines).FirstOrDefaultAsync();
            if (toathuoc == null)
                throw new Exception("Toa thuốc không tồn tại ");

            var journalObj = GetService<IAccountJournalService>();
            var journal = await journalObj.SearchQuery(x => x.Type == "cash" && x.CompanyId == CompanyId).FirstOrDefaultAsync();

            var medicineOrder = new MedicineOrderDisplay()
            {
                OrderDate = DateTime.Now,
                EmployeeId = toathuoc.EmployeeId.HasValue ? toathuoc.EmployeeId : null,
                Employee = toathuoc.EmployeeId.HasValue ? _mapper.Map<EmployeeSimple>(toathuoc.Employee) : null,
                PartnerId = toathuoc.PartnerId,
                Partner = _mapper.Map<PartnerBasic>(toathuoc.Partner),
                State = "draft",
                ToaThuocId = toathuoc.Id,
                ToaThuoc = _mapper.Map<ToaThuocDisplay>(toathuoc),
                CompanyId = CompanyId,
                Journal = _mapper.Map<AccountJournalSimple>(journal),
            };

            var medicineOrderLines = new List<MedicineOrderLineDisplay>();
            var toathuocLines = _mapper.Map<IEnumerable<ToaThuocLineDisplay>>(await toathuoLinecObj.SearchQuery(x => x.ToaThuocId == toathuoc.Id).Include(x => x.Product).ToListAsync());
            foreach (var line in toathuocLines)
            {
                medicineOrderLines.Add(new MedicineOrderLineDisplay
                {
                    Quantity = line.Quantity,
                    Price = line.Product.ListPrice.HasValue ? line.Product.ListPrice.Value : 0,
                    AmountTotal = line.Quantity * line.Product.ListPrice.Value,
                    ToaThuocLine = line,
                    ToaThuocLineId = line.Id
                });
            }
            medicineOrder.MedicineOrderLines = medicineOrderLines;
            return medicineOrder;
        }


        public async Task<MedicineOrder> CreateMedicineOrder(MedicineOrderSave val)
        {
            var medicineOrder = _mapper.Map<MedicineOrder>(val);
            SaveOrderLines(val, medicineOrder);
            await CreateAsync(medicineOrder);

            return medicineOrder;
        }



        public async Task UpdateMedicineOrder(Guid id, MedicineOrderSave val)
        {
            var medicineOrder = await SearchQuery(x => x.Id == id).Include(x => x.MedicineOrderLines).FirstOrDefaultAsync();

            medicineOrder = _mapper.Map(val, medicineOrder);

            SaveOrderLines(val, medicineOrder);

            await UpdateAsync(medicineOrder);

        }

        public async Task<MedicineOrderBasic> ActionPayment(MedicineOrderSave val)
        {
            //var medicineOrders = await SearchQuery(x => ids.Contains(x.Id))
            //    .Include(x => x.Company)
            //    .Include(x => x.MedicineOrderLines)
            //    .Include(x => x.AccountPayment)
            //    .Include(x => x.Company)
            //    .Include(x => x.Employee)
            //    .Include(x => x.Partner)
            //    .Include(x => x.ToaThuoc)
            //    .Include(x => x.Journal)
            //    .Include(x => x.Journal.DefaultDebitAccount)
            //    .Include(x => x.Journal.DefaultCreditAccount)
            //    .ToListAsync();

            var medicineOrder = _mapper.Map<MedicineOrder>(val);

            SaveOrderLines(val, medicineOrder);

            await CreateAsync(medicineOrder);

            var moveObj = GetService<IAccountMoveService>();


            if (medicineOrder.State != "draft")
                throw new Exception("Chỉ những phiếu nháp mới được vào sổ.");

            var moves = await _PrepareAccountMove(medicineOrder);
            await moveObj.ActionPost(moves);

            medicineOrder.State = "confirmed";

            ///thanh toán
            var amountTotal = medicineOrder.Amount;
            var payment = new AccountPayment()
            {
                Amount = amountTotal,
                JournalId = medicineOrder.JournalId,
                PaymentType = amountTotal > 0 ? "inbound" : "outbound",
                PartnerId = medicineOrder.PartnerId,
                PartnerType = "customer",
                CompanyId = medicineOrder.CompanyId
            };

            var paymentObj = GetService<IAccountPaymentService>();
            await paymentObj.CreateAsync(payment);
            await paymentObj.Post(new List<Guid>() { payment.Id });

            medicineOrder.AccountPaymentId = payment.Id;


            await UpdateAsync(medicineOrder);

            var basic = _mapper.Map<MedicineOrderBasic>(medicineOrder);
            return basic;

        }



        public async Task<IEnumerable<AccountMove>> _PrepareAccountMove(MedicineOrder self, bool final = false)
        {
            //param final: if True, refunds will be generated if necessary

            var medicineOrderLineObj = GetService<IMedicineOrderLineService>();
            var invoice_vals_list = new List<AccountMove>();
            // Invoice values.
            var invoice_vals = await _PrepareInvoice(self);

            //Invoice line values (keep only necessary sections)
            foreach (var line in self.MedicineOrderLines)
            {
                if (line.Quantity == 0)
                    continue;
                if (line.Quantity > 0 || (line.Quantity < 0 && final))
                {

                    var medicineline = await medicineOrderLineObj.SearchQuery(x => x.Id == line.Id).Include(x => x.ToaThuocLine).ThenInclude(s => s.Product).FirstOrDefaultAsync();
                    invoice_vals.InvoiceLines.Add(_PrepareInvoiceLine(medicineline));
                }
            }
            invoice_vals_list.Add(invoice_vals);

            var out_invoice_vals_list = new List<AccountMove>();
            var refund_invoice_vals_list = new List<AccountMove>();
            if (final)
            {
                if (invoice_vals.InvoiceLines.Sum(x => x.Quantity * x.PriceUnit) < 0)
                {
                    foreach (var l in invoice_vals.InvoiceLines)
                        l.Quantity = -l.Quantity;
                    invoice_vals.Type = "out_refund";
                    refund_invoice_vals_list.Add(invoice_vals);
                }
                else
                    out_invoice_vals_list.Add(invoice_vals);
            }
            else
                out_invoice_vals_list = invoice_vals_list;

            var moveObj = GetService<IAccountMoveService>();
            var moves = await moveObj.CreateMoves(out_invoice_vals_list, default_type: "out_invoice");
            moves = moves.Concat(await moveObj.CreateMoves(refund_invoice_vals_list, default_type: "out_refund"));

            return moves;
        }

        private async Task<AccountMove> _PrepareInvoice(MedicineOrder self)
        {
            var accountMoveObj = GetService<IAccountMoveService>();
            var journal = await accountMoveObj.GetDefaultJournalAsync(default_type: "out_invoice");
            if (journal == null)
                throw new Exception($"Please define an accounting sales journal for the company {CompanyId}.");

            var invoice_vals = new AccountMove
            {
                Ref = "",
                Type = "out_invoice",
                Narration = self.Note,
                PartnerId = self.PartnerId,
                InvoiceOrigin = self.Name,
                JournalId = journal.Id,
                Journal = journal,
                CompanyId = journal.CompanyId,
            };

            return invoice_vals;
        }

        public AccountMoveLine _PrepareInvoiceLine(MedicineOrderLine line)
        {

            var res = new AccountMoveLine
            {
                Name = line.ToaThuocLine.Product.Name,
                ProductId = line.ToaThuocLine.ProductId,
                Quantity = line.Quantity,
                PriceUnit = line.Price,
            };

            return res;
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var moveObj = GetService<IAccountMoveService>();
            var accountPaymentObj = GetService<IAccountPaymentService>();
            var medicineOrders = await SearchQuery(x => ids.Contains(x.Id))
               .Include(x => x.Company)
               .Include(x => x.MedicineOrderLines)
               .Include(x => x.AccountPayment)
               .Include(x => x.Company)
               .Include(x => x.Employee)
               .Include(x => x.Partner)
               .Include(x => x.ToaThuoc)
               .ToListAsync();

            var move_ids = new List<Guid>();
            var payment_ids = new List<Guid>();
            foreach (var order in medicineOrders)
            {
                var move_id = await moveObj.SearchQuery(x => x.InvoiceOrigin == order.Name).Select(x => x.Id).FirstOrDefaultAsync();
                if (move_id != null)
                    move_ids.Add(move_id);

                if (order.AccountPaymentId.HasValue)
                {
                    var payment_id = await accountPaymentObj.SearchQuery(x => x.Id == order.AccountPaymentId.Value).Select(x => x.Id).FirstOrDefaultAsync();
                    if (payment_id != null)
                        payment_ids.Add(payment_id);
                }

                order.State = "cancel";
            }

            if (move_ids.Any())
            {
                await moveObj.ButtonDraft(move_ids);
                await moveObj.Unlink(move_ids);
            }

            if (payment_ids.Any())
                await accountPaymentObj.ActionDraftUnlink(payment_ids);


            await UpdateAsync(medicineOrders);
        }



        private void SaveOrderLines(MedicineOrderSave val, MedicineOrder order)
        {
            var lineToRemoves = new List<MedicineOrderLine>();

            foreach (var existLine in order.MedicineOrderLines)
            {
                if (!val.MedicineOrderLines.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                order.MedicineOrderLines.Remove(line);
            }

            foreach (var line in val.MedicineOrderLines)
            {
                if (line.Id == Guid.Empty)
                {
                    var item = _mapper.Map<MedicineOrderLine>(line);
                    order.MedicineOrderLines.Add(item);
                }
                else
                {
                    var l = order.MedicineOrderLines.SingleOrDefault(c => c.Id == line.Id);
                    _mapper.Map(line, l);
                }

            }
        }

        public async override Task<MedicineOrder> CreateAsync(MedicineOrder entity)
        {
            var sequenceService = (IIRSequenceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IIRSequenceService));
            entity.Name = await sequenceService.NextByCode("medicine.order");
            if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
            {
                await InsertMedicineOrderSequence();
                entity.Name = await sequenceService.NextByCode("medicine.order");
            }

            await base.CreateAsync(entity);
            return entity;
        }

        private async Task InsertMedicineOrderSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Code = "medicine.order",
                Name = "Mã hóa đơn thuốc",
                Prefix = "HDTT{dd}{MM}{yy}-",
                Padding = 5,
            });
        }

        public async Task<MedicineOrderPrint> GetPrint(Guid id)
        {
            var medicineOrder = await SearchQuery(x => x.Id == id)
                .Include(x => x.MedicineOrderLines)
                .Include(x => x.AccountPayment)
                .Include(x => x.Company.Partner)
                .Include(x => x.Employee)
                .Include(x => x.Partner)
                .Include(x => x.ToaThuoc)
                .Include(x => x.Journal)
                .FirstOrDefaultAsync();

            var medicineOrderLineObj = GetService<IMedicineOrderLineService>();
            medicineOrder.MedicineOrderLines = await medicineOrderLineObj.SearchQuery(x => x.MedicineOrderId == medicineOrder.Id).Include(x => x.ToaThuocLine).ThenInclude(s => s.Product).ToListAsync();
            var res = _mapper.Map<MedicineOrderPrint>(medicineOrder);

            return res;
        }

        public async Task<MedicineOrderReport> GetReport(MedicineOrderFilterReport val)
        {
            var query = SearchQuery();

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.OrderDate >= val.DateFrom);

            if (val.DateTo.HasValue)
            {
                var dateOrderTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.OrderDate <= dateOrderTo);
            }

            var totalItems = await query.CountAsync();
            var amountTotal = await query.Where(x => x.State == "confirmed").SumAsync(x => x.Amount);
            //var items = await query.ToListAsync();

            var report = new MedicineOrderReport
            {
                AmountTotal = amountTotal,
                MedicineOrderCount = totalItems
            };
            return report;
        }

       



        public override ISpecification<MedicineOrder> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "base.medicine_order_comp_rule":
                    return new InitialSpecification<MedicineOrder>(x => x.CompanyId != Guid.Empty || companyIds.Contains(x.CompanyId));
                default:
                    return null;
            }
        }
    }
}
