import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RealRevenueReportRoutingModule } from './real-revenue-report-routing.module';
import { RealRevenueReportOverviewComponent } from './real-revenue-report-overview/real-revenue-report-overview.component';
import { RealRevenueReportService } from './real-revenue-report.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { RealRevenueReportItemDetailComponent } from './real-revenue-report-item-detail/real-revenue-report-item-detail.component';

@NgModule({
  declarations: [RealRevenueReportOverviewComponent, RealRevenueReportItemDetailComponent],
  imports: [
    CommonModule,
    RealRevenueReportRoutingModule,
    MyCustomKendoModule,
    SharedModule
  ],
  providers: [
    RealRevenueReportService
  ]
})
export class RealRevenueReportModule { }
