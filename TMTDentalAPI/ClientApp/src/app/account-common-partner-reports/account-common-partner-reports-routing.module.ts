import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccountCommonPartnerReportListComponent } from './account-common-partner-report-list/account-common-partner-report-list.component';

const routes: Routes = [
  {
    path: 'account-common-partner-reports',
    component: AccountCommonPartnerReportListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountCommonPartnerReportsRoutingModule { }
