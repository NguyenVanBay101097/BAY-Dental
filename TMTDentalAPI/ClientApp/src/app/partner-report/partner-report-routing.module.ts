import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PartnerReportLocationComponent } from './partner-report-location/partner-report-location.component';
import { PartnerReportLocationChartPieComponent } from './partner-report-location-chart-pie/partner-report-location-chart-pie.component';

const routes: Routes = [
  {
    path: 'partner-report-location',
    component: PartnerReportLocationChartPieComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PartnerReportRoutingModule { }
