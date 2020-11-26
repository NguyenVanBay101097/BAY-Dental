import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SaleDashboardReportFormComponent } from './sale-dashboard-report-form/sale-dashboard-report-form.component';

const routes: Routes = [
  { path: '', component: SaleDashboardReportFormComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SaleDashboardReportsRoutingModule { }
