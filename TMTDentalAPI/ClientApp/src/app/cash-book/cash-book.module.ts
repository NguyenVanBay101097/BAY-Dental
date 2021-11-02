import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CashBookRoutingModule } from './cash-book-routing.module';
import { CashBookComponent } from './cash-book/cash-book.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { CashBookService } from './cash-book.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { CashBookTabPageCaBoComponent } from './cash-book-tab-page-ca-bo/cash-book-tab-page-ca-bo.component';
import { CashBookTabPageRePaComponent } from './cash-book-tab-page-re-pa/cash-book-tab-page-re-pa.component';

@NgModule({
  declarations: [
    CashBookComponent,
    CashBookTabPageCaBoComponent,
    CashBookTabPageRePaComponent
  ],
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
  ]
})
export class CashBookModule { }
