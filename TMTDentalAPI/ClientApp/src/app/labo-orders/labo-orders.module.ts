import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LaboOrdersRoutingModule } from './labo-orders-routing.module';
import { LaboOrderListComponent } from './labo-order-list/labo-order-list.component';
import { LaboOrderService } from './labo-order.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { LaboOrderCreateUpdateComponent } from './labo-order-create-update/labo-order-create-update.component';
import { LaboOrderCuLineDialogComponent } from './labo-order-cu-line-dialog/labo-order-cu-line-dialog.component';

@NgModule({
  declarations: [LaboOrderListComponent, LaboOrderCreateUpdateComponent, LaboOrderCuLineDialogComponent],
  imports: [
    CommonModule,
    LaboOrdersRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    MyCustomKendoModule
  ],
  providers: [
    LaboOrderService
  ],
  entryComponents: [
    LaboOrderCuLineDialogComponent
  ]
})
export class LaboOrdersModule { }
