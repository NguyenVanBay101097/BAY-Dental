import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LaboOrderListComponent } from './labo-order-list/labo-order-list.component';
import { LaboOrderCreateUpdateComponent } from './labo-order-create-update/labo-order-create-update.component';

const routes: Routes = [
  {
    path: 'labo-orders',
    component: LaboOrderListComponent
  },
  {
    path: 'labo-orders/form',
    component: LaboOrderCreateUpdateComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LaboOrdersRoutingModule { }
