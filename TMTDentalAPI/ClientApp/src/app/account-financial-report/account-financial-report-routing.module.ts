import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccountFinancialViewReportComponent } from './account-financial-view-report/account-financial-view-report.component';

const routes: Routes = [
  { path: 'financial-report', component: AccountFinancialViewReportComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountFinancialReportRoutingModule { }
