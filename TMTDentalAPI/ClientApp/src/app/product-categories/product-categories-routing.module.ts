import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ProductCategoryListComponent } from './product-category-list/product-category-list.component';

const routes: Routes = [
  {
    path: 'product-categories',
    component: ProductCategoryListComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProductCategoriesRoutingModule { }
