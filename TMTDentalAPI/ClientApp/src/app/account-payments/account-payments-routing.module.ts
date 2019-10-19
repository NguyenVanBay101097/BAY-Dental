import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccountPaymentListComponent } from './account-payment-list/account-payment-list.component';
import { AccountPaymentCreateUpdateComponent } from './account-payment-create-update/account-payment-create-update.component';

const routes: Routes = [
  {
    path: 'accountpayments',
    component: AccountPaymentListComponent
  },
  {
    path: 'accountpayments/edit/:id',
    component: AccountPaymentCreateUpdateComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountPaymentsRoutingModule { }
