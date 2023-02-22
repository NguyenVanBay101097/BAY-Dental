import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';

import { EmployeeCategoriesRoutingModule } from './employee-categories-routing.module';
import { EmpCategoriesListComponent } from './emp-categories-list/emp-categories-list.component';
import { EmpCategoriesCreateUpdateComponent } from './emp-categories-create-update/emp-categories-create-update.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { EmpCategoryService } from './emp-category.service';
import { MyCustomNgbModule } from '../shared/my-custom-ngb.module';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { SharedModule } from '@progress/kendo-angular-grid';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [EmpCategoriesListComponent, EmpCategoriesCreateUpdateComponent],
  imports: [
    CommonModule,
    NgbModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    DragDropModule,
    MyCustomNgbModule,
    EmployeeCategoriesRoutingModule
  ], schemas: [NO_ERRORS_SCHEMA],
  entryComponents: [EmpCategoriesCreateUpdateComponent],
  providers: [
    EmpCategoryService
  ]
})
export class EmployeeCategoriesModule { }
