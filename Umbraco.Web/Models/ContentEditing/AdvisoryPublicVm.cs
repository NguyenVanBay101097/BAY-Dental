using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AdvisoryPublicFilter
    {
        public Guid PartnerId { get; set; }
    }

    public class AdvisoryPublicReponse
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string EmployeeName { get; set; }
        public string ToothType { get; set; }
        public IEnumerable<string> Teeth { get; set; } = new List<string>();
        public string TeethDisplay
        {
            get
            {
                switch (ToothType)
                {
                    case "whole_jaw":
                        return "Nguyên hàm";
                    case "upper_jaw":
                        return "Hàm trên";
                    case "lower_jaw":
                        return "Hàm dưới";
                    default:
                        return string.Join(", ", Teeth);
                }
            }
        }

        public IEnumerable<string> ToothDiagnosis { get; set; } = new List<string>();

        public IEnumerable<string> Products { get; set; } = new List<string>();

        public string Note { get; set; }
    }
}
