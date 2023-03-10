import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountCommonPartnerReportsRoutingModule } from './account-common-partner-reports-routing.module';
import { AccountCommonPartnerReportService } from './account-common-partner-report.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AccountCommonCustomerReportListComponent } from './account-common-customer-report-list/account-common-customer-report-list.component';
import { SharedModule } from '../shared/shared.module';
import { PartnerDebitListReportComponent } from './partner-debit-list-report/partner-debit-list-report.component';
import { PartnerDebitDetailListReportComponent } from './partner-debit-detail-list-report/partner-debit-detail-list-report.component';
import { PartnerReportManagementComponent } from './partner-report-management/partner-report-management.component';
import { PartnerReportOverviewComponent } from './partner-report-overview/partner-report-overview.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PartnerReportSaleOrderComponent } from './partner-report-sale-order/partner-report-sale-order.component';
import { PartnerAdvanceListReportComponent } from './partner-advance-list-report/partner-advance-list-report.component';
import { PartnerAdvanceDetailListReportComponent } from './partner-advance-detail-list-report/partner-advance-detail-list-report.component';
import { PartnerAreaReportComponent } from './partner-area-report/partner-area-report.component';
import { AccountInvoiceReportService } from '../account-invoice-reports/account-invoice-report.service';
import { PartnerTreatmentReportComponent } from './partner-treatment-report/partner-treatment-report.component';
import { PartnerReportAreaComponent } from './partner-report-area/partner-report-area.component';
import { PartnerReportSourceComponent } from './partner-report-source/partner-report-source.component';
import { PartnerReportAgeGenderComponent } from './partner-report-age-gender/partner-report-age-gender.component';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { PartnerReportFilterPopupComponent } from './partner-report-filter-popup/partner-report-filter-popup.component';
import { PartnerTreatmentReportFilterPopupComponent } from './partner-treatment-report-filter-popup/partner-treatment-report-filter-popup.component';

@NgModule({
  declarations: [
    AccountCommonCustomerReportListComponent,
    PartnerDebitListReportComponent,
    PartnerDebitDetailListReportComponent,
    PartnerReportManagementComponent,
    PartnerReportOverviewComponent,
    PartnerReportSaleOrderComponent,
    PartnerAdvanceListReportComponent,
    PartnerAdvanceDetailListReportComponent,
    PartnerAreaReportComponent,
    PartnerTreatmentReportComponent,
    PartnerReportAreaComponent,
    PartnerReportSourceComponent,
    PartnerReportAgeGenderComponent,
    PartnerReportFilterPopupComponent,
    PartnerTreatmentReportFilterPopupComponent],
  imports: [
    CommonModule,
    AccountCommonPartnerReportsRoutingModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule,
    NgbModule,
    DropDownsModule,
    ReactiveFormsModule
  ],
  providers: [
    AccountCommonPartnerReportService,
    // ThemeService,
    AccountInvoiceReportService
  ]
})
export class AccountCommonPartnerReportsModule { }
