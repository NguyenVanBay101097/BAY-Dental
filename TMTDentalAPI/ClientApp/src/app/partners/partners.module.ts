import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PartnersRoutingModule } from './partners-routing.module';
import { PartnerListComponent } from './partner-list/partner-list.component';
import { PartnerCreateUpdateComponent } from './partner-create-update/partner-create-update.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { PartnerCustomerListComponent } from './partner-customer-list/partner-customer-list.component';
import { PartnerCustomerListDetailComponent } from './partner-customer-list-detail/partner-customer-list-detail.component';
import { PartnerCustomerInfoComponent } from './partner-customer-info/partner-customer-info.component';
import { PartnerCustomerInvoicesComponent } from './partner-customer-invoices/partner-customer-invoices.component';
import { PartnerDetailListComponent } from './partner-detail-list/partner-detail-list.component';
import { PartnerSupplierListComponent } from './partner-supplier-list/partner-supplier-list.component';
import { PartnerInvoiceLinesComponent } from './partner-invoice-lines/partner-invoice-lines.component';
import { PartnerImportComponent } from './partner-import/partner-import.component';
import { PartnerPaymentsComponent } from './partner-payments/partner-payments.component';
import { PurchaseOrderRefundComponent } from './purchase-order-refund/purchase-order-refund.component';
import { PartnerCardsTabPaneComponent } from './partner-cards-tab-pane/partner-cards-tab-pane.component';
import { SharedModule } from '../shared/shared.module';
import { PartnerTabSaleOrderListComponent } from './partner-tab-sale-order-list/partner-tab-sale-order-list.component';
import { PartnerSearchDialogComponent } from './partner-search-dialog/partner-search-dialog.component';
import { PartnerCustomerDetailComponent } from './partner-customer-detail/partner-customer-detail.component';
import { HttpClientModule } from '@angular/common/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PartnerCustomerProfileComponent } from './partner-customer-profile/partner-customer-profile.component';
import { PartnerCustomerProfileInforComponent } from './partner-customer-profile-infor/partner-customer-profile-infor.component';
import { PartnerCustomerProfileNextAppointmentComponent } from './partner-customer-profile-next-appointment/partner-customer-profile-next-appointment.component';
import { PartnerCustomerTreatmentPaymentComponent } from './partner-customer-treatment-payment/partner-customer-treatment-payment.component';
import { PartnerCustomerAppointmentComponent } from './partner-customer-appointment/partner-customer-appointment.component';
import { PartnerCustomerTreatmentPaymentDetailComponent } from './partner-customer-treatment-payment-detail/partner-customer-treatment-payment-detail.component';
import { PartnerCustomerProductToaThuocListComponent } from './partner-customer-product-toa-thuoc-list/partner-customer-product-toa-thuoc-list.component';
import { PartnerCustomerTreatmentPaymentChildComponent } from './partner-customer-treatment-payment-child/partner-customer-treatment-payment-child.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { PartnerCustomerCategoriesComponent } from './partner-customer-categories/partner-customer-categories.component';
import { PartnerCustomerUploadImageComponent } from './partner-customer-upload-image/partner-customer-upload-image.component';
import { PartnerCustomerQuotationsComponent } from './partner-customer-quotations/partner-customer-quotations.component';
import { PartnerInfoComponent } from './partner-info/partner-info.component';
import { PartnerCategoryPopoverComponent } from './partner-customer-list/partner-category-popover/partner-category-popover.component';
import { PartnerCustomerAutoGenerateCodeDialogComponent } from './partner-customer-auto-generate-code-dialog/partner-customer-auto-generate-code-dialog.component';
import { PartnerCustomerTreatmentHistoryComponent } from './partner-customer-treatment-history/partner-customer-treatment-history.component';
import { PartnerCustomerTreatmentHistoryFormComponent } from './partner-customer-treatment-history-form/partner-customer-treatment-history-form.component';
import { PartnerCustomerTreatmentHistoryFormPaymentComponent } from './partner-customer-treatment-history-form-payment/partner-customer-treatment-history-form-payment.component';
import { PartnerCustomerTreatmentHistoryFormServiceListComponent } from './partner-customer-treatment-history-form-service-list/partner-customer-treatment-history-form-service-list.component';
import { PartnerCustomerTreatmentHistoryFormServiceSearchComponent } from './partner-customer-treatment-history-form-service-search/partner-customer-treatment-history-form-service-search.component';
import { PartnerCustomerTreatmentHistorySaleOrderComponent } from './partner-customer-treatment-history-sale-order/partner-customer-treatment-history-sale-order.component';
import { SaleOrderApplyCouponDialogComponent } from '../sale-orders/sale-order-apply-coupon-dialog/sale-order-apply-coupon-dialog.component';
import { SaleOrderApplyServiceCardsDialogComponent } from '../sale-orders/sale-order-apply-service-cards-dialog/sale-order-apply-service-cards-dialog.component';
import { SaleOrderApplyDiscountDefaultDialogComponent } from '../sale-orders/sale-order-apply-discount-default-dialog/sale-order-apply-discount-default-dialog.component';
import { SaleOrdersModule } from '../sale-orders/sale-orders.module';

@NgModule({
  declarations: [
    PartnerListComponent,
    PartnerCreateUpdateComponent,
    PartnerCustomerListComponent,
    PartnerCustomerListDetailComponent,
    PartnerCustomerInfoComponent,
    PartnerCustomerInvoicesComponent,
    PartnerDetailListComponent,
    PartnerSupplierListComponent,
    PartnerInvoiceLinesComponent,
    PartnerImportComponent,
    PartnerPaymentsComponent,
    PurchaseOrderRefundComponent,
    PartnerCardsTabPaneComponent,
    PartnerTabSaleOrderListComponent,
    PartnerSearchDialogComponent,
    PartnerCustomerDetailComponent,
    PartnerCustomerProfileComponent,
    PartnerCustomerProfileInforComponent,
    PartnerCustomerProfileNextAppointmentComponent,
    PartnerCustomerTreatmentPaymentComponent,
    PartnerCustomerAppointmentComponent,
    PartnerCustomerTreatmentPaymentDetailComponent,
    PartnerCustomerTreatmentPaymentChildComponent,
    PartnerCustomerProductToaThuocListComponent,
    PartnerCustomerCategoriesComponent,
    PartnerCustomerUploadImageComponent,
    PartnerCustomerQuotationsComponent,
    PartnerInfoComponent,
    PartnerCategoryPopoverComponent,
    PartnerCustomerAutoGenerateCodeDialogComponent,
    PartnerCustomerTreatmentHistoryComponent,
    PartnerCustomerTreatmentHistoryFormComponent,
    PartnerCustomerTreatmentHistoryFormPaymentComponent,
    PartnerCustomerTreatmentHistoryFormServiceListComponent,
    PartnerCustomerTreatmentHistoryFormServiceSearchComponent,
    PartnerCustomerTreatmentHistorySaleOrderComponent,
  ],
  imports: [
    CommonModule,
    PartnersRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    HttpClientModule,
    FormsModule,
    SharedModule,
    FlexLayoutModule,
    NgbModule,
    SaleOrdersModule
  ],
  entryComponents: [
    PartnerCreateUpdateComponent,
    PartnerImportComponent,
    PartnerSearchDialogComponent,
    PartnerCustomerTreatmentPaymentDetailComponent,
    PartnerCustomerAutoGenerateCodeDialogComponent,
    SaleOrderApplyCouponDialogComponent,
    SaleOrderApplyServiceCardsDialogComponent,
    SaleOrderApplyDiscountDefaultDialogComponent,
  ],
  providers: [],
  exports: [
    PartnerCustomerDetailComponent,
    PartnerCustomerTreatmentPaymentDetailComponent
  ]
})
export class PartnersModule { }
