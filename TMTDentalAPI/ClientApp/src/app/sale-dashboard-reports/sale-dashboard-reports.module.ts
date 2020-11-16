import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SaleDashboardReportsRoutingModule } from './sale-dashboard-reports-routing.module';
import { SaleDashboardReportFormComponent } from './sale-dashboard-report-form/sale-dashboard-report-form.component';
import { SharedModule } from '@progress/kendo-angular-grid';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AccountCommonPartnerReportsModule } from '../account-common-partner-reports/account-common-partner-reports.module';
import { RevenueReportModule } from '../revenue-report/revenue-report.module';

@NgModule({
  declarations: [SaleDashboardReportFormComponent],
  imports: [
    CommonModule,
    SaleDashboardReportsRoutingModule,
    SharedModule,
    MyCustomKendoModule,
    FormsModule,
    ReactiveFormsModule,
    AccountCommonPartnerReportsModule,
    RevenueReportModule
  ]
})
export class SaleDashboardReportsModule { }
