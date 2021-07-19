import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccountCommonCustomerReportListComponent } from './account-common-customer-report-list/account-common-customer-report-list.component';
import { PartnerDebitListReportComponent } from './partner-debit-list-report/partner-debit-list-report.component';

const routes: Routes = [
  {
    path: 'partner',
    component: AccountCommonCustomerReportListComponent
  },
  {
    path: 'partner-debit',
    component: PartnerDebitListReportComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountCommonPartnerReportsRoutingModule { }
