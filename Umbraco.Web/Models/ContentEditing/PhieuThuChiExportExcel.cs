using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PhieuThuChiExportExcel
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string JournalName { get; set; }
        public string LoaiThuChiName { get; set; }
        public decimal Amount { get; set; }
        public string PartnerDisplayName { get; set; }
        public string Reason { get; set; }
        public string State { get; set; }
        public string StateDisplay
        {
            get
            {
                switch(State)
                {
                    case "posted":
                        return "Đã xác nhận";
                    case "cancel":
                        return "Đã hủy";
                    default:
                        return "Nháp";
                }
            }
        }
    }
}
