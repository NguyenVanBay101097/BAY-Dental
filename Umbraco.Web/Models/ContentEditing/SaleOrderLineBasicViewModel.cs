﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderLineBasicViewModel
    {
        public SaleOrderLineBasicViewModel()
        {
            State = "draft";
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ProductId { get; set; }
        public ProductSimple Product { get; set; }
        public string Diagnostic { get; set; }
        public Guid? ToothCategoryId { get; set; }
        public ToothCategoryBasic ToothCategory { get; set; }
        public IEnumerable<ToothDisplay> Teeth { get; set; } = new List<ToothDisplay>();
        public IEnumerable<DotKhamStepBasic> DotKhamSteps { get; set; } = new List<DotKhamStepBasic>();
        public string State { get; set; }
        public int? Sequence { get; set; }
        public Guid OrderId { get; set; }
        public SaleOrderBasic Order { get; set; }
        public Guid? OrderPartnerId { get; set; }
        public PartnerSimple OrderPartner { get; set; }

    }
}
