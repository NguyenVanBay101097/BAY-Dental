import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountInvoicesRoutingModule } from './account-invoices-routing.module';
import { CustomerInvoiceListComponent } from './customer-invoice-list/customer-invoice-list.component';
import { CustomerInvoiceCreateUpdateComponent } from './customer-invoice-create-update/customer-invoice-create-update.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AccountInvoiceLineDialogComponent } from './account-invoice-line-dialog/account-invoice-line-dialog.component';
import { AccountInvoiceLineService } from './account-invoice-line.service';
import { AccountInvoiceService } from './account-invoice.service';
import { AccountInvoiceRegisterPaymentDialogComponent } from './account-invoice-register-payment-dialog/account-invoice-register-payment-dialog.component';
import { InvoiceCreateDotkhamDialogComponent } from './invoice-create-dotkham-dialog/invoice-create-dotkham-dialog.component';
import { CustomerInvoicePrintComponent } from './customer-invoice-print/customer-invoice-print.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AccountInvoiceAdvanceSearchComponent } from './account-invoice-advance-search/account-invoice-advance-search.component';
import { AccountInvoiceRegisterPaymentDialogV2Component } from './account-invoice-register-payment-dialog-v2/account-invoice-register-payment-dialog-v2.component';

@NgModule({
  declarations: [CustomerInvoiceListComponent, CustomerInvoiceCreateUpdateComponent, AccountInvoiceLineDialogComponent, AccountInvoiceRegisterPaymentDialogComponent, InvoiceCreateDotkhamDialogComponent, CustomerInvoicePrintComponent, AccountInvoiceAdvanceSearchComponent, AccountInvoiceRegisterPaymentDialogV2Component],
  imports: [
    CommonModule,
    AccountInvoicesRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    NgbModule
  ],
  exports: [
    CustomerInvoicePrintComponent
  ],
  providers: [
    AccountInvoiceLineService,
    AccountInvoiceService
  ],
  entryComponents: [
    AccountInvoiceLineDialogComponent,
    AccountInvoiceRegisterPaymentDialogComponent,
    InvoiceCreateDotkhamDialogComponent,
    AccountInvoiceRegisterPaymentDialogV2Component
  ]
})
export class AccountInvoicesModule { }
