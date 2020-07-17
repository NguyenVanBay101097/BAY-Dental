﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PhieuThuChiPaged
    {
        public PhieuThuChiPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        /// <summary>
        /// thu
        /// chi
        /// </summary>
        public string Type { get; set; }

        public string Search { get; set; }
    }
}