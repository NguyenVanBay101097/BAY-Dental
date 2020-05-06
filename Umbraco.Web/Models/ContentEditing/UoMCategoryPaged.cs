using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
   public class UoMCategoryPaged
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
    }
}
