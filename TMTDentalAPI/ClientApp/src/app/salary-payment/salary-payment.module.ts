import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { SalaryPaymentFormComponent } from './salary-payment-form/salary-payment-form.component';
import { SalaryPaymentListComponent } from './salary-payment-list/salary-payment-list.component';
import { SalaryPaymentRoutingModule } from './salary-payment-routing.module';


@NgModule({
  declarations: [
    SalaryPaymentListComponent,
    SalaryPaymentFormComponent,
  ],
  imports: [
    CommonModule,
    SalaryPaymentRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    SharedModule,
    MyCustomKendoModule
  ],
  entryComponents: [
  ],
})
export class SalaryPaymentModule { }
