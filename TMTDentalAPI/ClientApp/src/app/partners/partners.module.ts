import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
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
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PartnerCustomerProfileComponent } from './partner-customer-profile/partner-customer-profile.component';
import { PartnerCustomerProfileInforComponent } from './partner-customer-profile-infor/partner-customer-profile-infor.component';
import { PartnerCustomerProfileNextAppointmentComponent } from './partner-customer-profile-next-appointment/partner-customer-profile-next-appointment.component';
import { PartnerCustomerTreatmentPaymentComponent } from './partner-customer-treatment-payment/partner-customer-treatment-payment.component';
import { PartnerCustomerAppointmentComponent } from './partner-customer-appointment/partner-customer-appointment.component';
import { PartnerCustomerTreatmentPaymentDetailComponent } from './partner-customer-treatment-payment-detail/partner-customer-treatment-payment-detail.component';
import { PartnerCustomerProductToaThuocListComponent } from './partner-customer-product-toa-thuoc-list/partner-customer-product-toa-thuoc-list.component';
import { PartnerCustomerTreatmentPaymentChildComponent } from './partner-customer-treatment-payment-child/partner-customer-treatment-payment-child.component';
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
import { PartnerCustomerTreatmentHistoryFormAddServiceDialogComponent } from './partner-customer-treatment-history-form-add-service-dialog/partner-customer-treatment-history-form-add-service-dialog.component';
import { SaleOrderApplyCouponDialogComponent } from '../sale-orders/sale-order-apply-coupon-dialog/sale-order-apply-coupon-dialog.component';
import { SaleOrderApplyServiceCardsDialogComponent } from '../sale-orders/sale-order-apply-service-cards-dialog/sale-order-apply-service-cards-dialog.component';

import { SaleOrdersModule } from '../sale-orders/sale-orders.module';
import { SaleOrderApplyDiscountDefaultDialogComponent } from '../sale-orders/sale-order-apply-discount-default-dialog/sale-order-apply-discount-default-dialog.component';
import { TreatmentHistoryTeethPopoverComponent } from './partner-customer-treatment-history-form/treatment-history-teeth-popover/treatment-history-teeth-popover.component';
import { ApplyDiscountDefaultPopoverComponent } from './partner-customer-treatment-history/apply-discount-default-popover/apply-discount-default-popover.component';
import { ApplyDiscountSaleOrderLinePopoverComponent } from './partner-customer-treatment-history/apply-discount-sale-order-line-popover/apply-discount-sale-order-line-popover.component';
import { PartnerOverviewComponent } from './partner-overview/partner-overview/partner-overview.component';
import { PartnerOverviewInfoComponent } from './partner-overview/partner-overview-info/partner-overview-info.component';
import { PartnerOverviewAppointmentComponent } from './partner-overview/partner-overview-appointment/partner-overview-appointment.component';
import { PartnerOverviewTreatmentComponent } from './partner-overview/partner-overview-treatment/partner-overview-treatment.component';
import { PartnerOverviewAdvisoryComponent } from './partner-overview/partner-overview-advisory/partner-overview-advisory.component';
import { PartnerOverviewPromotionComponent } from './partner-overview/partner-overview-promotion/partner-overview-promotion.component';
import { PartnerOverviewReportComponent } from './partner-overview/partner-overview-report/partner-overview-report.component';
import { PartnerOverviewImageComponent } from './partner-overview/partner-overview-image/partner-overview-image.component';
import { AccountCommonPartnerReportsModule } from '../account-common-partner-reports/account-common-partner-reports.module';
import { SaleCouponProgramService } from '../sale-coupon-promotion/sale-coupon-program.service';
import { PartnerOverviewSaleOrderLineComponent } from './partner-overview/partner-overview-sale-order-line/partner-overview-sale-order-line.component';
import { PartnerCustomerTreatmentPaymentFastComponent } from './partner-customer-treatment-payment-fast/partner-customer-treatment-payment-fast.component';
import { PartnerInfoCustomerManagementComponent } from './partner-info-customer-management/partner-info-customer-management.component';
import { PartnerTitlesModule } from '../partner-titles/partner-titles.module';
import { PartnerSourcesModule } from '../partner-sources/partner-sources.module';
import { PartnerCategoriesModule } from '../partner-categories/partner-categories.module';
import { HistoryModule } from '../history/history.module';
import { PartnerSupplierFormComponent } from './partner-supplier-form/partner-supplier-form.component';
import { PartnerSupplierFormInforComponent } from './partner-supplier-form-infor/partner-supplier-form-infor.component';
import { PartnerSupplierFormDebitComponent } from './partner-supplier-form-debit/partner-supplier-form-debit.component';
import { PartnerSupplierFormPaymentComponent } from './partner-supplier-form-payment/partner-supplier-form-payment.component';
import { PartnerSupplierFormDebitPaymentDialogComponent } from './partner-supplier-form-debit-payment-dialog/partner-supplier-form-debit-payment-dialog.component';
import { PartnerCustomerLaboOrdersComponentComponent } from './partner-customer-labo-orders-component/partner-customer-labo-orders-component.component';
import { PartnerDotkhamListComponent } from './partner-overview/partner-dotkham-list/partner-dotkham-list.component';
import { PartnerDotkhamDetailComponent } from './partner-overview/partner-dotkham-detail/partner-dotkham-detail.component';
import { PartnerCustomerTreatmentListComponent } from './partner-customer-treatment/partner-customer-treatment-list/partner-customer-treatment-list.component';
import { PartnerCustomerTreatmentSaleOrderLineComponent } from './partner-customer-treatment/partner-customer-treatment-sale-order-line/partner-customer-treatment-sale-order-line.component';
import { PartnerCustomerQuotationListComponent } from './partner-customer-quotation/partner-customer-quotation-list/partner-customer-quotation-list.component';
import { PartnerCustomerQuotationFormComponent } from './partner-customer-quotation/partner-customer-quotation-form/partner-customer-quotation-form.component';
import { PartnerAdvancesModule } from '../partner-advances/partner-advances.module';

import { PartnerCustomerAdvisoryListComponent } from './partner-customer-advisory/partner-customer-advisory-list/partner-customer-advisory-list.component';
import { PartnerCustomerAdvisoryCuDialogComponent } from './partner-customer-advisory/partner-customer-advisory-cu-dialog/partner-customer-advisory-cu-dialog.component';
import { ToothDiagnosisPopoverComponent } from './partner-customer-advisory/partner-customer-advisory-list/tooth-diagnosis-popover/tooth-diagnosis-popover.component';
import { PartnerCustomerSaleOrderQuotationsLinesComponent } from './partner-customer-sale-order-quotations-lines/partner-customer-sale-order-quotations-lines.component';
import { ConsultingServicePopoverComponent } from './partner-customer-advisory/partner-customer-advisory-list/consulting-service-popover/consulting-service-popover.component';
import { PartnerCustomerTreatmentFastPromotionComponent } from './partner-customer-treatment-fast-promotion/partner-customer-treatment-fast-promotion.component';
import { PartnerCustomerTreatmentLineFastPromotionComponent } from './partner-customer-treatment-line-fast-promotion/partner-customer-treatment-line-fast-promotion.component';
import { PartnerCustomerDebtListComponent } from './partner-customer-debt-list/partner-customer-debt-list.component';
import { PartnerCustomerDebtPaymentDialogComponent } from './partner-customer-debt-payment-dialog/partner-customer-debt-payment-dialog.component';
import { PartnerCustomerDebtPaymentHistoryListComponent } from './partner-customer-debt-payment-history-list/partner-customer-debt-payment-history-list.component';
import { PartnerCustomerDebtManagementComponent } from './partner-customer-debt-management/partner-customer-debt-management.component';


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
    PartnerCustomerTreatmentHistorySaleOrderComponent,
    PartnerCustomerTreatmentHistoryFormAddServiceDialogComponent,
    TreatmentHistoryTeethPopoverComponent,
    ApplyDiscountDefaultPopoverComponent,
    ApplyDiscountSaleOrderLinePopoverComponent,
    PartnerOverviewComponent,
    PartnerOverviewInfoComponent,
    PartnerOverviewAppointmentComponent,
    PartnerOverviewTreatmentComponent,
    PartnerOverviewAdvisoryComponent,
    PartnerOverviewPromotionComponent,
    PartnerOverviewReportComponent,
    PartnerOverviewImageComponent,
    PartnerOverviewSaleOrderLineComponent,
    ApplyDiscountSaleOrderLinePopoverComponent,
    PartnerCustomerTreatmentPaymentFastComponent,
    PartnerInfoCustomerManagementComponent,
    PartnerSupplierFormComponent,
    PartnerSupplierFormInforComponent,
    PartnerSupplierFormDebitComponent,
    PartnerSupplierFormPaymentComponent,
    PartnerSupplierFormDebitPaymentDialogComponent,
    PartnerCustomerLaboOrdersComponentComponent,
    PartnerSupplierFormDebitPaymentDialogComponent,
    PartnerDotkhamListComponent,
    PartnerDotkhamDetailComponent,
    PartnerSupplierFormDebitPaymentDialogComponent,
    PartnerCustomerTreatmentListComponent,
    PartnerCustomerTreatmentSaleOrderLineComponent,
    PartnerCustomerQuotationListComponent,
    PartnerCustomerQuotationFormComponent,
    PartnerCustomerTreatmentSaleOrderLineComponent,
    PartnerCustomerAdvisoryListComponent,
    PartnerCustomerAdvisoryCuDialogComponent,
    ToothDiagnosisPopoverComponent,
    PartnerCustomerSaleOrderQuotationsLinesComponent,
    ConsultingServicePopoverComponent,
    PartnerCustomerTreatmentFastPromotionComponent,
    PartnerCustomerTreatmentLineFastPromotionComponent,
    PartnerCustomerDebtListComponent,
    PartnerCustomerDebtPaymentDialogComponent,
    PartnerCustomerDebtPaymentHistoryListComponent,
    PartnerCustomerDebtManagementComponent
  ],
  imports: [
    CommonModule,
    PartnersRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    NgbModule,
    AccountCommonPartnerReportsModule,
    SaleOrdersModule,
    PartnerTitlesModule,
    PartnerSourcesModule,
    PartnerCategoriesModule,
    PartnerAdvancesModule,
    HistoryModule
  ],
  entryComponents: [
    PartnerCreateUpdateComponent,
    PartnerImportComponent,
    PartnerSearchDialogComponent,
    PartnerCustomerTreatmentPaymentDetailComponent,
    PartnerCustomerAutoGenerateCodeDialogComponent,
    SaleOrderApplyCouponDialogComponent,
    SaleOrderApplyServiceCardsDialogComponent,
    PartnerSupplierFormDebitPaymentDialogComponent,
    SaleOrderApplyDiscountDefaultDialogComponent,
    PartnerCustomerAutoGenerateCodeDialogComponent,
    PartnerCustomerTreatmentHistoryFormAddServiceDialogComponent,
    PartnerCustomerAdvisoryCuDialogComponent,
    PartnerCustomerTreatmentFastPromotionComponent,
    PartnerCustomerTreatmentLineFastPromotionComponent,
    PartnerCustomerDebtPaymentDialogComponent
  ],
  providers: [SaleCouponProgramService],
  exports: [
    PartnerCustomerDetailComponent,
    PartnerCustomerTreatmentPaymentDetailComponent
  ], schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class PartnersModule { }
