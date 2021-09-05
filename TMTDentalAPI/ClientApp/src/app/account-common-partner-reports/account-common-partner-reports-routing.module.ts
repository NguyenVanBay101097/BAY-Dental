import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccountCommonCustomerReportListComponent } from './account-common-customer-report-list/account-common-customer-report-list.component';
import { PartnerDebitListReportComponent } from './partner-debit-list-report/partner-debit-list-report.component';
import { PartnerReportManagementComponent } from './partner-report-management/partner-report-management.component';
import { PartnerReportOverviewComponent } from './partner-report-overview/partner-report-overview.component';

const routes: Routes = [
  {
    path: 'partner',
    component: AccountCommonCustomerReportListComponent
  },
  {
    path: 'partner-debit-report',
    component: PartnerDebitListReportComponent
  },
  {
    path: '',
    component: PartnerReportManagementComponent,
    children: [
      {
        path: 'partner-debit',
        component: PartnerDebitListReportComponent
      },
      {
        path: 'partner-report-overview',
        component: PartnerReportOverviewComponent
      },
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'partner-report-overview'
      },
    ]
  }
 
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountCommonPartnerReportsRoutingModule { }
