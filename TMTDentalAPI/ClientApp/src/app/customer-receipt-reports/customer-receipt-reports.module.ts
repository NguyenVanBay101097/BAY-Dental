import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { MyCustomNgbModule } from '../shared/my-custom-ngb.module';
import { CustomerReceiptReportRoutingModule } from './CustomerReceiptRoutingReport.module';


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    CustomerReceiptReportRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    MyCustomNgbModule,
  ]
})
export class CustomerReceiptReportsModule { }
