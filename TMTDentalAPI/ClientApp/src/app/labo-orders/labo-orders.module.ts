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
import { LaboOrderCuDialogComponent } from './labo-order-cu-dialog/labo-order-cu-dialog.component';
import { LaboOrderQuickCreateDialogComponent } from './labo-order-quick-create-dialog/labo-order-quick-create-dialog.component';
import { LaboOrderStatisticsComponent } from './labo-order-statistics/labo-order-statistics.component';

@NgModule({
  declarations: [
    LaboOrderListComponent,
    LaboOrderCreateUpdateComponent,
    LaboOrderCuLineDialogComponent,
    LaboOrderCuDialogComponent,
    LaboOrderQuickCreateDialogComponent,
    LaboOrderStatisticsComponent
  ],
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
    LaboOrderCuLineDialogComponent,
    LaboOrderCuDialogComponent,
    LaboOrderQuickCreateDialogComponent
  ]
})
export class LaboOrdersModule { }
