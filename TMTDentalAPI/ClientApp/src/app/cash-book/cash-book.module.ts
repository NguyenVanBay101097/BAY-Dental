import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CashBookRoutingModule } from './cash-book-routing.module';
import { CashBookComponent } from './cash-book/cash-book.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { CashBookCuDialogComponent } from './cash-book-cu-dialog/cash-book-cu-dialog.component';
import { CashBookService } from './cash-book.service';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [CashBookComponent, CashBookCuDialogComponent],
  imports: [
    CommonModule,
    CashBookRoutingModule, 
    NgbModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    SharedModule
  ], 
  providers: [
    CashBookService
  ], 
  entryComponents: [
    CashBookCuDialogComponent
  ]
})
export class CashBookModule { }
