import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ResConfigSettingsFormComponent } from './res-config-settings-form/res-config-settings-form.component';

const routes: Routes = [
  {
    path: 'config-settings',
    component: ResConfigSettingsFormComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ResConfigSettingsRoutingModule { }
