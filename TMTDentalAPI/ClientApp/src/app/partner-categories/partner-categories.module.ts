import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PartnerCategoriesRoutingModule } from './partner-categories-routing.module';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { PartnerCategoryService } from './partner-category.service';
import { PartnerCategoryCuDialogComponent } from './partner-category-cu-dialog/partner-category-cu-dialog.component';
import { PartnerCategoryListComponent } from './partner-category-list/partner-category-list.component';
import { PartnerCategoryImportComponent } from './partner-category-import/partner-category-import.component';
import { SharedModule } from '../shared/shared.module';


@NgModule({
  declarations: [PartnerCategoryCuDialogComponent, PartnerCategoryListComponent, PartnerCategoryImportComponent],
  imports: [
    CommonModule,
    PartnerCategoriesRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
  ],
  providers: [
    PartnerCategoryService
  ],
  entryComponents: [
    PartnerCategoryCuDialogComponent,
    PartnerCategoryImportComponent
  ]
})
export class PartnerCategoriesModule { }
