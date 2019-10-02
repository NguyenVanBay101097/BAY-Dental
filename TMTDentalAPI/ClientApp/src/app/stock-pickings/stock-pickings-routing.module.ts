import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StockPickingListComponent } from './stock-picking-list/stock-picking-list.component';
import { StockPickingCreateUpdateComponent } from './stock-picking-create-update/stock-picking-create-update.component';

const routes: Routes = [
  {
    path: 'pickings',
    component: StockPickingListComponent
  },
  {
    path: 'pickings/create',
    component: StockPickingCreateUpdateComponent
  },
  {
    path: 'pickings/edit/:id',
    component: StockPickingCreateUpdateComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StockPickingsRoutingModule { }
