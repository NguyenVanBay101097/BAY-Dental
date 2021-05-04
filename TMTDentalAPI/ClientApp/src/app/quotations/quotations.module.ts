import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { QuotationsRoutingModule } from './quotations-routing.module';
import { QuotationCreateUpdateFormComponent } from './quotation-create-update-form/quotation-create-update-form.component';
import { SaleOrdersModule } from '../sale-orders/sale-orders.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { QuotationService } from './quotation.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { QuotationLineCuComponent } from './quotation-line-cu/quotation-line-cu.component';

@NgModule({
  declarations: [QuotationCreateUpdateFormComponent, QuotationLineCuComponent],
  imports: [
    CommonModule,
    QuotationsRoutingModule,
    SaleOrdersModule,
    FormsModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    SharedModule,
    NgbModule,
    // NgModule
  ],
  providers: [QuotationService]
})
export class QuotationsModule { }
