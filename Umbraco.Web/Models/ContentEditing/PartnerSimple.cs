﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }

    public class PartnerSimpleContact
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }
    }
}
