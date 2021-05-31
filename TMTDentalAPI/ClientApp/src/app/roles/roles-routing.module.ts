import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RoleFormV2Component } from './role-form-v2/role-form-v2.component';
import { RoleFormComponent } from './role-form/role-form.component';
import { RoleListComponent } from './role-list/role-list.component';

const routes: Routes = [
  {
    path: '',
    component: RoleListComponent
  },
  {
    path: 'form',
    component: RoleFormComponent
  },
  {
    path: 'form-v2',
    component: RoleFormV2Component
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RolesRoutingModule { }
