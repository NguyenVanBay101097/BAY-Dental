import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccountCommonCustomerReportListComponent } from './account-common-customer-report-list/account-common-customer-report-list.component';

const routes: Routes = [
  {
    path: 'partner',
    component: AccountCommonCustomerReportListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountCommonPartnerReportsRoutingModule { }
