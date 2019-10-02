import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountInvoiceReportsRoutingModule } from './account-invoice-reports-routing.module';
import { AccountInvoiceReportByTimeComponent } from './account-invoice-report-by-time/account-invoice-report-by-time.component';
import { AccountInvoiceReportService } from './account-invoice-report.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule } from '@angular/forms';
import { AccountInvoiceReportByTimeDetailComponent } from './account-invoice-report-by-time-detail/account-invoice-report-by-time-detail.component';

@NgModule({
  declarations: [AccountInvoiceReportByTimeComponent, AccountInvoiceReportByTimeDetailComponent],
  imports: [
    CommonModule,
    AccountInvoiceReportsRoutingModule,
    MyCustomKendoModule,
    FormsModule
  ],
  providers: [
    AccountInvoiceReportService
  ]
})
export class AccountInvoiceReportsModule { }
