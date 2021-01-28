import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TenantListComponent } from './tenant-list/tenant-list.component';
import { TrialRegistrationComponent } from './trial-registration/trial-registration.component';

const routes: Routes = [
  {
    path: '',
    component: TenantListComponent,
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
