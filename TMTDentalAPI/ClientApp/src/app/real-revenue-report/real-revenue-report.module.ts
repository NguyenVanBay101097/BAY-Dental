import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RealRevenueReportRoutingModule } from './real-revenue-report-routing.module';
import { RealRevenueReportOverviewComponent } from './real-revenue-report-overview/real-revenue-report-overview.component';
import { RealRevenueReportService } from './real-revenue-report.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    RealRevenueReportOverviewComponent,
  ],
  imports: [
    CommonModule,
    RealRevenueReportRoutingModule,
    MyCustomKendoModule,
    SharedModule,
    ReactiveFormsModule
  ],
  providers: [
    RealRevenueReportService
  ]
})
export class RealRevenueReportModule { }
