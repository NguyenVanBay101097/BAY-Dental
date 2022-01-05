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
            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);

            var totalItems = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated);

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var items = await query.Include(x => x.PaymentRels).ThenInclude(x => x.Payment).ThenInclude(x => x.Journal).ThenInclude(x => x.BankAccount).ThenInclude(x => x.Bank)
                .Include(x => x.Lines).ThenInclude(x => x.SaleOrderLine).ToListAsync();

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

        public async Task<IEnumerable<SaleOrderPayment>> GetPaymentsByOrderId(Guid orderId)
        {
            var orderPayments = await SearchQuery(x => x.OrderId == orderId && x.Lines.Any(s => s.Amount > 0))
                .Include(x => x.PaymentRels).ThenInclude(s => s.Payment)
                .Include(x => x.Lines).ThenInclude(x => x.SaleOrderLine)
                .Include(x => x.JournalLines).ThenInclude(x => x.Journal)
                .ToListAsync();

            return orderPayments;
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

            if (lineToRemoves.Any())
            {
                foreach (var line in lineToRemoves)
                {
                    orderPayment.Lines.Remove(line);
                }
            }


            foreach (var line in val.Lines.Where(x => x.Amount > 0))
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
            var cardObj = GetService<ICardCardService>();
            var productObj = GetService<IProductService>();
            var commSetObj = GetService<ICommissionSettlementService>();
            var commObj = GetService<ICommissionService>();
            var agentObj = GetService<IAgentService>();

            /// truy vấn đủ dữ liệu của saleorder payment`
            var saleOrderPayments = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Move)
                .Include(x => x.Order)
                .Include(x => x.Lines)
                .Include(x => x.JournalLines).ThenInclude(s => s.Journal)
                .Include(x => x.JournalLines).ThenInclude(s => s.Insurance)
                .ToListAsync();
            foreach (var saleOrderPayment in saleOrderPayments)
            {
                ///ghi sổ doang thu , công nợ lines               
                var invoice = await _CreateInvoices(saleOrderPayment, final: true);

                //ghi nhận hoa hồng trên doanh thu tạo ra
                var commissionSettlements = new List<CommissionSettlement>();

                var saleLineIds = saleOrderPayment.Lines.Select(x => x.SaleOrderLineId).ToList();
                var saleLines = await saleLineObj.SearchQuery(x => saleLineIds.Contains(x.Id))
                    .Include(x => x.Employee)
                    .Include(x => x.Assistant)
                    .Include(x => x.Counselor)
                    .Include(x => x.Agent)
                    .ToListAsync();

                foreach (var line in saleOrderPayment.Lines)
                {
                    //tính hoa hồng
                    var saleOrderLine = saleLines.FirstOrDefault(x => x.Id == line.SaleOrderLineId);

                    //Tổng thanh toán 
                    var totalPaid = (saleOrderLine.AmountPaid ?? 0) + line.Amount;

                    //Tổng giá vốn
                    var productStdPrice = (decimal)(await productObj.GetHistoryPrice(saleOrderLine.ProductId.Value, saleOrderLine.CompanyId.Value, date: saleOrderLine.Date));
                    var totalStandPrice = productStdPrice * saleOrderLine.ProductUOMQty;

                    //Tổng tiền đã tính hoa hồng trước đó
                    var totalBaseAmount = await commSetObj.SearchQuery(x => x.SaleOrderLineId == line.SaleOrderLineId).SumAsync(x => x.BaseAmount ?? 0);

                    //Tổng tiền lợi nhuận cho lần thanh toán này
                    var totalProfitAmount = totalPaid - totalStandPrice - totalBaseAmount;

                    //Số tiền lợi nhuận sẽ tính hoa hồng
                    var baseAmountCurrent = totalProfitAmount > 0 ? totalProfitAmount : 0;

                    //add hoa hồng bác sĩ
                    if (saleOrderLine.EmployeeId.HasValue && saleOrderLine.Employee.CommissionId.HasValue)
                    {
                        //var commPercent = await commObj.getCommissionPercent(saleOrderLine.ProductId, saleOrderLine.Employee.CommissionId);
                        var commPercent = await commObj.getCommissionPercent(saleOrderLine.ProductId, saleOrderLine.Employee.CommissionId);
                        commissionSettlements.Add(new CommissionSettlement
                        {
                            PartnerId = saleOrderLine.Employee.PartnerId,
                            EmployeeId = saleOrderLine.EmployeeId,
                            CommissionId = saleOrderLine.Employee.CommissionId,
                            ProductId = saleOrderLine.ProductId,
                            SaleOrderLineId = saleOrderLine.Id,
                            HistoryLineId = line.Id,
                            BaseAmount = baseAmountCurrent,
                            Amount = baseAmountCurrent * commPercent / 100,
                            Percentage = commPercent,
                            TotalAmount = line.Amount,
                            CompanyId = saleOrderPayment.CompanyId
                        });
                    }

                    //add hoa hồng phụ tá
                    if (saleOrderLine.AssistantId.HasValue && saleOrderLine.Assistant.AssistantCommissionId.HasValue)
                    {
                        var commPercent = await commObj.getCommissionPercent(saleOrderLine.ProductId, saleOrderLine.Assistant.AssistantCommissionId);
                        commissionSettlements.Add(new CommissionSettlement
                        {
                            PartnerId = saleOrderLine.Assistant.PartnerId,
                            EmployeeId = saleOrderLine.AssistantId,
                            CommissionId = saleOrderLine.Assistant.AssistantCommissionId,
                            ProductId = saleOrderLine.ProductId,
                            SaleOrderLineId = saleOrderLine.Id,
                            HistoryLineId = line.Id,
                            BaseAmount = baseAmountCurrent,
                            Amount = baseAmountCurrent * commPercent / 100,
                            Percentage = commPercent,
                            TotalAmount = line.Amount,
                            CompanyId = saleOrderPayment.CompanyId
                        });
                    }

                    //add hoa hồng tư vấn
                    if (saleOrderLine.CounselorId.HasValue && saleOrderLine.Counselor.CounselorCommissionId.HasValue)
                    {
                        var commPercent = await commObj.getCommissionPercent(saleOrderLine.ProductId, saleOrderLine.Counselor.CounselorCommissionId);
                        commissionSettlements.Add(new CommissionSettlement
                        {
                            PartnerId = saleOrderLine.Counselor.PartnerId,
                            EmployeeId = saleOrderLine.CounselorId,
                            CommissionId = saleOrderLine.Counselor.CounselorCommissionId,
                            ProductId = saleOrderLine.ProductId,
                            SaleOrderLineId = saleOrderLine.Id,
                            HistoryLineId = line.Id,
                            BaseAmount = baseAmountCurrent,
                            Amount = baseAmountCurrent * commPercent / 100,
                            Percentage = commPercent,
                            TotalAmount = line.Amount,
                            CompanyId = saleOrderPayment.CompanyId
                        });
                    }

                    if (saleOrderLine.Agent != null && saleOrderLine.Agent.CommissionId.HasValue)
                    {
                        var commPercent = await commObj.getCommissionPercent(saleOrderLine.ProductId, saleOrderLine.Agent.CommissionId.Value);
                        commissionSettlements.Add(new CommissionSettlement
                        {
                            PartnerId = saleOrderLine.Agent.PartnerId,
                            AgentId = saleOrderLine.Agent.Id,
                            CommissionId = saleOrderLine.Agent.CommissionId,
                            ProductId = saleOrderLine.ProductId,
                            SaleOrderLineId = saleOrderLine.Id,
                            HistoryLineId = line.Id,
                            BaseAmount = baseAmountCurrent,
                            Amount = baseAmountCurrent * commPercent / 100,
                            Percentage = commPercent,
                            TotalAmount = line.Amount,
                            CompanyId = saleOrderPayment.CompanyId
                        });
                    }
                }

                await commissionSettlementObj.CreateAsync(commissionSettlements);

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
                var loyaltyPoints = ConvertAmountToPoint(saleOrderPayment.Amount);
                var card = await cardObj.SearchQuery(x => x.PartnerId == saleOrderPayment.Order.PartnerId && x.State == "in_use").FirstOrDefaultAsync();
                if (card != null)
                {
                    card.TotalPoint += loyaltyPoints;
                    var typeId = await UpGradeCardCard((card.TotalPoint ?? 0));
                    card.TypeId = typeId == Guid.Empty ? card.TypeId : typeId;
                    await cardObj.UpdateAsync(card);
                }

                //Create Log SaleOrderPayment
                var insuranceId = payments.Any(s => s.InsuranceId.HasValue) ? payments.Select(s => s.InsuranceId).FirstOrDefault() : null;
                await GenerateLogSaleOrderPayment(saleOrderPayment, insuranceId);
            }

            //await partnerObj.UpdateMemberLevelForPartner(partnerIds);
            await UpdateAsync(saleOrderPayments);
        }

        public async Task GenerateLogSaleOrderPayment(SaleOrderPayment payment, Guid? insuranceId = null)
        {
            var insuranceObj = GetService<IResInsuranceService>();
            var mailMessageObj = GetService<IMailMessageService>();
            var content = "";
            var insurance = await insuranceObj.GetByIdAsync(insuranceId);
            if (payment.State == "posted")
                content = "Thanh toán phiếu điều trị";
            if (payment.State == "cancel")
                content = "Hủy thanh toán phiếu điều trị";
            if (payment.State == "posted" && insurance != null)
                content = $"Công ty <b>{insurance.Name}</b> bảo lãnh phiếu điều trị";
            if (payment.State == "cancel" && insurance != null)
                content = $"Hủy bảo lãnh phiếu điều trị";


            var bodyContent = string.Format("{0} {1} số tiền {2} đồng", content, payment.Order.Name, string.Format("{0:#,##0}", payment.Amount));
            await mailMessageObj.CreateActionLog(body: bodyContent, threadId: payment.Order.PartnerId, threadModel: "res.partner", subtype: "subtype_sale_order_payment");
        }

        public decimal ConvertAmountToPoint(decimal amount)
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

        public async Task<Guid> UpGradeCardCard(decimal? point)
        {
            var cardTypeObj = GetService<ICardTypeService>();
            var cardTypes = await cardTypeObj.SearchQuery().OrderByDescending(x => x.BasicPoint).ToListAsync();
            if (cardTypes == null)
                throw new Exception("Không có danh sách loại thẻ thành viên");
            for (int i = 0; i < cardTypes.Count(); i++)
            {
                var pointValue = cardTypes.ElementAt(i).BasicPoint;
                if (point >= pointValue)
                    return cardTypes.ElementAt(i).Id;
                else
                    continue;

            }
            return Guid.Empty;
        }

        public async Task<AccountMove> _CreateInvoices(SaleOrderPayment self, bool final = false)
        {
            //param final: if True, refunds will be generated if necessary
            var saleLineObj = GetService<ISaleOrderLineService>();
            var paymentLineObj = GetService<ISaleOrderPaymentHistoryLineService>();
            var accountObj = GetService<IAccountAccountService>();
            // Invoice values.
            var invoice_vals = await _PrepareInvoice(self);
            var lines = await paymentLineObj.SearchQuery(x => x.SaleOrderPaymentId == self.Id)
                .Include(x => x.SaleOrderPayment).ThenInclude(x => x.Order)
                .Include(x => x.SaleOrderLine).ThenInclude(x => x.Employee)
                .Include(x => x.SaleOrderLine).ThenInclude(s => s.OrderPartner).ThenInclude(c => c.Agent)
                .ToListAsync();

            foreach (var line in lines)
            {
                if (line.Amount == 0)
                    continue;
                var moveline = _PrepareInvoiceLineAsync(line);
                invoice_vals.InvoiceLines.Add(moveline);
            }

            if (self.JournalLines.Count == 1 && self.JournalLines.ElementAt(0).Journal.Type == "insurance")
            {
                foreach (var line in invoice_vals.InvoiceLines)
                {
                    line.InsuranceId = self.JournalLines.ElementAt(0).InsuranceId;
                }
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

        private AccountMoveLine _PrepareInvoiceLineAsync(SaleOrderPaymentHistoryLine self)
        {
            var res = new AccountMoveLine
            {
                Name = self.SaleOrderLine.Name,
                ProductId = self.SaleOrderLine.ProductId,
                ProductUoMId = self.SaleOrderLine.ProductUOMId,
                Quantity = 1,
                PriceUnit = self.Amount,
                EmployeeId = self.SaleOrderLine.EmployeeId,
                AssistantId = self.SaleOrderLine.AssistantId
            };

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
                    Communication = self.Note,
                    InsuranceId = line.Journal.Type == "insurance" ? line.InsuranceId : null
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
            var cardObj = GetService<ICardCardService>();
            var now = DateTime.Now;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var saleOrderPayments = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Move).ThenInclude(s => s.Lines)
                .Include(x => x.Order)
                .Include(x => x.Lines)
                .Include(x => x.PaymentRels).ThenInclude(s => s.Payment)
                .ToListAsync();


            foreach (var saleOrderPayment in saleOrderPayments)
            {
                ///check thanh toán trong mới cho hủy
                if (firstDayOfMonth < saleOrderPayment.Date && saleOrderPayment.Date > lastDayOfMonth)
                    throw new Exception("Bạn chỉ được hủy thanh toán trong tháng");

                if (saleOrderPayment.State == "cancel")
                    throw new Exception("Không thể hủy phiếu ở trạng thái hủy");

                ///xử lý tìm các payment update state = "cancel" va hóa đơn thanh toán để xóa
                var payment_ids = saleOrderPayment.PaymentRels.Select(x => x.PaymentId).ToList();
                await paymentObj.CancelAsync(payment_ids);


                /// xóa các hoa hồng                 
                var linesIds = saleOrderPayment.Lines.Select(x => x.Id).ToList();
                var settlementObj = GetService<ICommissionSettlementService>();
                var commissionSettlements = await settlementObj.SearchQuery(x => x.HistoryLineId.HasValue && linesIds.Contains(x.HistoryLineId.Value)).ToListAsync();
                await settlementObj.DeleteAsync(commissionSettlements);

                /// tìm và xóa hóa đơn ghi sổ doanh thu , công nợ
                if (saleOrderPayment.Move.Lines.Any())
                    await moveLineObj.RemoveMoveReconcile(saleOrderPayment.Move.Lines.Select(x => x.Id).ToList());

                await moveObj.ButtonCancel(new List<Guid>() { saleOrderPayment.Move.Id });
                await moveObj.Unlink(new List<Guid>() { saleOrderPayment.Move.Id });


                //Tính lại số tiền đã thanh toán, còn lại, số tiền thanh toán bảo hiểm của SaleOrderLine
                var saleLineIds = saleOrderPayment.Lines.Select(x => x.SaleOrderLineId).ToList();
                var saleLines = await saleLineObj.SearchQuery(x => saleLineIds.Contains(x.Id)).ToListAsync();
                saleLineObj._GetInvoiceAmount(saleLines);
                await saleLineObj.UpdateAsync(saleLines);

                //Tổng tiền thanh toán và còn lại của phiếu điều trị
                var saleOrderIds = saleLines.Select(x => x.OrderId).Distinct().ToList();
                var saleOrders = await saleOrderObj.SearchQuery(x => saleOrderIds.Contains(x.Id)).ToListAsync();
                saleOrderObj._ComputeResidual(saleOrders);
                await saleOrderObj.UpdateAsync(saleOrders);

                // cập nhập điểm và hạng thành viên
                //await ComputePointAndUpdateLevel(res, res.Order.PartnerId,"cancel");

                //Trừ điểm tích lũy và cập nhật lại hạng thành viên
                //await ComputePointAndUpdateLevel(saleOrderPayment, saleOrderPayment.Order.PartnerId, "payment");
                var loyaltyPoints = ConvertAmountToPoint(saleOrderPayment.Amount);
                var card = await cardObj.SearchQuery(x => x.PartnerId == saleOrderPayment.Order.PartnerId && x.State == "in_use").FirstOrDefaultAsync();
                if (card != null)
                {
                    card.TotalPoint -= loyaltyPoints;
                    var typeId = await UpGradeCardCard((card.TotalPoint ?? 0));
                    card.TypeId = typeId == Guid.Empty ? card.TypeId : typeId;
                    await cardObj.UpdateAsync(card);
                }

                //remove insurance payment if exist
                var insurancePaymentObj = GetService<IResInsurancePaymentService>();
                var insurancePaymentIds = await insurancePaymentObj.SearchQuery(x => x.SaleOrderPaymentId == saleOrderPayment.Id).Select(x => x.Id).ToListAsync();
                if (insurancePaymentIds.Any())
                    await insurancePaymentObj.Unlink(insurancePaymentIds);

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
            //var userObj = GetService<IUserService>();
            //var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "sale.sale_order_payment_comp_rule":
                    return new InitialSpecification<SaleOrderPayment>(x => x.CompanyId == CompanyId);
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
                .Include(x => x.Lines).ThenInclude(x => x.SaleOrderLine)
                ).FirstOrDefaultAsync();

            var result = _mapper.Map<SaleOrderPaymentPrintVM>(payment);
            if (result == null)
                return null;

            result.User = _mapper.Map<ApplicationUserSimple>(payment.CreatedBy);
            return result;

        }
    }
}
