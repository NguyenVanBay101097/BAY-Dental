using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CreateServiceCardTypeReq
    {
        public CreateServiceCardTypeReq()
        {
            NbrPeriod = 1;
        }
        public string Name { get; set; }
        public string Period { get; set; }

        public int NbrPeriod { get; set; }
        //public IEnumerable<ProductPricelistItemCreate> ProductPricelistItems { get; set; } = new List<ProductPricelistItemCreate>();
        public Guid? CompanyId { get; set; }
    }

    public class CreateServiceCardTypeRes
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Period { get; set; }

        public int? NbrPeriod { get; set; }
        public IEnumerable<ProductPricelistItemDisplay> ProductPricelistItems { get; set; } = new List<ProductPricelistItemDisplay>();
        public Guid? CompanyId { get; set; }
    }

    public class ServiceCardTypeSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class UpdateProductPricelistItem
    {
        public Guid Id { get; set; }

        public string ComputePrice { get; set; }

        public decimal? PercentPrice { get; set; }

        public decimal? FixedAmountPrice { get; set; }
    }

    public class AddProductPricelistItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
    }

    public class ApplyServiceCategoryReq
    {
        public Guid Id { get; set; }
        public IEnumerable<ServiceCategoryApplyDetail> ServiceCategoryApplyDetails { get; set; } = new List<ServiceCategoryApplyDetail>();
    }

    public class ServiceCategoryApplyDetail
    {
        public Guid CategId { get; set; }
        public string ComputePrice { get; set; }

        public decimal? PercentPrice { get; set; }

        public decimal? FixedAmountPrice { get; set; }
    }

    public class ApplyAllServiceReq
    {
        public Guid Id { get;set; }
        public string ComputePrice { get; set; }

        public decimal? PercentPrice { get; set; }

        public decimal? FixedAmountPrice { get; set; }
    }
}
