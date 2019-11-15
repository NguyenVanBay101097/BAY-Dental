import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SaleReportOverviewComponent } from './sale-report-overview/sale-report-overview.component';

const routes: Routes = [
  {
    path: 'sale-report',
    component: SaleReportOverviewComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SaleReportRoutingModule { }
