import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StockPickingListComponent } from './stock-picking-list/stock-picking-list.component';
import { StockPickingCreateUpdateComponent } from './stock-picking-create-update/stock-picking-create-update.component';
import { StockPickingOutgoingListComponent } from './stock-picking-outgoing-list/stock-picking-outgoing-list.component';
import { StockPickingOutgoingCreateUpdateComponent } from './stock-picking-outgoing-create-update/stock-picking-outgoing-create-update.component';
import { StockPickingIncomingListComponent } from './stock-picking-incoming-list/stock-picking-incoming-list.component';
import { StockPickingIncomingCreateUpdateComponent } from './stock-picking-incoming-create-update/stock-picking-incoming-create-update.component';

const routes: Routes = [
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
  {
    path: 'incoming-pickings/create',
    component: StockPickingIncomingCreateUpdateComponent
  },
  {
    path: 'incoming-pickings/edit/:id',
    component: StockPickingIncomingCreateUpdateComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StockPickingsRoutingModule { }
