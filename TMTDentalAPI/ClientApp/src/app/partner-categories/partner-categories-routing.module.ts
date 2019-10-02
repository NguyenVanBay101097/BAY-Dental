import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PartnerCategoryListComponent } from './partner-category-list/partner-category-list.component';

const routes: Routes = [
  {
    path: 'partner-categories',
    component: PartnerCategoryListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PartnerCategoriesRoutingModule { }
