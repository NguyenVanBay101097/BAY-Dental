import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { QuotationsRoutingModule } from './quotations-routing.module';
import { QuotationCreateUpdateFormComponent } from './quotation-create-update-form/quotation-create-update-form.component';
import { SaleOrdersModule } from '../sale-orders/sale-orders.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { QuotationService } from './quotation.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';

@NgModule({
  declarations: [QuotationCreateUpdateFormComponent],
  imports: [
    CommonModule,
    QuotationsRoutingModule,
    SaleOrdersModule,
    FormsModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    SharedModule,
    DateInputsModule
  ],
  providers: [QuotationService]
})
export class QuotationsModule { }
