import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmployeeAdminRoutingModule } from './employee-admin-routing.module';
import { EmployeeAdminRegisterComponent } from './employee-admin-register/employee-admin-register.component';
import { EmployeeAdminListComponent } from './employee-admin-list/employee-admin-list.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyCustomKendoModule } from 'app/my-custom-kendo.module';
import { EmployeeAdminService } from './employee-admin.service';
import { EmployeeAdminUpdateComponent } from './employee-admin-update/employee-admin-update.component';


@NgModule({
  imports: [
    CommonModule,
    EmployeeAdminRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    MyCustomKendoModule
  ],
  declarations: [
    EmployeeAdminRegisterComponent,
    EmployeeAdminListComponent,
    EmployeeAdminUpdateComponent
  ],
  providers: [
    EmployeeAdminService
  ],
  entryComponents: [
    
  ]
})
export class EmployeeAdminModule { }
