import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { MyCustomNgbModule } from '../shared/my-custom-ngb.module';
import { CustomerReceiptReportRoutingModule } from './CustomerReceiptRoutingReport.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CustomerReceiptReportManageComponent } from './customer-receipt-report-manage/customer-receipt-report-manage.component';
import { CustomerReceiptReportOverviewComponent } from './customer-receipt-report-overview/customer-receipt-report-overview.component';
import { CustomerReceiptReportForTimeComponent } from './customer-receipt-report-for-time/customer-receipt-report-for-time.component';
import { CustomerReceiptReportNoTreatmentComponent } from './customer-receipt-report-no-treatment/customer-receipt-report-no-treatment.component';
import { CustomerReceiptReportTimeserviceComponent } from './customer-receipt-report-timeservice/customer-receipt-report-timeservice.component';
import { CustomerReceiptReportForTimeDetailComponent } from './customer-receipt-report-for-time-detail/customer-receipt-report-for-time-detail.component';


@NgModule({
  declarations: [
    CustomerReceiptReportManageComponent,
    CustomerReceiptReportOverviewComponent,
    CustomerReceiptReportForTimeComponent,
    CustomerReceiptReportNoTreatmentComponent,
    CustomerReceiptReportTimeserviceComponent,
    CustomerReceiptReportForTimeDetailComponent
  ],
  imports: [
    CommonModule,
    CustomerReceiptReportRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    NgbModule,
    MyCustomNgbModule,
  ]
})
export class CustomerReceiptReportsModule { }
