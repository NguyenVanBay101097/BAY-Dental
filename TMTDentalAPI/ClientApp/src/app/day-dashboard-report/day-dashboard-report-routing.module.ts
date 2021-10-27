import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DayDashboardReportManagementComponent } from './day-dashboard-report-management/day-dashboard-report-management.component';

const routes: Routes = [
  {
    path: '',
    component: DayDashboardReportManagementComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DayDashboardReportRoutingModule { }
