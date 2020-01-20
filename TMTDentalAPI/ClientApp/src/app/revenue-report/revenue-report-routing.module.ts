import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RevenueReportManagerComponent } from './revenue-report-manager/revenue-report-manager.component';

const routes: Routes = [
  {
    path: 'revenue-report',
    component: RevenueReportManagerComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RevenueReportRoutingModule { }
