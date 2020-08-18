import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LaboOrderStatisticsComponent } from './labo-order-statistics/labo-order-statistics.component';

const routes: Routes = [
  {
    path: 'statistics',
    component: LaboOrderStatisticsComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LaboOrdersRoutingModule { }
