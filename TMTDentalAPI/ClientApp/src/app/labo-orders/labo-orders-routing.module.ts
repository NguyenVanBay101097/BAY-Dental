import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LaboOrderListComponent } from './labo-order-list/labo-order-list.component';
import { LaboOrderStatisticsComponent } from './labo-order-statistics/labo-order-statistics.component';

const routes: Routes = [
  {
    path: '',
    component: LaboOrderListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LaboOrdersRoutingModule { }
