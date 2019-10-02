import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RoleListComponent } from './role-list/role-list.component';
import { RoleCreateUpdateComponent } from './role-create-update/role-create-update.component';

const routes: Routes = [
  {
    path: 'roles',
    component: RoleListComponent
  },
  {
    path: 'roles/create',
    component: RoleCreateUpdateComponent
  },
  {
    path: 'roles/edit/:id',
    component: RoleCreateUpdateComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RolesRoutingModule { }
