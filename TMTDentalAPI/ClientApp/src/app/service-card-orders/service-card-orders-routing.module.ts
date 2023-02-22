import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ServiceCardOrderListComponent } from './service-card-order-list/service-card-order-list.component';
import { ServiceCardOrderCreateUpdateComponent } from './service-card-order-create-update/service-card-order-create-update.component';
import { ServiceCardOrderPosComponent } from './service-card-order-pos/service-card-order-pos.component';

const routes: Routes = [
  {
    path: '',
    component: ServiceCardOrderListComponent
  },
  {
    path: 'form',
    component: ServiceCardOrderCreateUpdateComponent
  },
  {
    path: 'create-card-order',
    component: ServiceCardOrderPosComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ServiceCardOrdersRoutingModule { }
