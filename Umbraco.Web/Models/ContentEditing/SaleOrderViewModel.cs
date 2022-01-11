﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderViewModel
    {
        public Guid Id { get; set; }

        public DateTime DateOrder { get; set; }

        public Guid PartnerId { get; set; }

        public string PartnerDisplayName { get; set; }

        public decimal? AmountTotal { get; set; }

        public string State { get; set; }

        public string Name { get; set; }

        public decimal? Residual { get; set; }

        public string Type { get; set; }

        /// <summary>
        /// Là phiếu tư vấn
        /// </summary>
        public bool? IsQuotation { get; set; }
    }

    public class SaleOrderSurveyBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOrder { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime? DateDone { get; set; }
    }

    public class ActionDonePar
    {
        public IEnumerable<Guid> Ids { get; set; }
    }

    //public class SaleOrderSimple
    //{
    //    public Guid Id { get; set; }
    //    public string Name { get; set; }
    //}

    public class SaleOrderRevenueReportPaged
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
        public Guid? CompanyId { get; set; }
        public string Search { get; set; }
    }
    /// <summary>
    /// dự kiến doanh thu
    /// </summary>
    public class SaleOrderRevenueReport
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PartnerName { get; set; }
        public decimal? AmountTotal { get; set; }
        public decimal? Residual { get; set; }
        public decimal? TotalPaid { get; set; }
    }

    public class GetRevenueSumTotalRes
    {
        public decimal AmountTotal { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Residual { get; set; }
    }
    public class GetRevenueSumTotalReq
    {
        public Guid? CompanyId { get; set; }
    }

    public class SaleOrderRevenueReportPrint
    {
        public Guid? CompanyId { get; set; }
        public string Search { get; set; }
    }

    public class GetCountSaleOrderFilter
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? CompanyId { get; set; }
    }

    public class GetPrintManagementItemRes: SaleOrderBasic
    {
        public IEnumerable<SaleOrderLineBasic> Lines { get; set; } = new List<SaleOrderLineBasic>();
    }

    public class GetExcelManagementItemRes : SaleOrderBasic
    {
        public IEnumerable<SaleOrderLineDisplay> Lines { get; set; } = new List<SaleOrderLineDisplay>();
    }
    public class GetPrintManagementRes
    {
        public IEnumerable<GetPrintManagementItemRes> Data { get; set; } = new List<GetPrintManagementItemRes>();
        public CompanyPrintVM Company { get; set; }
        public ApplicationUserSimple User { get; set; }
    }

}
