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
import { SaleOrderLineDiscountPopoverComponent } from './sale-order-line-discount-popover/sale-order-line-discount-popover.component';
import { SaleOrderLineInfoPopoverComponent } from './sale-order-line-info-popover/sale-order-line-info-popover.component';
import { PartnerCustomerToathuocListComponent } from './partner-customer-toathuoc-list/partner-customer-toathuoc-list.component';
import { SaleOrderDotkhamListComponent } from './sale-order-dotkham-list/sale-order-dotkham-list.component';
import { SaleOrderPaymentListComponent } from './sale-order-payment-list/sale-order-payment-list.component';
import { AccountPaymentPrintComponent } from '../shared/account-payment-print/account-payment-print.component';
import { SaleOrderFastListServiceComponent } from './sale-order-fast-list-service/sale-order-fast-list-service.component';
import { TreatmentProcessServiceListComponent } from './treatment-process-service-list/treatment-process-service-list.component';
import { TreatmentProcessServiceDialogComponent } from './treatment-process-service-dialog/treatment-process-service-dialog.component';
import { SaleOrdersDotkhamCuComponent } from './sale-orders-dotkham-cu/sale-orders-dotkham-cu.component';
import { SaleOrderDotkhamTeethPopoverComponent } from './sale-order-dotkham-teeth-popover/sale-order-dotkham-teeth-popover.component';
import { ToaThuocsModule } from '../toa-thuocs/toa-thuocs.module';
import { SaleOrderProductRequestListComponent } from './sale-order-product-request-list/sale-order-product-request-list.component';
import { SaleOrderProductRequestDialogComponent } from './sale-order-product-request-dialog/sale-order-product-request-dialog.component';
import { SaleOrderLineCuComponent } from './sale-order-line-cu/sale-order-line-cu.component';
import { SaleOrderPromotionDialogComponent } from './sale-order-promotion-dialog/sale-order-promotion-dialog.component';
import { SaleOrderPromotionService } from './sale-order-promotion.service';
import { SaleCouponProgramService } from '../sale-coupon-promotion/sale-coupon-program.service';

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
    PartnerCustomerToathuocListComponent,
    SaleOrderDotkhamListComponent,
    SaleOrderPaymentListComponent,
    SaleOrderLineDiscountPopoverComponent,
    SaleOrderLineInfoPopoverComponent,
    SaleOrderFastListServiceComponent,
    TreatmentProcessServiceListComponent,
    TreatmentProcessServiceDialogComponent,
    SaleOrdersDotkhamCuComponent,
    SaleOrderDotkhamTeethPopoverComponent,
    SaleOrderProductRequestListComponent,
    SaleOrderProductRequestDialogComponent,
    SaleOrderLineCuComponent,
    SaleOrderPromotionDialogComponent,
  ],
  imports: [
    CommonModule,
    SaleOrdersRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    ToaThuocsModule,
    FormsModule,
    SharedModule,
    NgbModule
  ],
  providers: [SaleOrderPromotionService,SaleCouponProgramService],
  exports: [
    SaleOrderLineDiscountPopoverComponent,
    SaleOrderLineInfoPopoverComponent,
    SaleOrderFastListServiceComponent
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
    TreatmentProcessServiceDialogComponent,
    SaleOrdersDotkhamCuComponent,
    SaleOrderProductRequestDialogComponent,
    SaleOrderPromotionDialogComponent
  ]
})
export class SaleOrdersModule { }
