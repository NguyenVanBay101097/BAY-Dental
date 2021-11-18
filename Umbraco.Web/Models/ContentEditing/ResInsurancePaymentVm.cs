using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ResInsurancePaymentSave
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public Guid? ResInsuranceId { get; set; }

        public IEnumerable<ResInsurancePaymentLineSave> Lines { get; set; } = new List<ResInsurancePaymentLineSave>();

        public string Note { get; set; }

    }

    public class ResInsurancePaymentDisplay
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public IEnumerable<ResInsurancePaymentLineDisplay> Lines { get; set; } = new List<ResInsurancePaymentLineDisplay>();

        public string Note { get; set; }

        public string State { get; set; }
    }

    public class ResInsurancePaymentRegisterDisplay
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public IEnumerable<ResInsurancePaymentLineRegisterDisplay> Lines { get; set; } = new List<ResInsurancePaymentLineRegisterDisplay>();

        public string Note { get; set; }

    }
}
