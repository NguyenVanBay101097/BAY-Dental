using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class GetAllFilterVM
    {
        public int Limit { get; set; }
        public string Search { get; set; }
        /// <summary>
        /// all tat ca
        /// customer : khach hang
        /// supplier: NCC
        /// </summary>
        public string ResultSelection { get; set; }
    }
}
