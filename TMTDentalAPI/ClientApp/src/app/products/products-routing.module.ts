import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ProductListComponent } from './product-list/product-list.component';
import { ProductServiceListComponent } from './product-service-list/product-service-list.component';
import { ProductMedicineListComponent } from './product-medicine-list/product-medicine-list.component';
import { ProductLaboListComponent } from './product-labo-list/product-labo-list.component';
import { ProductProductListComponent } from './product-product-list/product-product-list.component';

const routes: Routes = [
  {
    path: 'services',
    component: ProductServiceListComponent
  },
  {
    path: 'medicines',
    component: ProductMedicineListComponent
  },
  {
    path: 'products',
    component: ProductProductListComponent
  },
  {
    path: 'labos',
    component: ProductLaboListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProductsRoutingModule { }
