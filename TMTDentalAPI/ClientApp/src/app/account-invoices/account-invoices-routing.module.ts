import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CustomerInvoiceListComponent } from './customer-invoice-list/customer-invoice-list.component';
import { CustomerInvoiceCreateUpdateComponent } from './customer-invoice-create-update/customer-invoice-create-update.component';
import { AuthGuard } from '../auth/auth.guard';
import { CustomerInvoicePrintComponent } from './customer-invoice-print/customer-invoice-print.component';
import { AccountInvoiceListComponent } from './account-invoice-list/account-invoice-list.component';
import { AccountInvoiceCreateUpdateComponent } from './account-invoice-create-update/account-invoice-create-update.component';

const routes: Routes = [
  {
    path: 'account-invoices',
    component: AccountInvoiceListComponent,
  },
  {
    path: 'account-invoices/create',
    component: AccountInvoiceCreateUpdateComponent,
  },
  {
    path: 'account-invoices/edit/:id',
    component: AccountInvoiceCreateUpdateComponent,
  },
  {
    path: 'customer-invoices',
    component: CustomerInvoiceListComponent,
  },
  {
    path: 'customer-invoices/create',
    component: CustomerInvoiceCreateUpdateComponent
  },
  {
    path: 'customer-invoices/edit/:id',
    component: CustomerInvoiceCreateUpdateComponent
  },
  {
    path: 'customer-invoices/print/:id',
    component: CustomerInvoicePrintComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountInvoicesRoutingModule { }
