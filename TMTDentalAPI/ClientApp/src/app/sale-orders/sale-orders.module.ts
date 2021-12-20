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
import { SaleOrderApplyServiceCardsDialogComponent } from './sale-order-apply-service-cards-dialog/sale-order-apply-service-cards-dialog.component';
import { SaleOrderCuFormComponent } from './sale-order-cu-form/sale-order-cu-form.component';
import { SaleOrderApplyDiscountDefaultDialogComponent } from './sale-order-apply-discount-default-dialog/sale-order-apply-discount-default-dialog.component';
import { SaleOrderLineLaboOrdersDialogComponent } from './sale-order-line-labo-orders-dialog/sale-order-line-labo-orders-dialog.component';
import { LaboOrderCuDialogComponent } from './labo-order-cu-dialog/labo-order-cu-dialog.component';
import { LaboOrderCuLineDialogComponent } from './labo-order-cu-line-dialog/labo-order-cu-line-dialog.component';
import { SaleOrderPaymentDialogComponent } from './sale-order-payment-dialog/sale-order-payment-dialog.component';
import { PartnerCustomerToathuocListComponent } from './partner-customer-toathuoc-list/partner-customer-toathuoc-list.component';
import { SaleOrderPaymentListComponent } from './sale-order-payment-list/sale-order-payment-list.component';
import { TreatmentProcessServiceListComponent } from './treatment-process-service-list/treatment-process-service-list.component';
import { TreatmentProcessServiceDialogComponent } from './treatment-process-service-dialog/treatment-process-service-dialog.component';
import { SaleOrdersDotkhamCuComponent } from './sale-orders-dotkham-cu/sale-orders-dotkham-cu.component';
import { SaleOrderDotkhamTeethPopoverComponent } from './sale-order-dotkham-teeth-popover/sale-order-dotkham-teeth-popover.component';
import { ToaThuocsModule } from '../toa-thuocs/toa-thuocs.module';
import { SaleOrderProductRequestListComponent } from './sale-order-product-request-list/sale-order-product-request-list.component';
import { SaleOrderProductRequestDialogComponent } from './sale-order-product-request-dialog/sale-order-product-request-dialog.component';
import { SaleOrderPromotionDialogComponent } from './sale-order-promotion-dialog/sale-order-promotion-dialog.component';
import { SaleOrderPromotionService } from './sale-order-promotion.service';
import { SaleCouponProgramService } from '../sale-coupon-promotion/sale-coupon-program.service';
import { SaleOrderLinePromotionDialogComponent } from './sale-order-line-promotion-dialog/sale-order-line-promotion-dialog.component';
import { PromotionDiscountComponent } from './promotion-discount/promotion-discount.component';
import { SaleOrderApplyCouponComponent } from './sale-order-apply-coupon/sale-order-apply-coupon.component';
import { SaleOrderManagementComponent } from './sale-order-management/sale-order-management.component';
import { SaleOrderLineManagementComponent } from './sale-order-line-management/sale-order-line-management.component';
import { SaleOrderPrintPopupComponent } from './sale-order-print-popup/sale-order-print-popup.component';
import { SaleOrderImagesLibraryPopupComponent } from './sale-order-images-library-popup/sale-order-images-library-popup.component';
import { SaleOrderImageComponent } from './sale-order-image/sale-order-image.component';
import { SaleOrderServiceListComponent } from './sale-order-service-list/sale-order-service-list.component';
import { SaleOrderInsurancePaymentDialogComponent } from './sale-order-insurance-payment-dialog/sale-order-insurance-payment-dialog.component';

import { ConfirmPaymentDialogComponent } from './confirm-payment-dialog/confirm-payment-dialog.component';

@NgModule({
  declarations: [
    SaleOrderCreateUpdateComponent,
    SaleOrderListComponent,
    SaleOrderCreateDotKhamDialogComponent,
    SaleOrderApplyCouponDialogComponent,
    SaleOrderApplyServiceCardsDialogComponent,
    SaleOrderCuFormComponent,
    SaleOrderApplyDiscountDefaultDialogComponent,
    SaleOrderLineLaboOrdersDialogComponent,
    LaboOrderCuLineDialogComponent,
    LaboOrderCuDialogComponent,
    SaleOrderPaymentDialogComponent,
    PartnerCustomerToathuocListComponent,
    SaleOrderPaymentListComponent,
    TreatmentProcessServiceListComponent,
    TreatmentProcessServiceDialogComponent,
    SaleOrdersDotkhamCuComponent,
    SaleOrderDotkhamTeethPopoverComponent,
    SaleOrderProductRequestListComponent,
    SaleOrderProductRequestDialogComponent,
    SaleOrderPromotionDialogComponent,
    SaleOrderLinePromotionDialogComponent,
    PromotionDiscountComponent,
    SaleOrderApplyCouponComponent,
    SaleOrderManagementComponent,
    SaleOrderLineManagementComponent,
    SaleOrderPrintPopupComponent,
    SaleOrderImagesLibraryPopupComponent,
    SaleOrderImageComponent,
    SaleOrderServiceListComponent,
    SaleOrderInsurancePaymentDialogComponent,
    ConfirmPaymentDialogComponent,
  ],
  imports: [
    CommonModule,
    SaleOrdersRoutingModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    ToaThuocsModule,
    FormsModule,
    SharedModule,
    NgbModule,
  ],
  providers: [SaleOrderPromotionService, SaleCouponProgramService],
  exports: [
  ],
  entryComponents: [
    SaleOrderCreateDotKhamDialogComponent,
    SaleOrderApplyCouponDialogComponent,
    SaleOrderApplyServiceCardsDialogComponent,
    SaleOrderApplyDiscountDefaultDialogComponent,
    SaleOrderLineLaboOrdersDialogComponent,
    LaboOrderCuLineDialogComponent,
    LaboOrderCuDialogComponent,
    SaleOrderPaymentDialogComponent,
    TreatmentProcessServiceDialogComponent,
    SaleOrdersDotkhamCuComponent,
    SaleOrderProductRequestDialogComponent,
    SaleOrderPromotionDialogComponent,
    SaleOrderLinePromotionDialogComponent,
    SaleOrderPrintPopupComponent,
    SaleOrderImagesLibraryPopupComponent,
    SaleOrderInsurancePaymentDialogComponent
  ]
})
export class SaleOrdersModule { }
