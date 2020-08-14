import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ServiceCardOrderListComponent } from './service-card-order-list/service-card-order-list.component';
import { ServiceCardOrderCreateUpdateComponent } from './service-card-order-create-update/service-card-order-create-update.component';

const routes: Routes = [
  {
    path: '',
    component: ServiceCardOrderListComponent
  },
  {
    path: 'form',
    component: ServiceCardOrderCreateUpdateComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ServiceCardOrdersRoutingModule { }
