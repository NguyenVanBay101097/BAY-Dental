import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SaleOrdersRoutingModule } from './sale-orders-routing.module';
import { SaleOrderCreateUpdateComponent } from './sale-order-create-update/sale-order-create-update.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SaleOrderService } from './sale-order.service';
import { SaleOrderLineDialogComponent } from './sale-order-line-dialog/sale-order-line-dialog.component';
import { SaleOrderListComponent } from './sale-order-list/sale-order-list.component';
import { SaleOrderLineService } from './sale-order-line.service';

@NgModule({
  declarations: [SaleOrderCreateUpdateComponent, SaleOrderLineDialogComponent, SaleOrderListComponent],
  imports: [
    CommonModule,
    SaleOrdersRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule
  ],
  providers: [
    SaleOrderService,
    SaleOrderLineService
  ],
  entryComponents: [
    SaleOrderLineDialogComponent
  ]
})
export class SaleOrdersModule { }
