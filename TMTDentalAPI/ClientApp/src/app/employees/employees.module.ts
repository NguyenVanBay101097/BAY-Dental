import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { EmployeeListComponent } from './employee-list/employee-list.component';
import { EmployeesRoutingModule } from './employees-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { EmployeeCreateUpdateComponent } from './employee-create-update/employee-create-update.component';
import { EmployeeService } from './employee.service';
import { EmployeeInfoComponent } from './employee-info/employee-info.component';
import { EmployeeAdvanceSearchComponent } from './employee-advance-search/employee-advance-search.component';
import { DialogService, DialogContainerService } from '@progress/kendo-angular-dialog';
import { BrowserModule } from '@angular/platform-browser';
import { NgbModalModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [EmployeeListComponent, EmployeeCreateUpdateComponent, EmployeeInfoComponent, EmployeeAdvanceSearchComponent],
  imports: [
    CommonModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    EmployeesRoutingModule, NgbModalModule
  ],
  entryComponents: [EmployeeCreateUpdateComponent],
  providers: [
    EmployeeService,
    DialogService,
    DialogContainerService
  ]
})
export class EmployeesModule { }
