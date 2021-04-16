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
            var partnerCommission = await partnerCommissionObj.SearchQuery(x => x.SaleOrderLineId == linePaymentRel.SaleOrderLineId && x.CommissionId == employee.CommissionId && x.PartnerId == user.PartnerId).FirstOrDefaultAsync();
            if (partnerCommission == null)
                return null;

            var res = new CommissionSettlement
            {
                Partner = user.Partner,
                PartnerId = user.PartnerId,
                Employee = employee,
                EmployeeId = employee.Id,
                SaleOrderLine = linePaymentRel.SaleOrderLine,
                SaleOrderLineId = linePaymentRel.SaleOrderLineId,
                Payment = linePaymentRel.Payment,
                PaymentId = linePaymentRel.PaymentId,
                BaseAmount = linePaymentRel.AmountPrepaid,
                Percentage = partnerCommission.Percentage
            };

            return res;

        }

        public IEnumerable<CommissionSettlement> ComputeAmount(IEnumerable<CommissionSettlement> val , decimal amountPayment)
        {
            foreach (var item in val)
            {
                var amount = item.BaseAmount * amountPayment / item.TotalAmount;
                item.Amount = Math.Abs((decimal)((amount * item.Percentage.Value) / 100));
            }
            return val;

        }

        public async Task Unlink(IEnumerable<Guid> moveLineIds)
        {
            var res = await SearchQuery(x => moveLineIds.Contains(x.MoveLineId.Value)).ToListAsync();
            if (res == null)
                throw new Exception("Null CommissionSettlement");

            await DeleteAsync(res);
        }

        public async Task<IEnumerable<CommissionSettlement>> _PrepareCommission(AccountMove res)
        {
            var commissionSettlements = new List<CommissionSettlement>();      
            foreach(var moveline in res.InvoiceLines)
            {
                var settlements = await _PrepareCommissionByMoveLine(moveline);
                commissionSettlements.AddRange(settlements);
            }
          
            return commissionSettlements;
        }

        public async Task<IEnumerable<CommissionSettlement>> _PrepareCommissionByMoveLine(AccountMoveLine moveLine)
        {
            var commissionSettlements = new List<CommissionSettlement>();
            var employeeObj = GetService<IEmployeeService>();
            var orderLineObj = GetService<ISaleOrderLineService>();
            var commisstionProductRuleObj = GetService<ICommissionProductRuleService>();
            var lines = await orderLineObj.SearchQuery(x => x.SaleOrderLineInvoice2Rels.Any(s=>s.InvoiceLineId == moveLine.Id)).Include(x=>x.Product).Distinct().ToListAsync();
            foreach (var line in lines)
            {
                if (line.EmployeeId.HasValue)
                {
                    var employee = await employeeObj.SearchQuery(x => x.Id == line.EmployeeId)
                        .Include(x => x.Commission).FirstOrDefaultAsync();
                    if (employee == null || employee.Commission == null)
                        return null;

                    var commisstionProductRule_dict = await commisstionProductRuleObj.SearchQuery(x => x.CommissionId == employee.CommissionId.Value).ToDictionaryAsync(x => x.ProductId, x => x);

                    commissionSettlements.Add(new CommissionSettlement
                    {
                        Date = moveLine.Date,
                        Employee = employee,
                        EmployeeId = employee.Id,
                        TotalAmount = line.PriceSubTotal,
                        BaseAmount = (line.Product.ListPrice - (moveLine.Product.PurchasePrice.HasValue ? moveLine.Product.PurchasePrice : 0)),
                        Percentage = commisstionProductRule_dict[moveLine.ProductId].PercentDoctor,
                        MoveLineId = moveLine.Id,
                        Type = "doctor",
                        ProductId = line.ProductId,
                        SaleOrderId = line.OrderId,
                        CommissionId = employee.CommissionId
                    });
                }

                if (line.AssistantId.HasValue)
                {
                    var employee = await employeeObj.SearchQuery(x => x.Id == line.AssistantId)
                        .Include(x => x.Commission).FirstOrDefaultAsync();
                    if (employee == null || employee.Commission == null)
                        return null;

                    var commisstionProductRule_dict = await commisstionProductRuleObj.SearchQuery(x => x.CommissionId == employee.CommissionId.Value).ToDictionaryAsync(x => x.ProductId, x => x);

                    commissionSettlements.Add(new CommissionSettlement
                    {
                        Date = moveLine.Date,
                        Employee = employee,
                        EmployeeId = employee.Id,
                        TotalAmount = line.PriceSubTotal,
                        BaseAmount = (line.Product.ListPrice - (moveLine.Product.PurchasePrice.HasValue ? moveLine.Product.PurchasePrice : 0)),
                        Percentage = commisstionProductRule_dict[moveLine.ProductId].PercentAssistant,
                        MoveLineId = moveLine.Id,
                        Type = "assistant",
                        ProductId = line.ProductId,
                        SaleOrderId = line.OrderId,
                        CommissionId = employee.CommissionId
                    });
                }

                if (line.CounselorId.HasValue)
                {
                    if (!line.CounselorId.HasValue)
                        return null;

                    var employee = await employeeObj.SearchQuery(x => x.Id == line.CounselorId.Value)
                        .Include(x => x.Commission).FirstOrDefaultAsync();
                    if (employee == null || employee.Commission == null)
                        return null;

                    var commisstionProductRule_dict = await commisstionProductRuleObj.SearchQuery(x => x.CommissionId == employee.CommissionId.Value).ToDictionaryAsync(x => x.ProductId, x => x);

                    commissionSettlements.Add(new CommissionSettlement
                    {
                        Date = moveLine.Date,
                        Employee = employee,
                        EmployeeId = employee.Id,
                        TotalAmount = line.PriceSubTotal,
                        BaseAmount = (line.Product.ListPrice - (moveLine.Product.PurchasePrice.HasValue ? moveLine.Product.PurchasePrice : 0)),
                        Percentage = commisstionProductRule_dict[moveLine.ProductId].PercentAdvisory,
                        MoveLineId = moveLine.Id,
                        Type = "advisory",
                        ProductId = line.ProductId,
                        SaleOrderId = line.OrderId,
                        CommissionId = employee.CommissionId
                    });
                }

           
            }

            ComputeAmount(commissionSettlements, moveLine.PriceSubtotal.Value);


            return commissionSettlements;
        }

        public override ISpecification<CommissionSettlement> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "sale.commission_settlement_comp_rule":
                    return new InitialSpecification<CommissionSettlement>(x => !x.Employee.CompanyId.HasValue || companyIds.Contains(x.Employee.CompanyId.Value));
                default:
                    return null;
            }
        }

        public async Task<IEnumerable<CommissionSettlementReportOutput>> GetReport(CommissionSettlementReport val)
        {
            var query = SearchQuery();

            if (val.DateFrom.HasValue)
            {
                val.DateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.Payment.PaymentDate >= val.DateFrom);
            }

            if (val.DateTo.HasValue)
            {
                val.DateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Payment.PaymentDate <= val.DateTo);
            }

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.SaleOrderLine.CompanyId == val.CompanyId);

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

        public async Task<PagedResult2<CommissionSettlementReportDetailOutput>> GetReportDetail(CommissionSettlementReport val)
        {
            var query = SearchQuery();

            if (val.DateFrom.HasValue)
            {
                val.DateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.Payment.PaymentDate >= val.DateFrom);
            }

            if (val.DateTo.HasValue)
            {
                val.DateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Payment.PaymentDate <= val.DateTo);
            }

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.SaleOrderLine.CompanyId == val.CompanyId);

            if (val.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == val.EmployeeId);

            var items = await query.OrderBy(x => x.Payment.PaymentDate).Skip(val.Offset).Take(val.Limit)
                .Select(x => new CommissionSettlementReportDetailOutput
                {
                    Amount = x.Amount,
                    BaseAmount = x.BaseAmount,
                    Date = x.Payment.PaymentDate,
                    Percentage = x.Percentage,
                    ProductName = x.SaleOrderLine.Name
                }).ToListAsync();

            var totalItems = await query.CountAsync();

            return new PagedResult2<CommissionSettlementReportDetailOutput>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }
    }
}
