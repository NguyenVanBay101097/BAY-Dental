﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookPaged
    {
        public FacebookPaged()
        {
            Limit = 10;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
    }
}
