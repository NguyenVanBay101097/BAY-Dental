import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LaboOrdersRoutingModule } from './labo-orders-routing.module';
import { LaboOrderListComponent } from './labo-order-list/labo-order-list.component';
import { LaboOrderService } from './labo-order.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { LaboOrderCreateUpdateComponent } from './labo-order-create-update/labo-order-create-update.component';
import { LaboOrderQuickCreateDialogComponent } from './labo-order-quick-create-dialog/labo-order-quick-create-dialog.component';
import { LaboOrderStatisticsComponent } from './labo-order-statistics/labo-order-statistics.component';
import { LaboOrderLineService } from './labo-order-line.service';
import { LaboOrderListDialogComponent } from './labo-order-list-dialog/labo-order-list-dialog.component';


@NgModule({
  declarations: [
    LaboOrderListComponent,
    LaboOrderCreateUpdateComponent,
    LaboOrderQuickCreateDialogComponent,
    LaboOrderStatisticsComponent,
    LaboOrderListDialogComponent
  ],
  imports: [
    CommonModule,
    LaboOrdersRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    MyCustomKendoModule,
    NgbModule
  ],
  providers: [
    LaboOrderService,
    LaboOrderLineService
  ],
  entryComponents: [
    LaboOrderQuickCreateDialogComponent,
    LaboOrderListDialogComponent
  ]
})
export class LaboOrdersModule { }
