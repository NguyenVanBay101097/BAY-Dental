import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RealRevenueReportOverviewComponent } from './real-revenue-report-overview/real-revenue-report-overview.component';

const routes: Routes = [
  {
    path: 'real-revenue-report',
    component: RealRevenueReportOverviewComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RealRevenueReportRoutingModule { }
