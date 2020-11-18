import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SalaryPaymentRoutingModule } from './salary-payment-routing.module';
import { SalaryPaymentListComponent } from './salary-payment-list/salary-payment-list.component';
import { SalaryPaymentFormComponent } from './salary-payment-form/salary-payment-form.component';

@NgModule({
  declarations: [SalaryPaymentListComponent, SalaryPaymentFormComponent],
  imports: [
    CommonModule,
    SalaryPaymentRoutingModule
  ]
})
export class SalaryPaymentModule { }
