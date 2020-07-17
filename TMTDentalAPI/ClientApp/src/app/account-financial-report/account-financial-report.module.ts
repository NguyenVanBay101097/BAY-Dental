import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountFinancialReportRoutingModule } from './account-financial-report-routing.module';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AccountFinancialViewReportComponent } from './account-financial-view-report/account-financial-view-report.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [AccountFinancialViewReportComponent],
  imports: [
    CommonModule,
    AccountFinancialReportRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule
  ]
})
export class AccountFinancialReportModule { }
