import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PickingTypeOverviewComponent } from './picking-type-overview/picking-type-overview.component';

const routes: Routes = [
  {
    path: 'picking-type-overview',
    component: PickingTypeOverviewComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StockPickingTypesRoutingModule { }
