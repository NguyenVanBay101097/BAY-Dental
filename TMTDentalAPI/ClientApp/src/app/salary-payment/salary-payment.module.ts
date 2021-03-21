import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SalaryPaymentRoutingModule } from './salary-payment-routing.module';
import { SalaryPaymentListComponent } from './salary-payment-list/salary-payment-list.component';
import { SalaryPaymentFormComponent } from './salary-payment-form/salary-payment-form.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '../shared/shared.module';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SalaryPaymentListV2Component } from './salary-payment-list-v2/salary-payment-list-v2.component';
import { SalaryPaymentDialogV2Component } from './salary-payment-dialog-v2/salary-payment-dialog-v2.component';

@NgModule({
  declarations: [SalaryPaymentListComponent, SalaryPaymentFormComponent, SalaryPaymentListV2Component, SalaryPaymentDialogV2Component],
  imports: [
    CommonModule,
    SalaryPaymentRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    SharedModule,
    MyCustomKendoModule
  ],
  entryComponents: [SalaryPaymentDialogV2Component],
})
export class SalaryPaymentModule { }
