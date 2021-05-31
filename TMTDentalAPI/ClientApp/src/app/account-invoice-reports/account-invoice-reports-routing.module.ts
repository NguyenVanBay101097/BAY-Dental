import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccountInvoiceReportRevenueManageComponent } from './account-invoice-report-revenue-manage/account-invoice-report-revenue-manage.component';

const routes: Routes = [
  { path: '', redirectTo: 'time', pathMatch: 'full' },
  { path: 'revenue', component:  AccountInvoiceReportRevenueManageComponent}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountInvoiceReportsRoutingModule { }
