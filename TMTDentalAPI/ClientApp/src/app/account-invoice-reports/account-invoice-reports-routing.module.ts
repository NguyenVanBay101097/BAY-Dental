import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccountInvoiceReportByTimeComponent } from './account-invoice-report-by-time/account-invoice-report-by-time.component';

const routes: Routes = [
  {
    path: 'account-invoice-reports/index',
    component: AccountInvoiceReportByTimeComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountInvoiceReportsRoutingModule { }
