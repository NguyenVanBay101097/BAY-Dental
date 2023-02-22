import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CustomerManagementRoutingModule } from './customer-management-routing.module';
import { PartnerCategoriesModule } from '../partner-categories/partner-categories.module';
import { PartnerSourcesModule } from '../partner-sources/partner-sources.module';
import { HistoryModule } from '../history/history.module';
import { PartnerTitlesModule } from '../partner-titles/partner-titles.module';
import { PartnerInfoCustomerManagementComponent } from '../catalog/partner-info-customer-management/partner-info-customer-management.component';


@NgModule({
  declarations: [
    PartnerInfoCustomerManagementComponent
  ],
  imports: [
    CommonModule,
    CustomerManagementRoutingModule,
    PartnerCategoriesModule,
    PartnerSourcesModule,
    HistoryModule,
    PartnerTitlesModule
  ]
})
export class CustomerManagementModule { }
