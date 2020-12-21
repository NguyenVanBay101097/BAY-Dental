import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LaboOrderListComponent } from './labo-order-list/labo-order-list.component';
import { LaboOrderStatisticsComponent } from './labo-order-statistics/labo-order-statistics.component';
import { OrderLaboListComponent } from './order-labo-list/order-labo-list.component';

const routes: Routes = [
  {
    path: '',
    component: LaboOrderListComponent
  },
  {
    path: 'order',
    component: OrderLaboListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LaboOrdersRoutingModule { }
