import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountCommonPartnerReportsRoutingModule } from './account-common-partner-reports-routing.module';
import { AccountCommonPartnerReportService } from './account-common-partner-report.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule } from '@angular/forms';
import { AccountCommonCustomerReportListComponent } from './account-common-customer-report-list/account-common-customer-report-list.component';
import { SharedModule } from '../shared/shared.module';
import { AccountCommonPartnerReportListComponent } from './account-common-partner-report-list/account-common-partner-report-list.component';
import { AccountCommonPartnerReportDetailComponent } from './account-common-partner-report-detail/account-common-partner-report-detail.component';

@NgModule({
  declarations: [AccountCommonCustomerReportListComponent, AccountCommonPartnerReportDetailComponent, AccountCommonPartnerReportListComponent],
  imports: [
    CommonModule,
    AccountCommonPartnerReportsRoutingModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule
  ],
  providers: [
    AccountCommonPartnerReportService
  ]
})
export class AccountCommonPartnerReportsModule { }
