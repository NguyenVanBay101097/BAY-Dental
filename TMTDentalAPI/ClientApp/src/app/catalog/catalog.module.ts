import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CatalogRoutingModule } from './catalog-routing.module';
import { PartnerCategoriesModule } from '../partner-categories/partner-categories.module';
import { PartnerSourcesModule } from '../partner-sources/partner-sources.module';
import { PartnerTitlesModule } from '../partner-titles/partner-titles.module';
import { HistoryModule } from '../history/history.module';
import { PartnerInfoCustomerManagementComponent } from './partner-info-customer-management/partner-info-customer-management.component';
import { MemberLevelModule } from '../member-level/member-level.module';
import { PartnerSupplierListComponent } from './partner-supplier-list/partner-supplier-list.component';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { LaboManagementComponent } from './labo-management/labo-management.component';
import { PartnerImportComponent } from '../partners/partner-import/partner-import.component';

@NgModule({
  declarations: [
    PartnerInfoCustomerManagementComponent,
    PartnerSupplierListComponent,
    LaboManagementComponent,
  ],
  imports: [
    CommonModule,
    CatalogRoutingModule,
    PartnerCategoriesModule,
    PartnerSourcesModule,
    PartnerTitlesModule,
    HistoryModule,
    MemberLevelModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    MyCustomKendoModule
  ],
  entryComponents:[]
})
export class CatalogModule { }
