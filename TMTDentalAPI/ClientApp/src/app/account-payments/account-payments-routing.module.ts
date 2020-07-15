import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccountPaymentListComponent } from './account-payment-list/account-payment-list.component';
import { AccountPaymentCreateUpdateComponent } from './account-payment-create-update/account-payment-create-update.component';
import { AccountPaymentListTComponent } from './account-payment-list-t/account-payment-list-t.component';

const routes: Routes = [
  {
    path: 'accountpayments',
    component: AccountPaymentListComponent
  },
  {
    path: 'accountpayments/edit/:id',
    component: AccountPaymentCreateUpdateComponent
  },
  {
    path: 'account-payments-t',
    component: AccountPaymentListTComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountPaymentsRoutingModule { }
