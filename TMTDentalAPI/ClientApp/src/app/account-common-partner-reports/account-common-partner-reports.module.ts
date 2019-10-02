import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountCommonPartnerReportsRoutingModule } from './account-common-partner-reports-routing.module';
import { AccountCommonPartnerReportListComponent } from './account-common-partner-report-list/account-common-partner-report-list.component';
import { AccountCommonPartnerReportService } from './account-common-partner-report.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { AccountCommonPartnerReportDetailComponent } from './account-common-partner-report-detail/account-common-partner-report-detail.component';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [AccountCommonPartnerReportListComponent, AccountCommonPartnerReportDetailComponent],
  imports: [
    CommonModule,
    AccountCommonPartnerReportsRoutingModule,
    MyCustomKendoModule,
    FormsModule
  ],
  providers: [
    AccountCommonPartnerReportService
  ]
})
export class AccountCommonPartnerReportsModule { }
