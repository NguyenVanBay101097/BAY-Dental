using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyERP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SaleOrderPaymentService : BaseService<SaleOrderPayment>, ISaleOrderPaymentService
    {
        private readonly IMapper _mapper;

        public SaleOrderPaymentService(IAsyncRepository<SaleOrderPayment> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<SaleOrderPaymentBasic>> GetPagedResultAsync(SaleOrderPaymentPaged val)
        {
            var query = SearchQuery();

            if (val.SaleOrderId.HasValue)
                query = query.Where(x => x.OrderId == val.SaleOrderId.Value);


            var totalItems = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated);

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var items = await query.Include(x => x.PaymentRels).ThenInclude(x => x.Payment).ThenInclude(x => x.Journal).ToListAsync();

            var paged = new PagedResult2<SaleOrderPaymentBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<SaleOrderPaymentBasic>>(items)
            };

            return paged;
        }

        public async Task<PagedResult2<HistoryPartnerAdvanceResult>> GetPagedResultHistoryAdvanceAsync(HistoryPartnerAdvanceFilter val)
        {
            var journalObj = GetService<IAccountJournalService>();
            var paymentObj = GetService<IAccountPaymentService>();
            var journalAdvance = await journalObj.SearchQuery(x => x.CompanyId == CompanyId && x.Type == "advance" && x.Active).FirstOrDefaultAsync();

            var query = paymentObj.SearchQuery(x => x.Journal.Type == "advance" && x.State == "posted");
            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId.Value);

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.PaymentDate >= val.DateFrom);

            if (val.DateTo.HasValue)
            {
                var dateOrderTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.PaymentDate <= dateOrderTo);
            }

            var totalItems = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated);

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var items = await query.Select(x => new HistoryPartnerAdvanceResult
            {
                PaymentName = x.Name,
                PaymentDate = x.PaymentDate,
                PaymentAmount = x.Amount,
                Orders = x.SaleOrderPaymentAccountPaymentRels.Select(s => new SaleOrderSimple
                {
                    Id = s.SaleOrderPayment.OrderId,
                    Name = s.SaleOrderPayment.Order.Name
                })
            }).ToListAsync();

            var paged = new PagedResult2<HistoryPartnerAdvanceResult>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };

            return paged;
        }





        public async Task<SaleOrderPaymentDisplay> GetDisplay(Guid id)
        {
            var paymentJournalLineObj = GetService<ISaleOrderPaymentJournalLineService>();
            var paymentHistoryLineObj = GetService<ISaleOrderPaymentHistoryLineService>();

            var saleOrderPaymentDisplay = await SearchQuery(x => x.Id == id).Select(x => new SaleOrderPaymentDisplay
            {
                Date = x.Date,
                Amount = x.Amount,
                Note = x.Note,
                State = x.State
            }).FirstOrDefaultAsync();

            saleOrderPaymentDisplay.JournalLines = await paymentJournalLineObj.SearchQuery(x => x.SaleOrderPaymentId == saleOrderPaymentDisplay.Id).Select(x => new SaleOrderPaymentJournalLineDisplay
            {
                Id = x.Id,
                JournalId = x.JournalId,
                Journal = new AccountJournalSimple
                {
                    Id = x.Journal.Id,
                    Name = x.Journal.Name
                },
                Amount = x.Amount

            }).ToListAsync();

            saleOrderPaymentDisplay.Lines = _mapper.Map<IEnumerable<SaleOrderPaymentHistoryLineDisplay>>(await paymentHistoryLineObj.SearchQuery(x => x.SaleOrderPaymentId == saleOrderPaymentDisplay.Id).Include(x => x.SaleOrderLine).ToListAsync());


            return saleOrderPaymentDisplay;
        }


        public async Task<SaleOrderPayment> CreateSaleOrderPayment(SaleOrderPaymentSave val)
        {
            //Mapper
            var saleOrderPayment = _mapper.Map<SaleOrderPayment>(val);
            SaveLines(val, saleOrderPayment);
            await CreateAsync(saleOrderPayment);

            return saleOrderPayment;
        }

        private void SaveLines(SaleOrderPaymentSave val, SaleOrderPayment orderPayment)
        {
            var lineToRemoves = new List<SaleOrderPaymentHistoryLine>();

            foreach (var existLine in orderPayment.Lines)
            {
                if (!val.Lines.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                orderPayment.Lines.Remove(line);
            }

            foreach (var line in val.Lines)
            {
                if (line.Id == Guid.Empty)
                {
                    var item = _mapper.Map<SaleOrderPaymentHistoryLine>(line);
                    orderPayment.Lines.Add(item);
                }
                else
                {
                    var l = orderPayment.Lines.SingleOrDefault(c => c.Id == line.Id);
                    _mapper.Map(line, l);
                }
            }
        }


        public async Task ActionPayment(IEnumerable<Guid> ids)
        {
            var moveObj = GetService<IAccountMoveService>();
            var commissionSettlementObj = GetService<ICommissionSettlementService>();
            var saleOrderObj = GetService<ISaleOrderService>();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var paymentObj = GetService<IAccountPaymentService>();
            var partnerObj = GetService<IPartnerService>();
            /// truy vấn đủ dữ liệu của saleorder payment`
            var saleOrderPayments = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Move)
                .Include(x => x.Order)
                .Include(x => x.Lines)
                .Include(x => x.JournalLines)
                .ToListAsync();
            foreach (var saleOrderPayment in saleOrderPayments)
            {
                ///ghi sổ doang thu , công nợ lines               
                var invoice = await _CreateInvoices(saleOrderPayment, final: true);

                ///cập nhật amount hoa hồng cho bác sĩ , phụ tá , tư vấn
                var settlements = invoice.Lines.SelectMany(x => x.CommissionSettlements).ToList();
                await commissionSettlementObj.UpdateAsync(settlements);

                await moveObj.ActionPost(new List<AccountMove>() { invoice });

                //tính lại paid , residual saleorder , lines
                await _ComputeSaleOrder(saleOrderPayment.OrderId);

                ///vòng lặp phương thức thanh toán tạo move , move line
                var payments = _PreparePayments(saleOrderPayment, invoice.Id);
                await paymentObj.CreateAsync(payments);
                await paymentObj.Post(payments.Select(x => x.Id).ToList());
              
                saleOrderPayment.MoveId = invoice.Id;
                foreach (var payment in payments)
                    saleOrderPayment.PaymentRels.Add(new SaleOrderPaymentAccountPaymentRel { PaymentId = payment.Id });
                /// update state = "posted"
                saleOrderPayment.State = "posted";

                //Tich diem va cap nhat hang
                //await ComputePointAndUpdateLevel(saleOrderPayment, saleOrderPayment.Order.PartnerId, "payment");
                var loyaltyPoints = await ConvertAmountToPoint(saleOrderPayment.Amount);
                var propertyObj = GetService<IIRPropertyService>();
                var partnerPointsProp = propertyObj.get("loyalty_points", "res.partner", res_id: $"res.partner,{saleOrderPayment.Order.PartnerId}", force_company: saleOrderPayment.CompanyId);
                var partnerPoints = Convert.ToDecimal(partnerPointsProp == null ? 0 : partnerPointsProp);

                partnerPoints += loyaltyPoints;
                var pointValuesDict = new Dictionary<string, object>()
                {
                    { $"res.partner,{saleOrderPayment.Order.PartnerId}", partnerPoints }
                };

                propertyObj.set_multi("loyalty_points", "res.partner", pointValuesDict, force_company: saleOrderPayment.CompanyId);

                //lấy hạng thành viên hiện tại của partner
                var partnerLevelProp = propertyObj.get("member_level", "res.partner", res_id: $"res.partner,{saleOrderPayment.Order.PartnerId}", force_company: saleOrderPayment.CompanyId);
                var partnerLevelValue = partnerLevelProp == null ? string.Empty : partnerLevelProp.ToString();
                var partnerLevelId = !string.IsNullOrEmpty(partnerLevelValue) ? Guid.Parse(partnerLevelValue.Split(",")[1]) : (Guid?)null;

                //cập nhật hạng
                var mbObj = GetService<IMemberLevelService>();
                var partnerLevel = partnerLevelId.HasValue ? await mbObj.GetByIdAsync(partnerLevelId) : null;
                MemberLevel type = null;
                if (partnerLevel != null)
                    type = await mbObj.SearchQuery(x => x.Point <= partnerPoints && x.Point >= partnerLevel.Point && x.Id != partnerLevel.Id, orderBy: x => x.OrderByDescending(s => s.Point))
                        .FirstOrDefaultAsync();
                else
                    type = await mbObj.SearchQuery(x => x.Point <= partnerPoints, orderBy: x => x.OrderByDescending(s => s.Point))
                          .FirstOrDefaultAsync();

                if (type != null)
                {
                    var levelValuesDict = new Dictionary<string, object>()
                    {
                        { $"res.partner,{saleOrderPayment.Order.PartnerId}", type.Id }
                    };
                    propertyObj.set_multi("member_level", "res.partner", levelValuesDict, force_company: saleOrderPayment.CompanyId);
                }
            }
            //await partnerObj.UpdateMemberLevelForPartner(partnerIds);
            await UpdateAsync(saleOrderPayments);
        }

        public async Task<decimal> ConvertAmountToPoint(decimal amount)
        {
            //var prate = await GetLoyaltyPointExchangeRate();
            var prate = 10000;
            if (prate < 0)
                return 0;
            var points = amount / prate;
            var res = FloatUtils.FloatRound((double)points, precisionRounding: 1);
            return (decimal)res;
        }

        public async Task<decimal> GetLoyaltyPointExchangeRate()
        {
            var irConfigParameter = GetService<IIrConfigParameterService>();
            var value = await irConfigParameter.GetParam("loyalty.point_exchange_rate");
            if (!string.IsNullOrEmpty(value))
                return Convert.ToDecimal(value);
            return 1;
        }

        public async Task<string> _GetLevel(decimal? point)
        {
            var mbObj = GetService<IMemberLevelService>();
            var levels = await mbObj.SearchQuery().OrderByDescending(x => x.Point).ToListAsync();
            if (levels == null)
                throw new Exception("Không có danh sách hạng thành viên");
            for (int i = 0; i < levels.Count(); i++)
            {
                var pointValue = levels.ElementAt(i).Point;
                if (point >= pointValue)
                    return levels.ElementAt(i).Id.ToString();
                else
                    continue;

            }
            return null;

        }

        public async Task<AccountMove> _CreateInvoices(SaleOrderPayment self, bool final = false)
        {
            //param final: if True, refunds will be generated if necessary
            var saleLineObj = GetService<ISaleOrderLineService>();
            var paymentLineObj = GetService<ISaleOrderPaymentHistoryLineService>();
            // Invoice values.
            var invoice_vals = await _PrepareInvoice(self);
            var lines = await paymentLineObj.SearchQuery(x => x.SaleOrderPaymentId == self.Id)
                .Include(x => x.SaleOrderPayment).ThenInclude(x => x.Order)
                .Include(x => x.SaleOrderLine).ThenInclude(x => x.Employee)
                .ToListAsync();

            foreach (var line in lines)
            {
                if (line.Amount == 0)
                    continue;
                var moveline = await _PrepareInvoiceLineAsync(line);
                invoice_vals.InvoiceLines.Add(moveline);
            }

            if (!invoice_vals.InvoiceLines.Any())
                throw new Exception("There is no invoiceable line. If a product has a Delivered quantities invoicing policy, please make sure that a quantity has been delivered.");

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
                out_invoice_vals_list = new List<AccountMove>() { invoice_vals };

            var moveObj = GetService<IAccountMoveService>();
            var moves = await moveObj.CreateMoves(out_invoice_vals_list, default_type: "out_invoice");
            moves = moves.Concat(await moveObj.CreateMoves(refund_invoice_vals_list, default_type: "out_refund"));

            return invoice_vals;
        }

        private async Task<AccountMove> _PrepareInvoice(SaleOrderPayment self)
        {
            var accountMoveObj = GetService<IAccountMoveService>();
            var journal = await accountMoveObj.GetDefaultJournalAsync(default_type: "out_invoice", default_company_id: self.CompanyId);
            if (journal == null)
                throw new Exception($"Please define an accounting sales journal for the company {CompanyId}.");

            var invoice_vals = new AccountMove
            {
                Ref = "",
                Type = "out_invoice",
                Narration = self.Note,
                PartnerId = self.Order.PartnerId,
                InvoiceOrigin = self.Order.Name,
                JournalId = journal.Id,
                Journal = journal,
                CompanyId = journal.CompanyId,
                Date = self.Date
            };

            return invoice_vals;
        }

        private async Task<AccountMoveLine> _PrepareInvoiceLineAsync(SaleOrderPaymentHistoryLine self)
        {
            var res = new AccountMoveLine
            {
                Name = self.SaleOrderLine.Name,
                ProductId = self.SaleOrderLine.ProductId,
                ProductUoMId = self.SaleOrderLine.ProductUOMId,
                Quantity = 1,
                PriceUnit = self.Amount,

                //SalesmanId = self.SalesmanId
            };

            var lineObj = GetService<ISaleOrderLineService>();
            var partnerObj = GetService<IPartnerService>();
            var saleOrderLine = await lineObj.SearchQuery(x => x.Id == self.SaleOrderLineId).Include(x => x.Employee).Include(x => x.Assistant).Include(x => x.Counselor).FirstOrDefaultAsync();
            var partner = await partnerObj.SearchQuery(x => x.Id == self.SaleOrderPayment.Order.PartnerId).Include(x => x.Agent).FirstOrDefaultAsync();
            var today = DateTime.Today;
            //add hoa hồng bác sĩ
            if (saleOrderLine.EmployeeId.HasValue && saleOrderLine.Employee.CommissionId.HasValue)
            {

                res.CommissionSettlements.Add(new CommissionSettlement
                {
                    EmployeeId = saleOrderLine.EmployeeId,
                    CommissionId = saleOrderLine.Employee.CommissionId,
                    ProductId = saleOrderLine.ProductId,
                    Date = today
                });
            }

            //add hoa hồng phụ tá
            if (saleOrderLine.AssistantId.HasValue && saleOrderLine.Assistant.AssistantCommissionId.HasValue)
            {

                res.CommissionSettlements.Add(new CommissionSettlement
                {
                    EmployeeId = saleOrderLine.AssistantId,
                    CommissionId = saleOrderLine.Assistant.AssistantCommissionId,
                    ProductId = saleOrderLine.ProductId,
                    Date = today
                });
            }

            //add hoa hồng tư vấn
            if (saleOrderLine.CounselorId.HasValue && saleOrderLine.Counselor.CounselorCommissionId.HasValue)
            {
                res.CommissionSettlements.Add(new CommissionSettlement
                {
                    EmployeeId = saleOrderLine.CounselorId,
                    CommissionId = saleOrderLine.Counselor.CounselorCommissionId,
                    ProductId = saleOrderLine.ProductId,
                    Date = today
                });
            }

            //add người giới thiệu nếu có
            if (partner.AgentId.HasValue)
            {
                res.CommissionSettlements.Add(new CommissionSettlement
                {
                    PartnerId = partner.Agent.PartnerId,
                    ProductId = saleOrderLine.ProductId,
                    Date = today
                });
            }

             res.SaleLineRels.Add(new SaleOrderLineInvoice2Rel { OrderLineId = self.SaleOrderLineId });


            return res;
        }

        public IEnumerable<AccountPayment> _PreparePayments(SaleOrderPayment self, Guid moveId)
        {
            //check journal payment > 0 mới xử lý ghi sổ
            var lines = self.JournalLines.Where(x => x.Amount > 0).ToList();
            var results = new List<AccountPayment>();
            foreach (var line in lines)
            {
                var payment = new AccountPayment
                {
                    JournalId = line.JournalId,
                    PartnerId = self.Order.PartnerId,
                    Amount = line.Amount,
                    PartnerType = "customer",
                    PaymentDate = self.Date,
                    PaymentType = "inbound",
                    CompanyId = self.Order.CompanyId,
                    Communication = self.Note
                };

                payment.AccountMovePaymentRels.Add(new AccountMovePaymentRel { MoveId = moveId });
                results.Add(payment);

            }

            return results;
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var moveObj = GetService<IAccountMoveService>();
            var moveLineObj = GetService<IAccountMoveLineService>();
            var saleOrderObj = GetService<ISaleOrderService>();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var paymentObj = GetService<IAccountPaymentService>();
            var now = DateTime.Now;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var saleOrderPayments = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Move).ThenInclude(s => s.Lines)
                .Include(x => x.Order)
                .Include(x => x.PaymentRels).ThenInclude(s => s.Payment)
                .ToListAsync();


            foreach (var saleOrderPayment in saleOrderPayments)
            {
                ///check thanh toán trong mới cho hủy
                if (firstDayOfMonth < saleOrderPayment.Date && saleOrderPayment.Date > lastDayOfMonth)
                    throw new Exception("Bạn chỉ được hủy thanh toán trong tháng");

                if(saleOrderPayment.State == "cancel")
                    throw new Exception("Không thể hủy phiếu ở trạng thái hủy");

                ///xử lý tìm các payment update state = "cancel" va hóa đơn thanh toán để xóa
                var payment_ids = saleOrderPayment.PaymentRels.Select(x => x.PaymentId).ToList();
                await paymentObj.CancelAsync(payment_ids);


                /// xóa các hoa hồng của bác sĩ                 
                var settlementObj = GetService<ICommissionSettlementService>();
                await settlementObj.Unlink(saleOrderPayment.Move.Lines.Select(x => x.Id).ToList());

                /// tìm và xóa hóa đơn ghi sổ doanh thu , công nợ
                if (saleOrderPayment.Move.Lines.Any())
                    await moveLineObj.RemoveMoveReconcile(saleOrderPayment.Move.Lines.Select(x => x.Id).ToList());

                await moveObj.ButtonCancel(new List<Guid>() { saleOrderPayment.Move.Id });
                await moveObj.Unlink(new List<Guid>() { saleOrderPayment.Move.Id });


                //tính lại paid , residual saleorder , lines
                await _ComputeSaleOrder(saleOrderPayment.OrderId);
                // cập nhập điểm và hạng thành viên
                //await ComputePointAndUpdateLevel(res, res.Order.PartnerId,"cancel");

                //Trừ điểm tích lũy và cập nhật lại hạng thành viên
                //await ComputePointAndUpdateLevel(saleOrderPayment, saleOrderPayment.Order.PartnerId, "payment");
                var loyaltyPoints = await ConvertAmountToPoint(saleOrderPayment.Amount);
                var propertyObj = GetService<IIRPropertyService>();
                var partnerPointsProp = propertyObj.get("loyalty_points", "res.partner", res_id: $"res.partner,{saleOrderPayment.Order.PartnerId}", force_company: saleOrderPayment.CompanyId);
                var partnerPoints = Convert.ToDecimal(partnerPointsProp == null ? 0 : partnerPointsProp);

                partnerPoints -= loyaltyPoints;
                var pointValuesDict = new Dictionary<string, object>()
                {
                    { $"res.partner,{saleOrderPayment.Order.PartnerId}", partnerPoints }
                };

                propertyObj.set_multi("loyalty_points", "res.partner", pointValuesDict, force_company: saleOrderPayment.CompanyId);

                //lấy hạng thành viên hiện tại của partner
                var partnerLevelProp = propertyObj.get("member_level", "res.partner", res_id: $"res.partner,{saleOrderPayment.Order.PartnerId}", force_company: saleOrderPayment.CompanyId);
                var partnerLevelValue = partnerLevelProp == null ? string.Empty : partnerLevelProp.ToString();
                var partnerLevelId = !string.IsNullOrEmpty(partnerLevelValue) ? Guid.Parse(partnerLevelValue.Split(",")[1]) : (Guid?)null;

                //cập nhật hạng
                var mbObj = GetService<IMemberLevelService>();
                var partnerLevel = partnerLevelId.HasValue ? await mbObj.GetByIdAsync(partnerLevelId) : null;
                MemberLevel type = null;
                if (partnerLevel != null)
                    type = await mbObj.SearchQuery(x => x.Point <= partnerPoints && x.Id != partnerLevel.Id, orderBy: x => x.OrderByDescending(s => s.Point))
                        .FirstOrDefaultAsync();
                else
                    type = await mbObj.SearchQuery(x => x.Point <= partnerPoints, orderBy: x => x.OrderByDescending(s => s.Point))
                          .FirstOrDefaultAsync();

                if (type != null)
                {
                    var levelValuesDict = new Dictionary<string, object>()
                    {
                        { $"res.partner,{saleOrderPayment.Order.PartnerId}", type.Id }
                    };
                    propertyObj.set_multi("member_level", "res.partner", levelValuesDict, force_company: saleOrderPayment.CompanyId);
                }

                saleOrderPayment.State = "cancel";
            }

            await UpdateAsync(saleOrderPayments);


        }

        public async Task _ComputeSaleOrder(Guid saleOrderId)
        {
            var orderObj = GetService<ISaleOrderService>();
            var orderlineObj = GetService<ISaleOrderLineService>();
            var linePaymentRelObj = GetService<ISaleOrderLinePaymentRelService>();
            var order = await orderObj.SearchQuery(x => x.Id == saleOrderId)
                 .Include(x => x.OrderLines).Include("OrderLines.SaleOrderLineInvoiceRels")
                 .Include("OrderLines.SaleOrderLineInvoice2Rels.InvoiceLine")
                 .Include("OrderLines.SaleOrderLineInvoice2Rels.InvoiceLine.Move").FirstOrDefaultAsync();

            orderlineObj._GetInvoiceQty(order.OrderLines);
            orderlineObj._GetToInvoiceQty(order.OrderLines);
            orderlineObj._GetInvoiceAmount(order.OrderLines);
            orderlineObj._GetToInvoiceAmount(order.OrderLines);
            orderlineObj._ComputeInvoiceStatus(order.OrderLines);
            orderlineObj._ComputeLinePaymentRels(order.OrderLines);

            orderObj._GetInvoiced(new List<SaleOrder>() { order });
            orderObj._ComputeResidual(new List<SaleOrder>() { order });
            orderObj._AmountAll(order);
            await orderObj.UpdateAsync(order);
        }



        public override ISpecification<SaleOrderPayment> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "sale.sale_order_payment_comp_rule":
                    return new InitialSpecification<SaleOrderPayment>(x => companyIds.Contains(x.CompanyId));
                default:
                    return null;
            }
        }

        public async Task<SaleOrderPaymentPrintVM> GetPrint(Guid id)
        {
            var payment = await (SearchQuery(x => x.Id == id).Include(x => x.Company.Partner)
                .Include(x => x.Order.Partner)
                .Include(x => x.CreatedBy)
                .Include(x => x.JournalLines).ThenInclude(x => x.Journal)
                ).FirstOrDefaultAsync();

            var result = _mapper.Map<SaleOrderPaymentPrintVM>(payment);
            if (result == null)
                return null;

            result.User = _mapper.Map<ApplicationUserSimple>(payment.CreatedBy);
            return result;

        }
    }
}
