import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountCommonPartnerReportsRoutingModule } from './account-common-partner-reports-routing.module';
import { AccountCommonPartnerReportService } from './account-common-partner-report.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule } from '@angular/forms';
import { AccountCommonCustomerReportListComponent } from './account-common-customer-report-list/account-common-customer-report-list.component';
import { SharedModule } from '../shared/shared.module';
import { AccountCommonPartnerReportListComponent } from './account-common-partner-report-list/account-common-partner-report-list.component';
import { AccountCommonPartnerReportDetailComponent } from './account-common-partner-report-detail/account-common-partner-report-detail.component';
import { PartnerDebitListReportComponent } from './partner-debit-list-report/partner-debit-list-report.component';
import { PartnerDebitDetailListReportComponent } from './partner-debit-detail-list-report/partner-debit-detail-list-report.component';
import { PartnerReportManagementComponent } from './partner-report-management/partner-report-management.component';
import { PartnerReportOverviewComponent } from './partner-report-overview/partner-report-overview.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PartnerReportSaleOrderComponent } from './partner-report-sale-order/partner-report-sale-order.component';
import { PartnerAreaReportComponent } from './partner-area-report/partner-area-report.component';
import { ChartsModule, ThemeService } from 'ng2-charts';

@NgModule({
  declarations: [AccountCommonCustomerReportListComponent, AccountCommonPartnerReportDetailComponent, AccountCommonPartnerReportListComponent, PartnerDebitListReportComponent, PartnerDebitDetailListReportComponent, PartnerReportManagementComponent, PartnerReportOverviewComponent, PartnerReportSaleOrderComponent, PartnerAreaReportComponent],
  imports: [
    CommonModule,
    AccountCommonPartnerReportsRoutingModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule,
    NgbModule,
    ChartsModule
  ],
  providers: [
    AccountCommonPartnerReportService,
    ThemeService
  ]
})
export class AccountCommonPartnerReportsModule { }
