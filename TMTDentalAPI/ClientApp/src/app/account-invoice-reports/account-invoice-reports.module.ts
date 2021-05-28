import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountInvoiceReportsRoutingModule } from './account-invoice-reports-routing.module';
import { AccountInvoiceReportService } from './account-invoice-report.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule } from '@angular/forms';
import { AccountInvoiceReportRevenueComponent } from './account-invoice-report-revenue/account-invoice-report-revenue.component';
import { SharedModule } from '../shared/shared.module';
import { AccountInvoiceReportRevenueDetailComponent } from './account-invoice-report-revenue-detail/account-invoice-report-revenue-detail.component';

@NgModule({
  declarations: [AccountInvoiceReportRevenueComponent, AccountInvoiceReportRevenueDetailComponent],
  imports: [
    CommonModule,
    AccountInvoiceReportsRoutingModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule
  ],
  providers: [
    AccountInvoiceReportService
  ]
})
export class AccountInvoiceReportsModule { }
