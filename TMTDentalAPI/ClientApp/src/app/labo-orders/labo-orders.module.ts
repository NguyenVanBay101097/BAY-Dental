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
import { LaboOrderStatisticUpdateDialogComponent } from './labo-order-statistics/labo-order-statistic-update-dialog/labo-order-statistic-update-dialog.component';
import { LaboOrderDetailListComponent } from './labo-order-detail-list/labo-order-detail-list.component';
import { OrderLaboListComponent } from './order-labo-list/order-labo-list.component';
import { LaboOrderExportComponent } from './labo-order-export/labo-order-export.component';
import { LaboOrderReceiptDialogComponent } from './labo-order-receipt-dialog/labo-order-receipt-dialog.component';
import { LaboOrderExportDialogComponent } from './labo-order-export-dialog/labo-order-export-dialog.component';
import { LaboManagementComponent } from './labo-management/labo-management.component';
import { LaboFinishLinesModule } from '../labo-finish-lines/labo-finish-lines.module';
import { ProductsModule } from '../products/products.module';
import { LaboBridgesModule } from '../labo-bridges/labo-bridges.module';
import { LaboBiteJointsModule } from '../labo-bite-joints/labo-bite-joints.module';

@NgModule({
  declarations: [
    LaboOrderListComponent,
    LaboOrderCreateUpdateComponent,
    LaboOrderQuickCreateDialogComponent,
    LaboOrderStatisticsComponent,
    LaboOrderListDialogComponent,
    LaboOrderStatisticUpdateDialogComponent,
    LaboOrderDetailListComponent,
    OrderLaboListComponent,
    LaboOrderExportComponent,
    LaboOrderReceiptDialogComponent,
    LaboOrderExportDialogComponent,
    LaboManagementComponent,
  ],
  imports: [
    CommonModule,
    LaboOrdersRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    ProductsModule,
    LaboFinishLinesModule,
    LaboBridgesModule,
    LaboBiteJointsModule,
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
    LaboOrderListDialogComponent,
    LaboOrderStatisticUpdateDialogComponent,
    LaboOrderReceiptDialogComponent,
    LaboOrderExportDialogComponent
  ]
})
export class LaboOrdersModule { }
