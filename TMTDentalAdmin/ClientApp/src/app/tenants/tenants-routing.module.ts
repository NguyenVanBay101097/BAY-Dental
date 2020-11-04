import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TenantRegisterComponent } from './tenant-register/tenant-register.component';
import { TenantListComponent } from './tenant-list/tenant-list.component';
import { AuthGuard } from '../auth/auth.guard';

const routes: Routes = [
  {
    path: '',
    component: TenantListComponent,
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TenantsRoutingModule { }
