import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccountInvoiceReportByTimeComponent } from './account-invoice-report-by-time/account-invoice-report-by-time.component';
import { AccountInvoiceReportIndexComponent } from './account-invoice-report-index/account-invoice-report-index.component';

const routes: Routes = [
  {
    path: 'account-invoice-reports',
    component: AccountInvoiceReportIndexComponent,
    children: [
      { path: '', redirectTo: 'time', pathMatch: 'full' },
      { path: 'time', component: AccountInvoiceReportByTimeComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountInvoiceReportsRoutingModule { }
