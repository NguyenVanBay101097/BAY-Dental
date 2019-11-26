import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PartnerReportLocationComponent } from './partner-report-location/partner-report-location.component';

const routes: Routes = [
  {
    path: 'partner-report-location',
    component: PartnerReportLocationComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PartnerReportRoutingModule { }
