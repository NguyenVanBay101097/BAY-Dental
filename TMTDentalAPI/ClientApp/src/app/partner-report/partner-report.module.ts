import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PartnerReportRoutingModule } from './partner-report-routing.module';
import { PartnerReportLocationComponent } from './partner-report-location/partner-report-location.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { PartnerReportDistrictComponent } from './partner-report-district/partner-report-district.component';
import { PartnerReportWardComponent } from './partner-report-ward/partner-report-ward.component';
import { PartnerReportLocationFilterComponent } from './partner-report-location-filter/partner-report-location-filter.component';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { PartnerReportLocationChartPieComponent } from './partner-report-location-chart-pie/partner-report-location-chart-pie.component';

@NgModule({
  declarations: [PartnerReportLocationComponent, PartnerReportDistrictComponent, PartnerReportWardComponent, PartnerReportLocationFilterComponent, PartnerReportLocationChartPieComponent],
  imports: [
    CommonModule,
    PartnerReportRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    SharedModule
  ]
})
export class PartnerReportModule { }
