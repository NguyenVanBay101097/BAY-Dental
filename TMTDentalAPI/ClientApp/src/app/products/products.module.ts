import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProductsRoutingModule } from './products-routing.module';
import { ProductListComponent } from './product-list/product-list.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ProductService } from './product.service';
import { ProductDialogComponent } from './product-dialog/product-dialog.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ProductSearchListComponent } from './product-search-list/product-search-list.component';
import { ProductGridComponent } from './product-grid/product-grid.component';
import { ProductServiceListComponent } from './product-service-list/product-service-list.component';
import { ProductServiceFormComponent } from './product-service-form/product-service-form.component';
import { ProductAdvanceFilterComponent } from './product-advance-filter/product-advance-filter.component';
import { ProductMedicineListComponent } from './product-medicine-list/product-medicine-list.component';
import { ProductMedicineCuDialogComponent } from './product-medicine-cu-dialog/product-medicine-cu-dialog.component';
import { ProductMedicineFormComponent } from './product-medicine-form/product-medicine-form.component';
import { ProductLaboFormComponent } from './product-labo-form/product-labo-form.component';
import { ProductProductListComponent } from './product-product-list/product-product-list.component';
import { ProductProductCuDialogComponent } from './product-product-cu-dialog/product-product-cu-dialog.component';
import { ProductProductFormComponent } from './product-product-form/product-product-form.component';
import { ProductStepCuDialogComponent } from './product-step-cu-dialog/product-step-cu-dialog.component';
import { ProductStepFormComponent } from './product-step-form/product-step-form.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { ProductAdvanceSearchComponent } from './product-advance-search/product-advance-search.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ProductServiceImportDialogComponent } from './product-service-import-dialog/product-service-import-dialog.component';
import { SharedModule } from '../shared/shared.module';
import { DialogContainerService, DialogService, WindowContainerService, WindowService } from '@progress/kendo-angular-dialog';
import { ProductManagementServicesComponent } from './product-management-services/product-management-services.component';
import { ProductManagementProductsComponent } from './product-management-products/product-management-products.component';
import { ProductManagementMedicinesComponent } from './product-management-medicines/product-management-medicines.component';
import { ProductManagementComponent } from './product-management/product-management.component';
import { ProductCategoryListComponent } from './product-category-list/product-category-list.component';
import { ResConfigSettingsModule } from '../res-config-settings/res-config-settings.module';
import { ProductImportExcelDialogComponent } from './product-import-excel-dialog/product-import-excel-dialog.component';

@NgModule({
  declarations: [
    ProductImportExcelDialogComponent,
    ProductListComponent,
    ProductDialogComponent,
    ProductSearchListComponent,
    ProductGridComponent,
    ProductServiceListComponent,
    ProductServiceFormComponent,
    ProductAdvanceFilterComponent,
    ProductMedicineListComponent,
    ProductMedicineCuDialogComponent,
    ProductMedicineFormComponent,
    ProductLaboFormComponent,
    ProductProductListComponent,
    ProductProductCuDialogComponent,
    ProductProductFormComponent,
    ProductStepCuDialogComponent,
    ProductStepFormComponent,
    ProductAdvanceSearchComponent,
    ProductServiceImportDialogComponent,
    ProductManagementServicesComponent,
    ProductManagementProductsComponent,
    ProductManagementMedicinesComponent,
    ProductManagementComponent,
    ProductCategoryListComponent,
  ],
  imports: [
    CommonModule,
    ProductsRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    DragDropModule,
    ResConfigSettingsModule,
    SharedModule,
    NgbModule
  ],
  exports: [
    ProductSearchListComponent
  ],
  providers: [
    ProductService, WindowContainerService, DialogContainerService
  ],
  entryComponents: [ProductDialogComponent, ProductMedicineCuDialogComponent,
    ProductProductCuDialogComponent, ProductStepCuDialogComponent,
    ProductServiceImportDialogComponent
  ],
})
export class ProductsModule { }
