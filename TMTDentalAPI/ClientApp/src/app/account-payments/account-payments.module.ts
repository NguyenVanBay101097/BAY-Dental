import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountPaymentsRoutingModule } from './account-payments-routing.module';
import { AccountRegisterPaymentService } from './account-register-payment.service';
import { AccountPaymentListComponent } from './account-payment-list/account-payment-list.component';
import { AccountPaymentService } from './account-payment.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { AccountPaymentCreateUpdateComponent } from './account-payment-create-update/account-payment-create-update.component';
import { SharedModule } from '../shared/shared.module';
import { AccountPaymentListTComponent } from './account-payment-list-t/account-payment-list-t.component';

@NgModule({
  declarations: [AccountPaymentListComponent, AccountPaymentCreateUpdateComponent, AccountPaymentListTComponent],
  imports: [
    CommonModule,
    AccountPaymentsRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    SharedModule
  ],
  providers: [
    AccountRegisterPaymentService,
    AccountPaymentService
  ]
})
export class AccountPaymentsModule { }
