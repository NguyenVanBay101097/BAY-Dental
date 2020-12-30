import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CashBookRoutingModule } from './cash-book-routing.module';
import { CashBookComponent } from './cash-book/cash-book.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { CashBookCuDialogComponent } from './cash-book-cu-dialog/cash-book-cu-dialog.component';
import { CashBookService } from './cash-book.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { CashBookTabCashBankComponent } from './cash-book-tab-cash-bank/cash-book-tab-cash-bank.component';

@NgModule({
  declarations: [CashBookComponent, CashBookCuDialogComponent, CashBookTabCashBankComponent],
  imports: [
    CommonModule,
    CashBookRoutingModule, 
    NgbModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    SharedModule,
    FormsModule
  ], 
  providers: [
    CashBookService
  ], 
  entryComponents: [
    CashBookCuDialogComponent, 
    CashBookTabCashBankComponent
  ]
})
export class CashBookModule { }
