import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UomCategoryListComponent } from './uom-category-list/uom-category-list.component';

const routes: Routes = [
  {
    path: 'uom-category', component: UomCategoryListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UomCategoryRoutingModule { }
