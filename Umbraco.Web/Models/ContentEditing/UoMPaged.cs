﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class UoMPaged
    {
        public UoMPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
        public Guid? CategoryId { get; set; }
    }

    public class UoMImportExcelRow
    {
        public string Name { get; set; }

        public string CategName { get; set; }

        public string Type { get; set; }

        public decimal Factor { get; set; }
    }
}
