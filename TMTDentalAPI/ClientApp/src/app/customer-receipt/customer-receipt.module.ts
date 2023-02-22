import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomerReceiptRoutingModule } from './customer-receipt-routing.module';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { MyCustomNgbModule } from '../shared/my-custom-ngb.module';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    CustomerReceiptRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    MyCustomNgbModule, 
  ]
})
export class CustomerReceiptModule { }
