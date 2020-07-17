import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccountReportGeneralLedgerCashBankComponent } from './account-report-general-ledger-cash-bank/account-report-general-ledger-cash-bank.component';

const routes: Routes = [
  {
    path: 'report-cash-bank',
    component: AccountReportGeneralLedgerCashBankComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountReportGeneralLedgersRoutingModule { }
