import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountInvoiceReportsRoutingModule } from './account-invoice-reports-routing.module';
import { AccountInvoiceReportService } from './account-invoice-report.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule } from '@angular/forms';
import { AccountInvoiceReportRevenueManageComponent } from './account-invoice-report-revenue-manage/account-invoice-report-revenue-manage.component';
import { SharedModule } from '../shared/shared.module';
import { AccountInvoiceReportRevenueDetailComponent } from './account-invoice-report-revenue-detail/account-invoice-report-revenue-detail.component';
import { SaleOrderReportRevenueComponent } from './sale-order-report-revenue/sale-order-report-revenue.component';
import { AccountInvoiceReportRevenueComponent } from './account-invoice-report-revenue/account-invoice-report-revenue.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AccountInvoiceReportRevenueServiceComponent } from './account-invoice-report-revenue-service/account-invoice-report-revenue-service.component';
import { AccountInvoiceReportRevenueEmployeeComponent } from './account-invoice-report-revenue-employee/account-invoice-report-revenue-employee.component';

@NgModule({
  declarations: [AccountInvoiceReportRevenueDetailComponent, SaleOrderReportRevenueComponent, AccountInvoiceReportRevenueComponent,AccountInvoiceReportRevenueManageComponent,
    AccountInvoiceReportRevenueServiceComponent,
    AccountInvoiceReportRevenueEmployeeComponent
  ],
  imports: [
    CommonModule,
    AccountInvoiceReportsRoutingModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule,
    NgbModule,
  ],
  providers: [
    AccountInvoiceReportService
  ],
  entryComponents: [SaleOrderReportRevenueComponent,AccountInvoiceReportRevenueComponent]
})
export class AccountInvoiceReportsModule { }
