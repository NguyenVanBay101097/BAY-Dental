import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SaleReportOverviewComponent } from './sale-report-overview/sale-report-overview.component';
import { SaleReportPartnerComponent } from './sale-report-partner/sale-report-partner.component';

const routes: Routes = [
  {
    path: '',
    component: SaleReportOverviewComponent
  },
  {
    path: 'partner',
    component: SaleReportPartnerComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SaleReportRoutingModule { }
