import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SaleReportRoutingModule } from './sale-report-routing.module';
import { SaleReportOverviewComponent } from './sale-report-overview/sale-report-overview.component';
import { SaleReportService } from './sale-report.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { SaleReportItemDetailComponent } from './sale-report-item-detail/sale-report-item-detail.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SaleReportPartnerComponent } from './sale-report-partner/sale-report-partner.component';
import { SaleReportPartnerDaysFilterComponent } from './sale-report-partner-days-filter/sale-report-partner-days-filter.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [SaleReportOverviewComponent, SaleReportItemDetailComponent, SaleReportPartnerComponent, SaleReportPartnerDaysFilterComponent],
  imports: [
    CommonModule,
    SaleReportRoutingModule,
    MyCustomKendoModule,
    SharedModule,
    FormsModule,
    NgbModule,
    ReactiveFormsModule
  ],
  providers: [
    SaleReportService
  ]
})
export class SaleReportModule { }
