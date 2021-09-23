import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DayDashboardReportRoutingModule } from './day-dashboard-report-routing.module';
import { DayDashboardReportManagementComponent } from './day-dashboard-report-management/day-dashboard-report-management.component';
import { DayDashboardReportRevenueServiceComponent } from './day-dashboard-report-revenue-service/day-dashboard-report-revenue-service.component';
import { DayDashboardReportRevenueMedicinesComponent } from './day-dashboard-report-revenue-medicines/day-dashboard-report-revenue-medicines.component';
import { DayDashboardReportCashbookComponent } from './day-dashboard-report-cashbook/day-dashboard-report-cashbook.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '../shared/shared.module';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { DayDashboardReportRegistrationServiceComponent } from './day-dashboard-report-registration-service/day-dashboard-report-registration-service.component';
import { DayDashboardReportService } from './day-dashboard-report.service';
import { DayDashboardReportRevenueComponent } from './day-dashboard-report-revenue/day-dashboard-report-revenue.component';

@NgModule({
  declarations: [DayDashboardReportManagementComponent, DayDashboardReportRevenueServiceComponent, DayDashboardReportRevenueMedicinesComponent, DayDashboardReportCashbookComponent, DayDashboardReportRegistrationServiceComponent, DayDashboardReportRevenueComponent],
  imports: [
    CommonModule,
    DayDashboardReportRoutingModule,
    MyCustomKendoModule,
    SharedModule,
    FormsModule,
    NgbModule,
    ReactiveFormsModule
  ],
  providers: [DayDashboardReportService],
})
export class DayDashboardReportModule { }
