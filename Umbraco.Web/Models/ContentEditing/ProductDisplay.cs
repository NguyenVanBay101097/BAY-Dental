﻿using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductDisplay: ProductBasic
    {
        public ProductDisplay()
        {
            Type = "consu";
            Active = true;
            SaleOK = true;
            PurchaseOK = true;
            LaboPrice = 0;
            PurchasePrice = 0;
        }

        /// <summary>
        /// Giá vốn
        /// </summary>
        public double StandardPrice { get; set; }
        /// <summary>
        /// Xuất xứ
        /// </summary>
        public string Origin { get; set; }
        /// <summary>
        /// Hạn sử dụng (tháng)
        /// </summary>
        public decimal? Expiry { get; set; }

        public IEnumerable<ProductStepDisplay> StepList { get; set; } = new List<ProductStepDisplay>();

        public IEnumerable<Guid> UoMIds { get; set; } = new List<Guid>();

        public UoMBasic UOM { get; set; }

        public UoMBasic UOMPO { get; set; }

        public ProductCategoryBasic Categ { get; set; }

        public string Firm { get; set; }
        public IEnumerable<StockInventoryCriteriaSimple> StockInventoryCriterias { get; set; } = new List<StockInventoryCriteriaSimple>();
        /// <summary>
        /// list định mức vật tư
        /// </summary>
        public IEnumerable<ProductBomBasic> Boms { get; set; } = new List<ProductBomBasic>();

    }

    public class ProductStockInventoryCriteriaRelSimple
    {
        public Guid ProductId { get; set; }
        public Guid StockInventoryCriteriaId { get; set; }
    }
}
