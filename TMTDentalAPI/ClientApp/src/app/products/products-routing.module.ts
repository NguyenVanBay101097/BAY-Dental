import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ProductManagementComponent } from './product-management/product-management.component';
import { ProductManagementServicesComponent } from './product-management-services/product-management-services.component';
import { ProductManagementProductsComponent } from './product-management-products/product-management-products.component';
import { ProductManagementMedicinesComponent } from './product-management-medicines/product-management-medicines.component';

const routes: Routes = [
  {
    path: '',
    component: ProductManagementComponent,
    children: [
      { path: '', redirectTo: 'services', pathMatch: 'full' },
      { path: 'services', component: ProductManagementServicesComponent },
      { path: 'products', component: ProductManagementProductsComponent },
      { path: 'medicines', component: ProductManagementMedicinesComponent },
      { path: 'services', component: ProductManagementServicesComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProductsRoutingModule { }
