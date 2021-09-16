import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DayDashboardReportRoutingModule } from './day-dashboard-report-routing.module';
import { DayDashboardReportManagementComponent } from './day-dashboard-report-management/day-dashboard-report-management.component';
import { DayDashboardReportRevenueServiceComponent } from './day-dashboard-report-revenue-service/day-dashboard-report-revenue-service.component';
import { DayDashboardReportRevenueMedicinesComponent } from './day-dashboard-report-revenue-medicines/day-dashboard-report-revenue-medicines.component';
import { DayDashboardReportServiceComponent } from './day-dashboard-report-service/day-dashboard-report-service.component';
import { DayDashboardReportCashbookComponent } from './day-dashboard-report-cashbook/day-dashboard-report-cashbook.component';

@NgModule({
  declarations: [DayDashboardReportManagementComponent, DayDashboardReportRevenueServiceComponent, DayDashboardReportRevenueMedicinesComponent, DayDashboardReportServiceComponent, DayDashboardReportCashbookComponent],
  imports: [
    CommonModule,
    DayDashboardReportRoutingModule
  ]
})
export class DayDashboardReportModule { }
