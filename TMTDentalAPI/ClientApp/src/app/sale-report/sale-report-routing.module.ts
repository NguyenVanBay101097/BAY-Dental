import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// import { SaleReportOldNewPartnerComponent } from './sale-report-old-new-partner/sale-report-old-new-partner.component';
import { SaleReportOverviewComponent } from './sale-report-overview/sale-report-overview.component';
// import { SaleReportPartnerComponent } from './sale-report-partner/sale-report-partner.component';
import { ServiceReportManagementComponent } from './service-report-management/service-report-management.component';
import { ServiceReportServiceComponent } from './service-report-service/service-report-service.component';
import { ServiceReportTimeComponent } from './service-report-time/service-report-time.component';
import { ServiceSaleReportComponent } from './service-sale-report/service-sale-report.component';

const routes: Routes = [
  // {
  //   path: '',
  //   component: SaleReportOverviewComponent
  // },
  // {
  //   path: 'partner',
  //   component: SaleReportPartnerComponent
  // }, 
  // {
  //   path: 'old-new-partner',
  //   component: SaleReportOldNewPartnerComponent
  // },
  {
    path: 'service-report',
    component: ServiceReportManagementComponent,
    children: [
      {
        path: '',
        redirectTo: 'overview'
      },
      {
        path: 'overview',
        component: SaleReportOverviewComponent
      },
      {
        path: 'time',
        component: ServiceReportTimeComponent
      },
      {
        path: 'service',
        component: ServiceReportServiceComponent
      },
      {
        path: 'sale',
        component: ServiceSaleReportComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SaleReportRoutingModule { }
