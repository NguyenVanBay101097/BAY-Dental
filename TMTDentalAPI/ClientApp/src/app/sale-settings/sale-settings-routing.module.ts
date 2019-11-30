import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SaleSettingsOverviewComponent } from './sale-settings-overview/sale-settings-overview.component';

const routes: Routes = [
  {
    path: 'sale-settings',
    component: SaleSettingsOverviewComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SaleSettingsRoutingModule { }
