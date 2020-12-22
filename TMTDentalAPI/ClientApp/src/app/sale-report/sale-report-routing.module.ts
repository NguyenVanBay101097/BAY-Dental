import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SaleReportOldNewPartnerComponent } from './sale-report-old-new-partner/sale-report-old-new-partner.component';
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
  }, 
  {
    path: 'old-new-partner',
    component: SaleReportOldNewPartnerComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SaleReportRoutingModule { }
