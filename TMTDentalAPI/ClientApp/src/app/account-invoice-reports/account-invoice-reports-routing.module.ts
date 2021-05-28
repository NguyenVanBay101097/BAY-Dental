import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccountInvoiceReportRevenueComponent } from './account-invoice-report-revenue/account-invoice-report-revenue.component';

const routes: Routes = [
  { path: '', redirectTo: 'time', pathMatch: 'full' },
  { path: 'revenue', component:  AccountInvoiceReportRevenueComponent}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountInvoiceReportsRoutingModule { }
