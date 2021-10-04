using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models.PrintTemplate
{
    public class ToaThuocPrintTemplate
    {
        public CompanyPrintTemplate Company { get; set; }

        public PartnerPrintTemplate Partner { get; set; }

        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public string Note { get; set; }
        public string Diagnostic { get; set; }
        public string EmployeeName { get; set; }

        public DateTime? ReExaminationDate { get; set; }

        public IEnumerable<ToaThuocLinePrintTemplate> Lines { get; set; } = new List<ToaThuocLinePrintTemplate>();
    }

    public class ToaThuocLinePrintTemplate
    {
        public string ProductName { get; set; }
        public string ProductUOMName { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public int? Sequence { get; set; }
        public int NumberOfTimes { get; set; }
        public int NumberOfDays { get; set; }
        public decimal AmountOfTimes { get; set; }
        public string UseAt { get; set; }
        public string Note { get; set; }
        public string GetUseAtDisplay
        {
            get
            {
                switch (UseAt)
                {
                    case "after_meal": return "Sau khi ăn";
                    case "before_meal": return "Trước khi ăn";
                    case "in_meal": return "Trong khi ăn";
                    case "after_wakeup": return "Sau khi dậy";
                    case "before_sleep": return "Trước khi đi ngủ";
                    case "other": return (string.IsNullOrEmpty(Note) ? "Khác" : Note);
                    default:
                        return "";
                }
            }
            set
            {

            }
        }
    }
}
