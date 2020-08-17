import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountReportGeneralLedgersRoutingModule } from './account-report-general-ledgers-routing.module';
import { AccountReportGeneralLedgerCashBankComponent } from './account-report-general-ledger-cash-bank/account-report-general-ledger-cash-bank.component';
import { ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { AccountReportGeneralLedgerMoveLineComponent } from './account-report-general-ledger-move-line/account-report-general-ledger-move-line.component';

@NgModule({
  declarations: [AccountReportGeneralLedgerCashBankComponent, AccountReportGeneralLedgerMoveLineComponent],
  imports: [
    CommonModule,
    AccountReportGeneralLedgersRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    SharedModule
  ]
})
export class AccountReportGeneralLedgersModule { }
