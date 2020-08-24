import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { EmpCategoriesListComponent } from './emp-categories-list/emp-categories-list.component';


const routes: Routes = [
  {
    path: '', component: EmpCategoriesListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})

export class EmployeeCategoriesRoutingModule { }
