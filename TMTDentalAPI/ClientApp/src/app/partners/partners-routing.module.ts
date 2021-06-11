import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PartnerListComponent } from './partner-list/partner-list.component';
import { PartnerCustomerListComponent } from './partner-customer-list/partner-customer-list.component';
import { PartnerSupplierListComponent } from './partner-supplier-list/partner-supplier-list.component';
import { PartnerCustomerDetailComponent } from './partner-customer-detail/partner-customer-detail.component';
import { PartnerCustomerProfileComponent } from './partner-customer-profile/partner-customer-profile.component';
import { PartnerCustomerTreatmentPaymentComponent } from './partner-customer-treatment-payment/partner-customer-treatment-payment.component';
import { PartnerCustomerAppointmentComponent } from './partner-customer-appointment/partner-customer-appointment.component';
import { PartnerCustomerProductToaThuocListComponent } from './partner-customer-product-toa-thuoc-list/partner-customer-product-toa-thuoc-list.component';
import { PartnerCustomerCategoriesComponent } from './partner-customer-categories/partner-customer-categories.component';
import { PartnerCustomerUploadImageComponent } from './partner-customer-upload-image/partner-customer-upload-image.component';
import { PartnerCustomerQuotationsComponent } from './partner-customer-quotations/partner-customer-quotations.component';
import { PartnerCustomerTreatmentHistoryComponent } from './partner-customer-treatment-history/partner-customer-treatment-history.component';
import { PartnerCustomerTreatmentHistoryFormComponent } from './partner-customer-treatment-history-form/partner-customer-treatment-history-form.component';
import { PartnerCustomerTreatmentHistorySaleOrderComponent } from './partner-customer-treatment-history-sale-order/partner-customer-treatment-history-sale-order.component';
import { PartnerCustomerTreatmentPaymentFastComponent } from './partner-customer-treatment-payment-fast/partner-customer-treatment-payment-fast.component';
import { PartnerOverviewComponent } from './partner-overview/partner-overview/partner-overview.component';
import { PartnerInfoCustomerManagementComponent } from './partner-info-customer-management/partner-info-customer-management.component';
import { PartnerCategoryListComponent } from '../partner-categories/partner-category-list/partner-category-list.component';
import { PartnerSourceListComponent } from '../partner-sources/partner-source-list/partner-source-list.component';
import { PartnerTitleListComponent } from '../partner-titles/partner-title-list/partner-title-list.component';
import { HistoriesListComponent } from '../history/histories-list/histories-list.component';
import { PartnerSupplierFormComponent } from './partner-supplier-form/partner-supplier-form.component';
import { PartnerSupplierFormInforComponent } from './partner-supplier-form-infor/partner-supplier-form-infor.component';
import { PartnerSupplierFormDebitComponent } from './partner-supplier-form-debit/partner-supplier-form-debit.component';
import { PartnerSupplierFormPaymentComponent } from './partner-supplier-form-payment/partner-supplier-form-payment.component';
import { PartnerCustomerLaboOrdersComponentComponent } from './partner-customer-labo-orders-component/partner-customer-labo-orders-component.component';
import { PartnerCustomerTreatmentListComponent } from './partner-customer-treatment/partner-customer-treatment-list/partner-customer-treatment-list.component';
import { PartnerCustomerQuotationListComponent } from './partner-customer-quotation/partner-customer-quotation-list/partner-customer-quotation-list.component';
import { PartnerCustomerAdvisoryListComponent } from './partner-customer-advisory/partner-customer-advisory-list/partner-customer-advisory-list.component';
import { PartnerAdvanceListComponent } from '../partner-advances/partner-advance-list/partner-advance-list.component';
import { AuthGuard } from '../auth/auth.guard';
import { PartnerCustomerDebtListComponent } from './partner-customer-debt-list/partner-customer-debt-list.component';
import { PartnerCustomerDebtPaymentHistoryListComponent } from './partner-customer-debt-payment-history-list/partner-customer-debt-payment-history-list.component';
import { PartnerCustomerDebtManagementComponent } from './partner-customer-debt-management/partner-customer-debt-management.component';
import { PartnerAdvanceManagementComponent } from '../partner-advances/partner-advance-management/partner-advance-management.component';
import { PartnerAdvanceHistoryListComponent } from '../partner-advances/partner-advance-history-list/partner-advance-history-list.component';

const routes: Routes = [
  {
    path: 'customers', component: PartnerCustomerListComponent,
    canActivate: [AuthGuard],
    data: {
      permissions: ['Basic.Partner.Read']
    }
  },
  {
    path: 'treatment-paymentfast/from',
    component: PartnerCustomerTreatmentPaymentFastComponent
  },
  {
    path: 'customer/:id',
    component: PartnerCustomerDetailComponent,
    children: [
      { path: '', redirectTo: 'overview', pathMatch: 'full' },
      { path: 'profile', component: PartnerCustomerProfileComponent },
      { path: 'treatment-payment', component: PartnerCustomerTreatmentPaymentComponent },
      { path: 'appointment', component: PartnerCustomerAppointmentComponent },
      { path: 'prescription', component: PartnerCustomerProductToaThuocListComponent },
      { path: 'categories', component: PartnerCustomerCategoriesComponent },
      {
        path: 'advances', component: PartnerAdvanceManagementComponent,
        children: [
          { path: '', redirectTo: 'list', pathMatch: 'full' },
          { path: 'list', component: PartnerAdvanceListComponent },
          { path: 'advance-histories', component: PartnerAdvanceHistoryListComponent },
        ]
      },
      { path: 'partner-images', component: PartnerCustomerUploadImageComponent },
      { path: 'quotations', component: PartnerCustomerQuotationListComponent },
      // { path: 'quotations', component: PartnerCustomerQuotationsComponent },
      { path: 'advisories', component: PartnerCustomerAdvisoryListComponent },
      { path: 'treatment-histories', component: PartnerCustomerTreatmentHistoryComponent },
      { path: 'treatment-histories/form', component: PartnerCustomerTreatmentHistoryFormComponent },
      { path: 'overview', component: PartnerOverviewComponent },
      { path: 'labo-orders', component: PartnerCustomerLaboOrdersComponentComponent },
      { path: 'treatment', component: PartnerCustomerTreatmentListComponent },
      {
        path: 'debt', component: PartnerCustomerDebtManagementComponent,
        children: [
          { path: '', redirectTo: 'list', pathMatch: 'full' },
          { path: 'list', component: PartnerCustomerDebtListComponent },
          { path: 'debt-histories', component: PartnerCustomerDebtPaymentHistoryListComponent },
        ]
      },

    ]
  },
  {
    path: 'customer-management',
    component: PartnerInfoCustomerManagementComponent,
    children: [
      { path: '', redirectTo: 'customer-categ', pathMatch: 'full' },
      { path: 'customer-categ', component: PartnerCategoryListComponent },
      { path: 'customer-source', component: PartnerSourceListComponent },
      { path: 'customer-title', component: PartnerTitleListComponent },
      { path: 'customer-history', component: HistoriesListComponent },
    ]
  },

  {
    path: 'suppliers', component: PartnerSupplierListComponent
  },

  {
    path: 'supplier/:id',
    component: PartnerSupplierFormComponent,
    children: [
      { path: '', redirectTo: 'info', pathMatch: 'full' },
      { path: 'info', component: PartnerSupplierFormInforComponent },
      { path: 'debit', component: PartnerSupplierFormDebitComponent },
      { path: 'payment', component: PartnerSupplierFormPaymentComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PartnersRoutingModule { }
