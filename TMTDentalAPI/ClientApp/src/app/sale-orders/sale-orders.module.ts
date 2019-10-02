import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SaleOrdersRoutingModule } from './sale-orders-routing.module';
import { SaleOrderCreateUpdateComponent } from './sale-order-create-update/sale-order-create-update.component';
import { ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SaleOrderService } from './sale-order.service';
import { SaleOrderLineDialogComponent } from './sale-order-line-dialog/sale-order-line-dialog.component';

@NgModule({
  declarations: [SaleOrderCreateUpdateComponent, SaleOrderLineDialogComponent],
  imports: [
    CommonModule,
    SaleOrdersRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule
  ],
  providers: [
    SaleOrderService
  ],
  entryComponents: [
    SaleOrderLineDialogComponent
  ]
})
export class SaleOrdersModule { }
