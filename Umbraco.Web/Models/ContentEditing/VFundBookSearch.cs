using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class VFundBookSearch
    {
        public VFundBookSearch()
        {
            Limit = 20;
            Offset = 0;
            InitBal = false;
            State = "all";
        }
        public bool InitBal { get; set; }
        public DateTime? DateTo { get; set; }
        public DateTime? DateFrom { get; set; }
        /// <summary>
        /// all : lay tat ca
        /// posted: phieu dc xac nhan
        /// </summary>
        public string State { get; set; }
        public string Type { get; set; }
        public Guid? CompanyId { get; set; }
        public string Search { get; set; }
        /// <summary>
        /// bank: ngan hang
        /// cash: tien mat
        /// cash_bank: tong quy
        /// </summary>
        public string ResultSelection { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
    }
}
