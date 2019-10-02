import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { EmployeeCategoriesRoutingModule } from './employee-categories-routing.module';
import { EmpCategoriesListComponent } from './emp-categories-list/emp-categories-list.component';
import { EmpCategoriesCreateUpdateComponent } from './emp-categories-create-update/emp-categories-create-update.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { EmpCategoryService } from './emp-category.service';

@NgModule({
  declarations: [EmpCategoriesListComponent, EmpCategoriesCreateUpdateComponent],
  imports: [
    CommonModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    EmployeeCategoriesRoutingModule
  ],
  entryComponents: [EmpCategoriesCreateUpdateComponent],
  providers: [
    EmpCategoryService
  ]
})
export class EmployeeCategoriesModule { }
