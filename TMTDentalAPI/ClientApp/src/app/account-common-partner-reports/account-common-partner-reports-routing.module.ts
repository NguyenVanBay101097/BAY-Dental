import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccountCommonCustomerReportListComponent } from './account-common-customer-report-list/account-common-customer-report-list.component';
import { AccountCommonPartnerReportListComponent } from './account-common-partner-report-list/account-common-partner-report-list.component';

const routes: Routes = [
  {
    path: 'partner',
    component: AccountCommonPartnerReportListComponent
  },
  {
    path: 'customer',
    component: AccountCommonCustomerReportListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountCommonPartnerReportsRoutingModule { }
