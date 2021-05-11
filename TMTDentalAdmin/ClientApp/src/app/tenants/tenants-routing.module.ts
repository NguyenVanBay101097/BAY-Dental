import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TenantListComponent } from './tenant-list/tenant-list.component';
import { TenantsProcessUpdateComponent } from './tenants-process-update/tenants-process-update.component';
import { TrialRegistrationComponent } from './trial-registration/trial-registration.component';

const routes: Routes = [
  {
    path: '',
    component: TenantListComponent,
  },
  {
    path: 'process-update',
    component: TenantsProcessUpdateComponent,
  },
  {
    path: 'register',
    component: TrialRegistrationComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TenantsRoutingModule { }
