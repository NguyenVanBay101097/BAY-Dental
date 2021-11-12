using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models.PrintTemplate
{
    public class AdvisoryPrintTemplate
    {
        public CompanyPrintTemplate Company { get; set; }
        public PartnerPrintTemplate Partner { get; set; }
        public IEnumerable<AdvisoryItemPrintTemplate> Advisories { get; set; }
    }

    public class AdvisoryItemPrintTemplate
    {
        //Ngày tư vấn
        public DateTime Date { get; set; }

        /// <summary>
        /// Người tư vấn
        /// </summary>
        public EmployeePrintTemplate Employee { get; set; }

        public string ToothType { get; set; }

        /// <summary>
        /// ToothType Or list name tooth string join
        /// </summary>
        public string Tooths { get; set; }



        /// <summary>
        /// list name Diagnosis string join
        /// </summary>
        public string Diagnosis { get; set; }

        /// <summary>
        /// list name services
        /// </summary>
        public string Services { get; set; }

    }
}
