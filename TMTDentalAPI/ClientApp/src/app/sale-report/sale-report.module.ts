import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SaleReportRoutingModule } from './sale-report-routing.module';
import { SaleReportOverviewComponent } from './sale-report-overview/sale-report-overview.component';
import { SaleReportService } from './sale-report.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { SaleReportItemDetailComponent } from './sale-report-item-detail/sale-report-item-detail.component';

@NgModule({
  declarations: [SaleReportOverviewComponent, SaleReportItemDetailComponent],
  imports: [
    CommonModule,
    SaleReportRoutingModule,
    MyCustomKendoModule,
    SharedModule
  ],
  providers: [
    SaleReportService
  ]
})
export class SaleReportModule { }
