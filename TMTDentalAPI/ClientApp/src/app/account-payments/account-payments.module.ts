import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountPaymentsRoutingModule } from './account-payments-routing.module';
import { AccountRegisterPaymentService } from './account-register-payment.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    AccountPaymentsRoutingModule
  ],
  providers: [
    AccountRegisterPaymentService
  ]
})
export class AccountPaymentsModule { }
