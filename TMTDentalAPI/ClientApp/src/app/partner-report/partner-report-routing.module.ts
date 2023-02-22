import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PartnerReportLocationChartPieComponent } from './partner-report-location-chart-pie/partner-report-location-chart-pie.component';

const routes: Routes = [
  {
    path: '',
    component: PartnerReportLocationChartPieComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PartnerReportRoutingModule { }
