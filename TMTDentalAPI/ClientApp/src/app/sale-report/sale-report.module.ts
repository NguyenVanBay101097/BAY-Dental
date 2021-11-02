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
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SaleReportOldNewPartnerComponent } from './sale-report-old-new-partner/sale-report-old-new-partner.component';
import { SaleReportPartnerDetailComponent } from './sale-report-partner-detail/sale-report-partner-detail.component';
import { PartnerOldNewReportService } from './partner-old-new-report.service';
import { ServiceReportManagementComponent } from './service-report-management/service-report-management.component';
import { ServiceReportTimeComponent } from './service-report-time/service-report-time.component';
import { ServiceReportServiceComponent } from './service-report-service/service-report-service.component';
import { ServiceReportDetailComponent } from './service-report-detail/service-report-detail.component';
import { ServiceSaleReportComponent } from './service-sale-report/service-sale-report.component';

@NgModule({
  declarations: [
    SaleReportOverviewComponent,
    SaleReportItemDetailComponent,
    SaleReportPartnerComponent,
    SaleReportOldNewPartnerComponent,
    SaleReportPartnerDetailComponent,
    ServiceReportManagementComponent,
    ServiceReportTimeComponent,
    ServiceReportServiceComponent,
    ServiceReportDetailComponent,
    ServiceSaleReportComponent],
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
    SaleReportService, PartnerOldNewReportService
  ],
  exports: [SaleReportItemDetailComponent]
})
export class SaleReportModule { }
