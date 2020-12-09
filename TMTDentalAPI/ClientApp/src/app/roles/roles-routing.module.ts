import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
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
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RolesRoutingModule { }
