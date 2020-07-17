using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IAccountReportGeneralLedgerService
    {
        ReportGeneralLedgerValues GetCashBankReportValues(ReportCashBankGeneralLedger val);
    }
}
