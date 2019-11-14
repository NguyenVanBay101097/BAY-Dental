import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StockPickingListComponent } from './stock-picking-list/stock-picking-list.component';
import { StockPickingCreateUpdateComponent } from './stock-picking-create-update/stock-picking-create-update.component';
import { StockPickingOutgoingListComponent } from './stock-picking-outgoing-list/stock-picking-outgoing-list.component';
import { StockPickingOutgoingCreateUpdateComponent } from './stock-picking-outgoing-create-update/stock-picking-outgoing-create-update.component';
import { StockPickingIncomingListComponent } from './stock-picking-incoming-list/stock-picking-incoming-list.component';

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
  },
  {
    path: 'outgoing-pickings',
    component: StockPickingOutgoingListComponent
  },
  {
    path: 'outgoing-pickings/create',
    component: StockPickingOutgoingCreateUpdateComponent
  },
  {
    path: 'outgoing-pickings/edit/:id',
    component: StockPickingOutgoingCreateUpdateComponent
  },
  {
    path: 'incoming-pickings',
    component: StockPickingIncomingListComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StockPickingsRoutingModule { }
