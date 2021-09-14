import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: 'companies',
    loadChildren: () => import('../companies/companies.module').then(m => m.CompaniesModule),
  },
  {
    path: 'roles',
    loadChildren: () => import('../roles/roles.module').then(m => m.RolesModule),
  },
  {
    path: 'config-settings',
    loadChildren: () => import('../res-config-settings/res-config-settings.module').then(m => m.ResConfigSettingsModule),
  },
  {
    path: 'print-template-config',
    loadChildren: () => import('../print-template-configs/print-template-configs.module').then(m => m.PrintTemplateConfigsModule),
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SettingRoutingModule { }
