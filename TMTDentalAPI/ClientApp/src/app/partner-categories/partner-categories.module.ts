import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PartnerCategoriesRoutingModule } from './partner-categories-routing.module';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { PartnerCategoryService } from './partner-category.service';
import { PartnerCategoryCuDialogComponent } from './partner-category-cu-dialog/partner-category-cu-dialog.component';
import { PartnerCategoryListComponent } from './partner-category-list/partner-category-list.component';


@NgModule({
  declarations: [PartnerCategoryCuDialogComponent, PartnerCategoryListComponent],
  imports: [
    CommonModule,
    PartnerCategoriesRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule
  ],
  providers: [
    PartnerCategoryService
  ],
  entryComponents: [
    PartnerCategoryCuDialogComponent
  ]
})
export class PartnerCategoriesModule { }
