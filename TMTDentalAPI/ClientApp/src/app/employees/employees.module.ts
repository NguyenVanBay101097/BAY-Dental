import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';

import { EmployeesRoutingModule } from './employees-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { EmployeeCreateUpdateComponent } from './employee-create-update/employee-create-update.component';
import { EmployeeService } from './employee.service';
import { EmployeeInfoComponent } from './employee-info/employee-info.component';
import { EmployeeAdvanceSearchComponent } from './employee-advance-search/employee-advance-search.component';
import { MyCustomNgbModule } from '../shared/my-custom-ngb.module';
import { EmployeeListComponent } from './employee-list/employee-list.component';
import { EmpCategoryService } from '../employee-categories/emp-category.service';
import { RoleService } from '../roles/role.service';


@NgModule({
  declarations: [EmployeeListComponent, EmployeeCreateUpdateComponent, EmployeeInfoComponent, EmployeeAdvanceSearchComponent],
  imports: [
    CommonModule,
    EmployeesRoutingModule,
    NgbModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    MyCustomNgbModule,
    DragDropModule,
  ],
   providers: [
    EmployeeService,
    EmpCategoryService,
    RoleService
  ],
  schemas: [NO_ERRORS_SCHEMA],
  entryComponents: [EmployeeCreateUpdateComponent],
 
})
export class EmployeesModule { }
