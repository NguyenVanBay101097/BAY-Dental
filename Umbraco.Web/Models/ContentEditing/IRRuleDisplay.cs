﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class IRRuleDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Global { get; set; }

        public bool Active { get; set; }

        public bool PermUnlink { get; set; }

        public bool PermWrite { get; set; }

        public bool PermRead { get; set; }

        public bool PermCreate { get; set; }

        public Guid ModelId { get; set; }
        public IRModelBasic Model { get; set; }

        public string Code { get; set; }
    }
}
