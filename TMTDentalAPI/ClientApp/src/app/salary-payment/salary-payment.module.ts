import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SalaryPaymentRoutingModule } from './salary-payment-routing.module';
import { SalaryPaymentListComponent } from './salary-payment-list/salary-payment-list.component';
import { SalaryPaymentFormComponent } from './salary-payment-form/salary-payment-form.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '../shared/shared.module';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';

@NgModule({
  declarations: [SalaryPaymentListComponent, SalaryPaymentFormComponent],
  imports: [
    CommonModule,
    SalaryPaymentRoutingModule, 
    FormsModule, 
    ReactiveFormsModule,
    NgbModule, 
    SharedModule,
    MyCustomKendoModule
  ],
  schemas: [ CUSTOM_ELEMENTS_SCHEMA ]
})
export class SalaryPaymentModule { }
