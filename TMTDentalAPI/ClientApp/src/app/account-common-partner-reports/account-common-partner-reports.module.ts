import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountCommonPartnerReportsRoutingModule } from './account-common-partner-reports-routing.module';
import { AccountCommonPartnerReportService } from './account-common-partner-report.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule } from '@angular/forms';
import { AccountCommonCustomerReportListComponent } from './account-common-customer-report-list/account-common-customer-report-list.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [AccountCommonCustomerReportListComponent],
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
