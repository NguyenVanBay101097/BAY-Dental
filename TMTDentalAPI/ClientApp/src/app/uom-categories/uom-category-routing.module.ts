import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UomCategoryListComponent } from './uom-category-list/uom-category-list.component';

const routes: Routes = [
  {
    path: '', component: UomCategoryListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UomCategoryRoutingModule { }
