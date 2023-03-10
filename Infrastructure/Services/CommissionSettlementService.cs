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
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class CommissionSettlementService : BaseService<CommissionSettlement>, ICommissionSettlementService
    {
        private readonly IMapper _mapper;
        public CommissionSettlementService(IAsyncRepository<CommissionSettlement> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        //public async Task CreateSettlements(AccountPayment val)
        //{
        //    var settlements = new List<CommissionSettlement>();
        //    var rels = val.SaleOrderLinePaymentRels;
        //    foreach (var rel in rels)
        //        settlements.Add(await _GetSettlement(rel));

        //    ComputeAmount(settlements);

        //    await CreateAsync(settlements);
        //}

        public async Task<CommissionSettlement> _GetSettlement(SaleOrderLinePaymentRel val)
        {
            var linePaymentRelObj = GetService<ISaleOrderLinePaymentRelService>();
            var linePaymentRel = await linePaymentRelObj.SearchQuery(x => x.Id == val.Id).Include(x => x.Payment).Include(x => x.SaleOrderLine).Include("SaleOrderLine.PartnerCommissions").FirstOrDefaultAsync();

            var employeeObj = GetService<IEmployeeService>();
            var employee = await employeeObj.SearchQuery(x => x.UserId == linePaymentRel.SaleOrderLine.SalesmanId)
                .Include(x => x.Commission).FirstOrDefaultAsync();
            if (employee == null || employee.Commission == null)
                return null;


            var userObj = GetService<IUserService>();
            var user = await userObj.GetByIdAsync(linePaymentRel.SaleOrderLine.SalesmanId);
            if (user == null)
                return null;

            var partnerCommissionObj = GetService<ISaleOrderLinePartnerCommissionService>();
            var partnerCommission = await partnerCommissionObj.SearchQuery(x => x.SaleOrderLineId == linePaymentRel.SaleOrderLineId && x.CommissionId == employee.CommissionId).FirstOrDefaultAsync();
            if (partnerCommission == null)
                return null;

            var res = new CommissionSettlement
            {

                Employee = employee,
                EmployeeId = employee.Id,
                BaseAmount = linePaymentRel.AmountPrepaid,
            };

            return res;

        }

        public override async Task UpdateAsync(IEnumerable<CommissionSettlement> entities)
        {
            //await ComputeAmount(entities);
            await base.UpdateAsync(entities);
        }

        //public async Task ComputeAmount(IEnumerable<CommissionSettlement> val)
        //{
        //    var commisstionProductRuleObj = GetService<ICommissionProductRuleService>();
        //    foreach (var settlement in val)
        //    {
        //        //tính lợi nhuận
        //        //tổng thanh toán dịch vụ: sum priceSub in all moveline of this orderLineId
        //        var moveLineObj = GetService<IAccountMoveLineService>();
        //        var ams = await moveLineObj.SearchQuery(x => x.Id == settlement.MoveLineId).Include(x => x.SaleLineRels).ToListAsync();
        //        var amIds = ams.sel

        //        var percent = await commisstionProductRuleObj.SearchQuery(x => x.CommissionId == settlement.CommissionId && x.ProductId == settlement.ProductId).Select(x => x.Percent).FirstOrDefaultAsync();
        //        if (percent <= 0)
        //            continue;

        //        var total = settlement.MoveLine.PriceSubtotal;
        //        settlement.TotalAmount = total;
        //        settlement.BaseAmount = total;
        //        settlement.Percentage = percent ?? 0;
        //        settlement.Amount = ((total * (percent ?? 0)) / 100);
        //    }
        //}


        //public override Task<IEnumerable<CommissionSettlement>> CreateAsync(IEnumerable<CommissionSettlement> entities)
        //{
        //    // tạo (AccountMove -> AccountMoveLine -> add Settlement), lấy ra list sett
        //    ComputeAmount(entities);
        //    return base.CreateAsync(entities);
        //}

        //public IEnumerable<CommissionSettlement> ComputeAmount(IEnumerable<CommissionSettlement> self)
        //{
        //    foreach (var item in self)
        //    {
        //        //Lấy ra số tiền thanh toán dựa trên AccountMoveLine

        //        //Lấy ra tổng tiền vốn = số lượng * giá vốn SaleOrderLine

        //        //Tính tổng thanh toán của các lần thanh toán trước dựa vào SaleOrderLine có list AccountMoveLines
        //        //Tính tổng lợi nhuận từ các lần thanh toán trước

        //        //Tính ra số tiền lợi nhuận = số tiền thanh toán + tổng thanh toán các lần trước - tổng tiền vốn - tổng lợi nhuận của các lần thanh toán trước


        //        //item.Amount = ???;
        //        //var amount = item.BaseAmount * amountPayment / item.TotalAmount;
        //        //item.Amount = Math.Abs((decimal)((amount * item.Percentage.Value) / 100));
        //    }
        //    return self;

        //}

        public async Task Unlink(IEnumerable<Guid> moveLineIds)
        {
            var res = await SearchQuery(x => moveLineIds.Contains(x.MoveLineId.Value)).ToListAsync();
            if (res == null)
                throw new Exception("Null CommissionSettlement");

            await DeleteAsync(res);
        }


        public override ISpecification<CommissionSettlement> RuleDomainGet(IRRule rule)
        {
            //var userObj = GetService<IUserService>();
            //var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "sale.commission_settlement_comp_rule":
                    return new InitialSpecification<CommissionSettlement>(x => !x.CompanyId.HasValue || x.CompanyId == CompanyId);
                default:
                    return null;
            }
        }

        public async Task<IEnumerable<CommissionSettlementReportOutput>> GetReport(CommissionSettlementFilterReport val)
        {
            var query = SearchQuery(x => x.EmployeeId.HasValue);

            if (val.DateFrom.HasValue)
            {
                val.DateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.Date >= val.DateFrom);
            }

            if (val.DateTo.HasValue)
            {
                val.DateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= val.DateTo);
            }


            if (val.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == val.EmployeeId);

            var result = await query
                .GroupBy(x => new
                {
                    EmployeeId = x.EmployeeId,
                    EmployeeName = x.Employee.Name
                })
                .Select(x => new CommissionSettlementReportOutput
                {
                    EmployeeId = x.Key.EmployeeId,
                    EmployeeName = x.Key.EmployeeName,
                    BaseAmount = x.Sum(s => s.BaseAmount),
                    Percentage = x.Average(s => s.Percentage),
                    Amount = x.Sum(s => s.Amount),
                    CompanyId = val.CompanyId,
                    DateFrom = val.DateFrom,
                    DateTo = val.DateTo
                }).ToListAsync();

            return result;
        }

        public async Task<PagedResult2<CommissionSettlementReportDetailOutput>> GetReportDetail(CommissionSettlementFilterReport val)
        {
            var query = GetQueryableReportPaged(val);

            var totalItems = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated);
            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var items = await query.Select(x => new CommissionSettlementReportDetailOutput
            {
                Amount = x.Amount,
                BaseAmount = x.BaseAmount,
                Date = x.Date,
                Percentage = x.Percentage,
                ProductName = x.Product.Name,
                PartnerName = x.SaleOrderLine.OrderPartner.Name,
                InvoiceOrigin = x.SaleOrderLine.Order.Name,
                CommissionType = x.Commission.Type,
                Classify = x.Agent.Classify,
                Name = x.Partner.Name,
                TotalAmount = x.TotalAmount,
                SaleOrderId = x.SaleOrderLine.OrderId,
                UOMName = x.SaleOrderLine.ProductUOM != null ? x.SaleOrderLine.ProductUOM.Name : ""
            }).ToListAsync();

            return new PagedResult2<CommissionSettlementReportDetailOutput>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<decimal> GetSumReport(CommissionSettlementFilterReport val)
        {
            var query = GetQueryableReportPaged(val);

            return await query.GroupBy(x => new { x.EmployeeId.Value, Amount = x.Amount.Value }).SumAsync(x => x.Key.Amount);
        }

        public async Task<SumAmountTotalReponse> GetSumAmountTotalReport(CommissionSettlementFilterReport val)
        {
            var query = GetQueryableReportPaged(val);

            var res = new SumAmountTotalReponse();
            res.TotalAmount = await query.SumAsync(x => x.TotalAmount ?? 0);
            res.TotalBaseAmount = await query.SumAsync(x => x.BaseAmount ?? 0);
            res.TotalComissionAmount = await query.SumAsync(x => x.Amount ?? 0);

            return res;
        }

        public IQueryable<CommissionSettlement> GetQueryableReportPaged(CommissionSettlementFilterReport val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.SaleOrderLine.Order.Name.Contains(val.Search)
                || x.Product.Name.Contains(val.Search)
                || x.Product.NameNoSign.Contains(val.Search)
                || x.SaleOrderLine.OrderPartner.Name.Contains(val.Search)
                || x.SaleOrderLine.OrderPartner.NameNoSign.Contains(val.Search)
                );


            if (val.DateFrom.HasValue)
            {
                val.DateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.Date >= val.DateFrom);
            }

            if (val.DateTo.HasValue)
            {
                val.DateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= val.DateTo);
            }

            if (val.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == val.EmployeeId);

            if (val.AgentId.HasValue)
                query = query.Where(x => x.AgentId == val.AgentId);

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);

            if (!string.IsNullOrEmpty(val.CommissionType))
                query = query.Where(x => x.Commission.Type == val.CommissionType);

            if (val.GroupBy == "employee")
                query = query.Where(x => x.EmployeeId.HasValue);
            else if (val.GroupBy == "agent")
            {
                query = query.Where(x => x.AgentId.HasValue);

                if (!string.IsNullOrEmpty(val.Classify))
                    query = query.Where(x => x.Agent.Classify == val.Classify);
            }

            if (!string.IsNullOrEmpty(val.CommissionDisplay))
            {
                if (val.CommissionDisplay == "greater_than_zero")
                    query = query.Where(x => x.Percentage > 0);
                else if(val.CommissionDisplay == "equals_zero")
                    query = query.Where(x => x.Percentage == 0);
            }

            return query;
        }

        public string CommissionType(string commType)
        {
            switch (commType)
            {
                case "doctor":
                    return "Bác sĩ";
                case "assistant":
                    return "Phụ tá";
                case "counselor":
                    return "Tư vấn";
                default:
                    return "";
            };
        }

        public string Classify(string val)
        {
            switch (val)
            {
                case "partner":
                    return "Đối tác";
                case "employee":
                    return "Nhân viên";
                case "customer":
                    return "Khách hàng";
                default:
                    return "";
            };
        }

        public async Task<IEnumerable<CommissionSettlementReportRes>> GetReportPaged(CommissionSettlementFilterReport val)
        {
            var query = GetQueryableReportPaged(val);

            //var items = query.GroupBy(x => new { EmployeeId = x.EmployeeId.Value, EmployeeName = x.Employee.Name, Date = x.Date.Value.Date, CommissionType = x.Commission.Type });
            var items = await query.Include(x => x.Employee).Include(x => x.Commission).ToListAsync();


            var res = items.GroupBy(x => new
            {
                EmployeeId = x.EmployeeId.Value,
                EmployeeName = x.Employee.Name,
                //Date = x.Date.Value.Date,
                CommissionType = x.Commission.Type
            }).Select(x => new CommissionSettlementReportRes()
            {
                Amount = x.Sum(z => z.Amount),
                CommissionType = x.Key.CommissionType,
                EmployeeId = x.Key.EmployeeId,
                EmployeeName = x.Key.EmployeeName
            }).ToList();

            return res;
        }

        public async Task<IEnumerable<CommissionSettlementOverview>> GetCommissionSettlements(DateTime? dateFrom, DateTime? dateTo, string classify, string groupBy)
        {

            var query = SearchQuery();

            if (dateFrom.HasValue)
                query = query.Where(x => x.Date >= dateFrom.Value.AbsoluteBeginOfDate());

            if (dateTo.HasValue)
                query = query.Where(x => x.Date <= dateTo.Value.AbsoluteEndOfDate());

            var res = new List<CommissionSettlementOverview>();

            if (groupBy == "employee")
            {
                res = await query.Where(x => x.EmployeeId.HasValue).GroupBy(x => new
                {
                    Id = x.EmployeeId.Value,
                    Name = x.Employee.Name,
                }).Select(x => new CommissionSettlementOverview()
                {
                    Id = x.Key.Id,
                    Name = x.Key.Name,
                    BaseAmount = x.Sum(s => s.BaseAmount ?? 0),
                    Amount = x.Sum(s => s.Amount ?? 0)
                }).ToListAsync();
            }
            else if (groupBy == "agent")
            {
                if (!string.IsNullOrEmpty(classify))
                    query = query.Where(x => x.Agent.Classify == classify);

                res = await query.Where(x => x.AgentId.HasValue).GroupBy(x => new
                {
                    Id = x.AgentId.Value,
                    Name = x.Agent.Name,
                    Classify = x.Agent.Classify
                }).Select(x => new CommissionSettlementOverview()
                {
                    Id = x.Key.Id,
                    Name = x.Key.Name,
                    Classify = x.Key.Classify,
                    BaseAmount = x.Sum(s => s.BaseAmount ?? 0),
                    Amount = x.Sum(s => s.Amount ?? 0)
                }).ToListAsync();
            }

            return res;
        }

        public async Task Recompute()
        {
            var productObj = GetService<IProductService>();
            var commissionObj = GetService<ICommissionService>();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var paymentLineObj = GetService<ISaleOrderPaymentHistoryLineService>();
            var commissionSettlementObj = GetService<ICommissionSettlementService>();

            IList<SaleOrderLine> saleOrderLines = await saleLineObj.SearchQuery(x => x.State != "draft")
                .Include(x => x.Employee).ThenInclude(x => x.Commission)
                .Include(x => x.Assistant).ThenInclude(x => x.AssistantCommission)
                .Include(x => x.Counselor).ThenInclude(x => x.CounselorCommission)
                .ToListAsync();

            IList<SaleOrderPaymentHistoryLine> allPaymentLines = await paymentLineObj.SearchQuery(x => x.SaleOrderPayment.State == "posted")
                .Include(x => x.SaleOrderPayment)
                .ToListAsync();

            IList<CommissionSettlement> allSettlements = await commissionSettlementObj.SearchQuery().ToListAsync();

            IList<CommissionSettlement> createSettlements = new List<CommissionSettlement>();
            IList<CommissionSettlement> updateSettlements = new List<CommissionSettlement>();

            foreach (var saleOrderLine in saleOrderLines)
            {
                decimal totalPaid = 0;
                decimal commissionProfitAmount = 0; //Tổng tiền lợi nhuận đã tính hoa hồng

                //Tổng giá vốn
                var productStdPrice = (decimal)(await productObj.GetHistoryPrice(saleOrderLine.ProductId.Value, saleOrderLine.CompanyId.Value, date: saleOrderLine.Date));
                var totalStandPrice = productStdPrice * saleOrderLine.ProductUOMQty;

                var paymentLines = allPaymentLines.Where(x => x.SaleOrderLineId == saleOrderLine.Id).OrderBy(x => x.SaleOrderPayment.Date).ToList();
                if (paymentLines.Count == 1)
                    continue;

                foreach (var paymentLine in paymentLines)
                {
                    var amountPaid = paymentLine.Amount + totalPaid;
                    var paymentDate = paymentLine.SaleOrderPayment.Date;

                    //Tính lợi nhuận = Số tiền thanh toán - tổng giá vốn
                    var baseAmount = Math.Max(amountPaid - totalStandPrice - commissionProfitAmount, 0);

                    commissionProfitAmount += baseAmount;
                    totalPaid += paymentLine.Amount;

                    var commissionSettlements = allSettlements.Where(x => x.HistoryLineId == paymentLine.Id && x.SaleOrderLineId == saleOrderLine.Id).ToList();

                    if (saleOrderLine.Employee != null && saleOrderLine.Employee.Commission != null)
                    {
                        var commissionPercent = await commissionObj.getCommissionPercent(saleOrderLine.ProductId, saleOrderLine.Employee.CommissionId);

                        var settlement = commissionSettlements.FirstOrDefault(x => x.CommissionId == saleOrderLine.Employee.Commission.Id && x.HistoryLineId == paymentLine.Id);
                        if (settlement != null)
                        {
                            settlement.BaseAmount = baseAmount;
                            settlement.Percentage = commissionPercent;
                            settlement.TotalAmount = paymentLine.Amount;
                            settlement.Amount = baseAmount * commissionPercent / 100;

                            updateSettlements.Add(settlement);
                        }
                        else
                        {
                            createSettlements.Add(new CommissionSettlement
                            {
                                PartnerId = saleOrderLine.Employee.PartnerId,
                                EmployeeId = saleOrderLine.Employee.Id,
                                CommissionId = saleOrderLine.Employee.CommissionId,
                                ProductId = saleOrderLine.ProductId,
                                SaleOrderLineId = saleOrderLine.Id,
                                HistoryLineId = paymentLine.Id,
                                BaseAmount = baseAmount,
                                Amount = baseAmount * commissionPercent / 100,
                                Percentage = commissionPercent,
                                TotalAmount = paymentLine.Amount,
                                CompanyId = saleOrderLine.CompanyId.Value
                            });
                        }
                    }

                    if (saleOrderLine.Assistant != null && saleOrderLine.Assistant.AssistantCommission != null)
                    {
                        var commissionPercent = await commissionObj.getCommissionPercent(saleOrderLine.ProductId, saleOrderLine.Assistant.AssistantCommission.Id);

                        var settlement = commissionSettlements.FirstOrDefault(x => x.CommissionId == saleOrderLine.Assistant.AssistantCommission.Id && x.HistoryLineId == paymentLine.Id);
                        if (settlement != null)
                        {
                            settlement.BaseAmount = baseAmount;
                            settlement.Percentage = commissionPercent;
                            settlement.TotalAmount = paymentLine.Amount;
                            settlement.Amount = baseAmount * commissionPercent / 100;

                            updateSettlements.Add(settlement);
                        }
                        else
                        {
                            createSettlements.Add(new CommissionSettlement
                            {
                                PartnerId = saleOrderLine.Assistant.PartnerId,
                                EmployeeId = saleOrderLine.Assistant.Id,
                                CommissionId = saleOrderLine.Assistant.AssistantCommission.Id,
                                ProductId = saleOrderLine.ProductId,
                                SaleOrderLineId = saleOrderLine.Id,
                                HistoryLineId = paymentLine.Id,
                                BaseAmount = baseAmount,
                                Amount = baseAmount * commissionPercent / 100,
                                Percentage = commissionPercent,
                                TotalAmount = paymentLine.Amount,
                                CompanyId = saleOrderLine.CompanyId.Value
                            });
                        }
                    }

                    if (saleOrderLine.Counselor != null && saleOrderLine.Counselor.CounselorCommission != null)
                    {
                        var commissionPercent = await commissionObj.getCommissionPercent(saleOrderLine.ProductId, saleOrderLine.Counselor.CounselorCommission.Id);

                        var settlement = commissionSettlements.FirstOrDefault(x => x.CommissionId == saleOrderLine.Counselor.CounselorCommission.Id && x.HistoryLineId == paymentLine.Id);
                        if (settlement != null)
                        {
                            settlement.BaseAmount = baseAmount;
                            settlement.Percentage = commissionPercent;
                            settlement.TotalAmount = paymentLine.Amount;
                            settlement.Amount = baseAmount * commissionPercent / 100;

                            updateSettlements.Add(settlement);
                        }
                        else
                        {
                            createSettlements.Add(new CommissionSettlement
                            {
                                PartnerId = saleOrderLine.Counselor.PartnerId,
                                EmployeeId = saleOrderLine.Counselor.Id,
                                CommissionId = saleOrderLine.Counselor.CounselorCommission.Id,
                                ProductId = saleOrderLine.ProductId,
                                SaleOrderLineId = saleOrderLine.Id,
                                HistoryLineId = paymentLine.Id,
                                BaseAmount = baseAmount,
                                Amount = baseAmount * commissionPercent / 100,
                                Percentage = commissionPercent,
                                TotalAmount = amountPaid,
                                CompanyId = saleOrderLine.CompanyId.Value
                            });
                        }
                    }
                }
            }

            await commissionSettlementObj.UpdateAsync(updateSettlements);
            await commissionSettlementObj.CreateAsync(createSettlements);
        }
    }
}
