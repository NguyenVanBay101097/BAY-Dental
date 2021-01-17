import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ProductListComponent } from './product-list/product-list.component';
import { ProductServiceListComponent } from './product-service-list/product-service-list.component';
import { ProductMedicineListComponent } from './product-medicine-list/product-medicine-list.component';
import { ProductLaboListComponent } from './product-labo-list/product-labo-list.component';
import { ProductProductListComponent } from './product-product-list/product-product-list.component';
import { ProductLaboAttachListComponent } from './product-labo-attach-list/product-labo-attach-list.component';
import { ProductManagementComponent } from './product-management/product-management.component';
import { ProductManagementServicesComponent } from './product-management-services/product-management-services.component';
import { ProductManagementProductsComponent } from './product-management-products/product-management-products.component';
import { ProductManagementMedicinesComponent } from './product-management-medicines/product-management-medicines.component';

const routes: Routes = [
  // {
  //   path: 'services',
  //   component: ProductServiceListComponent
  // },
  // {
  //   path: 'medicines',
  //   component: ProductMedicineListComponent
  // },
  // {
  //   path: 'products',
  //   component: ProductProductListComponent
  // },
  {
    path: 'labos',
    component: ProductLaboListComponent
  },
  {
    path: 'labo-attachs',
    component: ProductLaboAttachListComponent
  },
  {
    path: '',
    component: ProductManagementComponent,
    children: [
      { path: '', redirectTo: 'services', pathMatch: 'full' },
      { path: 'services', component: ProductManagementServicesComponent },
      { path: 'products', component: ProductManagementProductsComponent },
      { path: 'medicines', component: ProductManagementMedicinesComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProductsRoutingModule { }
