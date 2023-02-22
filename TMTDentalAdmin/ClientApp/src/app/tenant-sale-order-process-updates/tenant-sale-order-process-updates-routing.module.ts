import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TenantSaleOrderProcessUpdateListComponent } from './tenant-sale-order-process-update-list/tenant-sale-order-process-update-list.component';

const routes: Routes = [
  {
    path: '',
    component: TenantSaleOrderProcessUpdateListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TenantSaleOrderProcessUpdatesRoutingModule { }
