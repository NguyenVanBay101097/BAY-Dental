import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProductCategoriesRoutingModule } from './product-categories-routing.module';
import { ProductCategoryListComponent } from './product-category-list/product-category-list.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ProductCategoryService } from './product-category.service';
import { ProductCategoryImportExcelDialogComponent } from './product-category-import-excel-dialog/product-category-import-excel-dialog.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';

@NgModule({
  declarations: [ProductCategoryListComponent, ProductCategoryImportExcelDialogComponent],
  imports: [
    CommonModule,
    ProductCategoriesRoutingModule,
    ReactiveFormsModule,
    SharedModule,
    FormsModule,
    MyCustomKendoModule
  ],
  providers: [
    ProductCategoryService
  ],
  entryComponents: [ProductCategoryImportExcelDialogComponent],
})
export class ProductCategoriesModule { }
