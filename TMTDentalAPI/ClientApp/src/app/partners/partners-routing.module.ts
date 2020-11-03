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

const routes: Routes = [
  {
    path: 'customers', component: PartnerCustomerListComponent
  },
  {
    path: 'suppliers', component: PartnerSupplierListComponent
  },
  {
    path:'treatment-paymentfast',
    component: PartnerCustomerTreatmentPaymentFastComponent
  },
  {
    path: 'customer/:id',
    component: PartnerCustomerDetailComponent,
    children: [
      { path: '', redirectTo: 'treatment-payment', pathMatch: 'full' },
      { path: 'profile', component: PartnerCustomerProfileComponent },
      { path: 'treatment-payment', component: PartnerCustomerTreatmentPaymentComponent },
      { path: 'appointment', component: PartnerCustomerAppointmentComponent },
      { path: 'prescription', component: PartnerCustomerProductToaThuocListComponent },
      { path: 'categories', component: PartnerCustomerCategoriesComponent },
      { path: 'partner-images', component: PartnerCustomerUploadImageComponent },
      { path: 'quotations', component: PartnerCustomerQuotationsComponent },
      { path: 'treatment-histories', component: PartnerCustomerTreatmentHistoryComponent },
      {
        path: 'treatment-histories/form',
        component: PartnerCustomerTreatmentHistoryFormComponent
      }
      
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PartnersRoutingModule { }
