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
import { ProductLaboListComponent } from '../products/product-labo-list/product-labo-list.component';
import { ProductLaboAttachListComponent } from '../products/product-labo-attach-list/product-labo-attach-list.component';
import { ProductLaboCuDialogComponent } from '../products/product-labo-cu-dialog/product-labo-cu-dialog.component';
import { ProductImportExcelDialogComponent } from '../products/product-import-excel-dialog/product-import-excel-dialog.component';
import { ProductLaboAttachCuDialogComponent } from '../products/product-labo-attach-cu-dialog/product-labo-attach-cu-dialog.component';

@NgModule({
  declarations: [
    PartnerInfoCustomerManagementComponent,
    PartnerSupplierListComponent,
    LaboManagementComponent,
    PartnerImportComponent,
    ProductLaboListComponent,
    ProductLaboAttachListComponent,
    ProductLaboCuDialogComponent,
    ProductImportExcelDialogComponent,
    ProductLaboAttachCuDialogComponent
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
    MyCustomKendoModule,
  ],
  exports: [PartnerImportComponent],
  entryComponents: [PartnerImportComponent,
    ProductLaboCuDialogComponent,
    ProductImportExcelDialogComponent,
    ProductLaboAttachCuDialogComponent
  ]
})
export class CatalogModule { }
