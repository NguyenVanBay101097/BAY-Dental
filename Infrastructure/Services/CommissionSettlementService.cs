﻿using ApplicationCore.Entities;
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
            await ComputeAmount(entities);
            await base.UpdateAsync(entities);
        }

        public async Task ComputeAmount(IEnumerable<CommissionSettlement> val)
        {
            var commisstionProductRuleObj = GetService<ICommissionProductRuleService>();
            foreach (var settlement in val)
            {
                if (settlement.PartnerId.HasValue && !settlement.EmployeeId.HasValue)
                    continue;

                var percent = await commisstionProductRuleObj.SearchQuery(x => x.CommissionId == settlement.CommissionId && x.ProductId == settlement.ProductId).Select(x => x.Percent).FirstOrDefaultAsync();
                if (percent <= 0)
                    continue;

                var total = settlement.MoveLine.PriceSubtotal;              
                settlement.TotalAmount = total;
                settlement.BaseAmount = total;
                settlement.Percentage = percent ?? 0;
                settlement.Amount = ((total * (percent ?? 0)) / 100);
            }         
        }


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

        public async Task<PagedResult2<CommissionSettlementReportDetailOutput>> GetReportDetail(CommissionSettlementReport val)
        {
            var query = SearchQuery();

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

            var items = await query.OrderBy(x => x.Date).Skip(val.Offset).Take(val.Limit)
                .Select(x => new CommissionSettlementReportDetailOutput
                {
                    Amount = x.Amount,
                    BaseAmount = x.BaseAmount,
                    Date = x.Date,
                    Percentage = x.Percentage,
                    ProductName = x.Product.Name,
                    PartnerName = x.MoveLine.Partner.Name,
                    InvoiceOrigin = x.MoveLine.Move.InvoiceOrigin
                }).ToListAsync();

            var totalItems = await query.CountAsync();

            return new PagedResult2<CommissionSettlementReportDetailOutput>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public decimal GetStandardPrice(Guid id, Guid? force_company_id = null)
        {
            var propertyObj = GetService<IIRPropertyService>();
            var val = propertyObj.get("standard_price", "product.product", res_id: $"product.product,{id}", force_company: force_company_id);
            return Convert.ToDecimal(val == null ? 0 : val);
        }
    }
}
