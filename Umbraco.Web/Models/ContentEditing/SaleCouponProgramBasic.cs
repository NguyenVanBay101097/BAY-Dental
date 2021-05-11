﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleCouponProgramBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string Status { get; set; }
        public DateTime? RuleDateFrom { get; set; }
        public DateTime? RuleDateTo { get; set; }
        public int? MaximumUseNumber { get; set; }
    }
}
