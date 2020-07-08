import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PartnerListComponent } from './partner-list/partner-list.component';
import { PartnerCustomerListComponent } from './partner-customer-list/partner-customer-list.component';
import { PartnerHistoryComponent } from './partner-history/partner-history.component';
import { PartnerSupplierListComponent } from './partner-supplier-list/partner-supplier-list.component';
import { PartnerCustomerCuDialogComponent } from './partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { PartnerCustomerDetailComponent } from './partner-customer-detail/partner-customer-detail.component';
import { PartnerCustomerProfileComponent } from './partner-customer-profile/partner-customer-profile.component';
import { PartnerCustomerTreatmentPaymentComponent } from './partner-customer-treatment-payment/partner-customer-treatment-payment.component';
import { PartnerCustomerAppointmentComponent } from './partner-customer-appointment/partner-customer-appointment.component';
import { PartnerCustomerProductToaThuocListComponent } from './partner-customer-product-toa-thuoc-list/partner-customer-product-toa-thuoc-list.component';

const routes: Routes = [
  {
    path: 'partners', component: PartnerListComponent
  },
  {
    path: 'customers', component: PartnerCustomerListComponent
  },
  {
    path: 'suppliers', component: PartnerSupplierListComponent
  },
  {
    path: 'customer/:id',
    component: PartnerCustomerDetailComponent,
    children: [
      { path: '', redirectTo: 'profile', pathMatch: 'full' },
      { path: 'profile', component: PartnerCustomerProfileComponent },
      { path: 'treatment-payment', component: PartnerCustomerTreatmentPaymentComponent },
      { path: 'appointment', component: PartnerCustomerAppointmentComponent }, 
      { path: 'prescription', component: PartnerCustomerProductToaThuocListComponent }, 
    ]
  },
  {
    path: 'partners/history/:id',
    component: PartnerHistoryComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PartnersRoutingModule { }
