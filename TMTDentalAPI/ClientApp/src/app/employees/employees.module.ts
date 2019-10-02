import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { EmployeeListComponent } from './employee-list/employee-list.component';
import { EmployeesRoutingModule } from './employees-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { EmployeeCreateUpdateComponent } from './employee-create-update/employee-create-update.component';
import { EmployeeService } from './employee.service';
import { EmpCategoriesCreateUpdateComponent } from './emp-categories-create-update/emp-categories-create-update.component';

@NgModule({
  declarations: [EmployeeListComponent, EmployeeCreateUpdateComponent, EmpCategoriesCreateUpdateComponent],
  imports: [
    CommonModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    EmployeesRoutingModule
  ],
  entryComponents: [EmployeeCreateUpdateComponent],
  providers: [
    EmployeeService
  ]
})
export class EmployeesModule { }
