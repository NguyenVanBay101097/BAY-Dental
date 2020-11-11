import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SaleOrdersRoutingModule } from './sale-orders-routing.module';
import { SaleOrderCreateUpdateComponent } from './sale-order-create-update/sale-order-create-update.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SaleOrderListComponent } from './sale-order-list/sale-order-list.component';
import { SaleOrderCreateDotKhamDialogComponent } from './sale-order-create-dot-kham-dialog/sale-order-create-dot-kham-dialog.component';
import { SharedModule } from '../shared/shared.module';
import { SaleOrderApplyCouponDialogComponent } from './sale-order-apply-coupon-dialog/sale-order-apply-coupon-dialog.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SaleOrderCreateLaboOrderDialogComponent } from './sale-order-create-labo-order-dialog/sale-order-create-labo-order-dialog.component';
import { SaleOrderInvoiceListComponent } from './sale-order-invoice-list/sale-order-invoice-list.component';
import { SaleOrderApplyServiceCardsDialogComponent } from './sale-order-apply-service-cards-dialog/sale-order-apply-service-cards-dialog.component';
import { SaleOrderCuDialogComponent } from './sale-order-cu-dialog/sale-order-cu-dialog.component';
import { SaleOrderCuFormComponent } from './sale-order-cu-form/sale-order-cu-form.component';
import { SaleOrderApplyDiscountDefaultDialogComponent } from './sale-order-apply-discount-default-dialog/sale-order-apply-discount-default-dialog.component';
import { SaleOrderLineLaboOrdersDialogComponent } from './sale-order-line-labo-orders-dialog/sale-order-line-labo-orders-dialog.component';
import { LaboOrderCuDialogComponent } from './labo-order-cu-dialog/labo-order-cu-dialog.component';
import { LaboOrderCuLineDialogComponent } from './labo-order-cu-line-dialog/labo-order-cu-line-dialog.component';
import { SaleOrderPaymentDialogComponent } from './sale-order-payment-dialog/sale-order-payment-dialog.component';
import { PartnersModule } from '../partners/partners.module';
import { SaleOrderListServiceComponent } from './sale-order-list-service/sale-order-list-service.component';
import { SaleOrderTeethPopoverComponent } from './sale-order-teeth-popover/sale-order-teeth-popover.component';

@NgModule({
  declarations: [
    SaleOrderCreateUpdateComponent,
    SaleOrderListComponent,
    SaleOrderCreateDotKhamDialogComponent,
    SaleOrderApplyCouponDialogComponent,
    SaleOrderCreateLaboOrderDialogComponent,
    SaleOrderInvoiceListComponent,
    SaleOrderApplyServiceCardsDialogComponent,
    SaleOrderCuDialogComponent,
    SaleOrderCuFormComponent,
    SaleOrderApplyDiscountDefaultDialogComponent,
    SaleOrderLineLaboOrdersDialogComponent,
    LaboOrderCuLineDialogComponent,
    LaboOrderCuDialogComponent,
    SaleOrderPaymentDialogComponent,
    SaleOrderListServiceComponent,
    SaleOrderTeethPopoverComponent,
  ],
  imports: [
    CommonModule,
    SaleOrdersRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    FormsModule,
    SharedModule,
    NgbModule
  ],
  providers: [],
  exports: [
    SaleOrderTeethPopoverComponent
  ],
  entryComponents: [
    SaleOrderCreateDotKhamDialogComponent,
    SaleOrderApplyCouponDialogComponent,
    SaleOrderApplyServiceCardsDialogComponent,
    SaleOrderApplyDiscountDefaultDialogComponent,
    SaleOrderCuDialogComponent,
    SaleOrderLineLaboOrdersDialogComponent,
    LaboOrderCuLineDialogComponent,
    LaboOrderCuDialogComponent,
    SaleOrderPaymentDialogComponent,
  ]
})
export class SaleOrdersModule { }
