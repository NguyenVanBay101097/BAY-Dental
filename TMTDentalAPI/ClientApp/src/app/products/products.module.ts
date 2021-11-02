import { DragDropModule } from '@angular/cdk/drag-drop';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { DialogContainerService, WindowContainerService } from '@progress/kendo-angular-dialog';
import { ResConfigSettingsModule } from '../res-config-settings/res-config-settings.module';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { ProductAdvanceFilterComponent } from './product-advance-filter/product-advance-filter.component';
import { ProductAdvanceSearchComponent } from './product-advance-search/product-advance-search.component';
import { ProductCategoryListComponent } from './product-category-list/product-category-list.component';
import { ProductDialogComponent } from './product-dialog/product-dialog.component';
import { ProductGridComponent } from './product-grid/product-grid.component';
import { ProductImportExcelDialogComponent } from './product-import-excel-dialog/product-import-excel-dialog.component';
import { ProductLaboFormComponent } from './product-labo-form/product-labo-form.component';
import { ProductListComponent } from './product-list/product-list.component';
import { ProductManagementMedicinesComponent } from './product-management-medicines/product-management-medicines.component';
import { ProductManagementProductsComponent } from './product-management-products/product-management-products.component';
import { ProductManagementServicesComponent } from './product-management-services/product-management-services.component';
import { ProductManagementComponent } from './product-management/product-management.component';
import { ProductMedicineCuDialogComponent } from './product-medicine-cu-dialog/product-medicine-cu-dialog.component';
import { ProductMedicineFormComponent } from './product-medicine-form/product-medicine-form.component';
import { ProductMedicineListComponent } from './product-medicine-list/product-medicine-list.component';
import { ProductProductCuDialogComponent } from './product-product-cu-dialog/product-product-cu-dialog.component';
import { ProductProductFormComponent } from './product-product-form/product-product-form.component';
import { ProductProductListComponent } from './product-product-list/product-product-list.component';
import { ProductSearchListComponent } from './product-search-list/product-search-list.component';
import { ProductServiceFormComponent } from './product-service-form/product-service-form.component';
import { ProductServiceImportDialogComponent } from './product-service-import-dialog/product-service-import-dialog.component';
import { ProductServiceListComponent } from './product-service-list/product-service-list.component';
import { ProductStepCuDialogComponent } from './product-step-cu-dialog/product-step-cu-dialog.component';
import { ProductStepFormComponent } from './product-step-form/product-step-form.component';
import { ProductService } from './product.service';
import { ProductsRoutingModule } from './products-routing.module';


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
