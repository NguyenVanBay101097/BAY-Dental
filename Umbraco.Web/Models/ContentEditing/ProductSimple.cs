﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductSimple
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public UoMDisplay UOMPO { get; set; }
    }
}
