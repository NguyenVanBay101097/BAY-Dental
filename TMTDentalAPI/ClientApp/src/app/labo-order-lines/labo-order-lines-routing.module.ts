import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LaboOrderLineListComponent } from './labo-order-line-list/labo-order-line-list.component';

const routes: Routes = [
  {
    path: 'labo-order-lines',
    component: LaboOrderLineListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LaboOrderLinesRoutingModule { }
