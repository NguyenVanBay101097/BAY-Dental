﻿using Kendo.DynamicLinqCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderLinesPaged
    {
        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

        public Guid? OrderPartnerId { get; set; }

        public Guid? OrderId { get; set; }

        public bool IsLabo { get; set; }

        public string State { get; set; }

        public Guid? ProductId { get; set; }

        public DateTime? DateOrderFrom { get; set; }

        public DateTime? DateOrderTo { get; set; }
        public bool? IsQuotation { get; set; }

        /// <summary>
        /// draft: Nháp
        /// confirmed: Đơn hàng
        /// </summary>
        public string LaboState { get; set; }

        /// <summary>
        /// not_created: Chưa tạo
        /// created: Đã tạo
        /// </summary>
        public string LaboStatus { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? CompanyId { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public IEnumerable<Aggregator> Aggregate { get; set; }
        public Guid? PartnerId { get; set; }
    }

    public class SaleOrderLinesLaboPaged
    {
        public SaleOrderLinesLaboPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

        /// <summary>
        /// true: Đã tạo
        /// false: Chưa tạo
        /// </summary>
        public bool? HasAnyLabo { get; set; }

        /// <summary>
        /// draft: Nháp
        /// confirmed: Đã tạo
        /// </summary>
        public string LaboState { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class SaleOrderLineIsActivePatch
    {
        public bool IsActive { get; set; }
 
    }

}
