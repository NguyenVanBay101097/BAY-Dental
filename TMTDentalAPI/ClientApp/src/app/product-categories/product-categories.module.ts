import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProductCategoriesRoutingModule } from './product-categories-routing.module';
import { ProductCategoryListComponent } from './product-category-list/product-category-list.component';
import { ProductCategoryDialogComponent } from './product-category-dialog/product-category-dialog.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { GridModule } from '@progress/kendo-angular-grid';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { ProductCategoryService } from './product-category.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ProductCategoryImportExcelDialogComponent } from './product-category-import-excel-dialog/product-category-import-excel-dialog.component';

@NgModule({
  declarations: [ProductCategoryListComponent, ProductCategoryDialogComponent, ProductCategoryImportExcelDialogComponent],
  imports: [
    CommonModule,
    ProductCategoriesRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    SharedModule,
    GridModule,
    ButtonsModule,
    DialogsModule,
    FormsModule
  ],
  providers: [
    ProductCategoryService
  ],
  entryComponents: [ProductCategoryDialogComponent, ProductCategoryImportExcelDialogComponent],
})
export class ProductCategoriesModule { }
