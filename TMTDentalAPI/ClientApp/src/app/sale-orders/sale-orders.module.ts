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
import { SaleOrderCreateDotKhamDialogComponent } from './sale-order-create-dot-kham-dialog/sale-order-create-dot-kham-dialog.component';
import { SharedModule } from '../shared/shared.module';
import { SaleOrderApplyCouponDialogComponent } from './sale-order-apply-coupon-dialog/sale-order-apply-coupon-dialog.component';

@NgModule({
  declarations: [SaleOrderCreateUpdateComponent, SaleOrderLineDialogComponent, SaleOrderListComponent, SaleOrderCreateDotKhamDialogComponent, SaleOrderApplyCouponDialogComponent],
  imports: [
    CommonModule,
    SaleOrdersRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule
  ],
  providers: [
    SaleOrderService,
    SaleOrderLineService
  ],
  entryComponents: [
    SaleOrderLineDialogComponent,
    SaleOrderCreateDotKhamDialogComponent,
    SaleOrderApplyCouponDialogComponent
  ]
})
export class SaleOrdersModule { }
