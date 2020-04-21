import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ServiceCardOrderListComponent } from './service-card-order-list/service-card-order-list.component';
import { ServiceCardOrderCreateUpdateComponent } from './service-card-order-create-update/service-card-order-create-update.component';

const routes: Routes = [
  {
    path: 'service-card-orders/list',
    component: ServiceCardOrderListComponent
  },
  {
    path: 'service-card-orders/form',
    component: ServiceCardOrderCreateUpdateComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ServiceCardOrdersRoutingModule { }
