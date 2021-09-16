import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DayDashboardReportRoutingModule } from './day-dashboard-report-routing.module';
import { DayDashboardReportManagementComponent } from './day-dashboard-report-management/day-dashboard-report-management.component';

@NgModule({
  declarations: [DayDashboardReportManagementComponent],
  imports: [
    CommonModule,
    DayDashboardReportRoutingModule
  ]
})
export class DayDashboardReportModule { }
