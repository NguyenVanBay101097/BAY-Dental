import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { EmployeeAdminRegisterComponent } from './employee-admin-register/employee-admin-register.component';
import { EmployeeAdminListComponent } from './employee-admin-list/employee-admin-list.component';
import { EmployeeAdminUpdateComponent } from './employee-admin-update/employee-admin-update.component';

const routes: Routes = [
  {
    path: '',
    component: EmployeeAdminListComponent,
  },
  {
    path: 'register',
    component: EmployeeAdminRegisterComponent
  },
  {
    path: 'update/:id',
    component: EmployeeAdminUpdateComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EmployeeAdminRoutingModule { }
