import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PartnerListComponent } from './partner-list/partner-list.component';
import { PartnerCustomerListComponent } from './partner-customer-list/partner-customer-list.component';
import { PartnerHistoryComponent } from './partner-history/partner-history.component';
import { PartnerSupplierListComponent } from './partner-supplier-list/partner-supplier-list.component';

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
    path: 'partners/history/:id',
    component: PartnerHistoryComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PartnersRoutingModule { }
