import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccountCommonCustomerReportListComponent } from './account-common-customer-report-list/account-common-customer-report-list.component';
import { PartnerAdvanceListReportComponent } from './partner-advance-list-report/partner-advance-list-report.component';
import { PartnerAreaReportComponent } from './partner-area-report/partner-area-report.component';
import { PartnerDebitListReportComponent } from './partner-debit-list-report/partner-debit-list-report.component';
import { PartnerReportManagementComponent } from './partner-report-management/partner-report-management.component';
import { PartnerReportOverviewComponent } from './partner-report-overview/partner-report-overview.component';
import { PartnerTreatmentReportComponent } from './partner-treatment-report/partner-treatment-report.component';

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
        path: 'partner-advance',
        component: PartnerAdvanceListReportComponent
      },
      {
        path: 'partner-report-overview',
        component: PartnerReportOverviewComponent
      },
      {
        path: 'partner-treatment',
        component: PartnerTreatmentReportComponent
      },
      {
        path: 'partner-area',
        component: PartnerAreaReportComponent
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
