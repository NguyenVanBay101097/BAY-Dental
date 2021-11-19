import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CatalogRoutingModule } from './catalog-routing.module';
import { PartnerCategoriesModule } from '../partner-categories/partner-categories.module';
import { PartnerSourcesModule } from '../partner-sources/partner-sources.module';
import { PartnerTitlesModule } from '../partner-titles/partner-titles.module';
import { HistoryModule } from '../history/history.module';
import { MemberLevelModule } from '../member-level/member-level.module';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { PartnerImportComponent } from '../partners/partner-import/partner-import.component';
import { ProductImportExcelDialogComponent } from '../products/product-import-excel-dialog/product-import-excel-dialog.component';

@NgModule({
  declarations: [
    PartnerImportComponent,
    ProductImportExcelDialogComponent,
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
    ProductImportExcelDialogComponent,
  ]
})
export class CatalogModule { }
