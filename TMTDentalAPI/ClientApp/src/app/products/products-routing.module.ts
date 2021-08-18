import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ProductLaboListComponent } from './product-labo-list/product-labo-list.component';
import { ProductManagementComponent } from './product-management/product-management.component';
import { ProductManagementServicesComponent } from './product-management-services/product-management-services.component';
import { ProductManagementProductsComponent } from './product-management-products/product-management-products.component';
import { ProductManagementMedicinesComponent } from './product-management-medicines/product-management-medicines.component';
import { ProductLaboAttachListComponent } from './product-labo-attach-list/product-labo-attach-list.component';

const routes: Routes = [
  {
    path: '',
    component: ProductManagementComponent,
    children: [
      { path: '', redirectTo: 'services', pathMatch: 'full' },
      { path: 'services', component: ProductManagementServicesComponent },
      { path: 'products', component: ProductManagementProductsComponent },
      { path: 'medicines', component: ProductManagementMedicinesComponent },
      { path: 'services', component: ProductManagementServicesComponent },
      { path: 'product-labos', component: ProductLaboListComponent },
      { path: 'labo-attachs', component: ProductLaboAttachListComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProductsRoutingModule { }
