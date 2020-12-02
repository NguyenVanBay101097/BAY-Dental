import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SaleOrderCreateUpdateComponent } from './sale-order-create-update/sale-order-create-update.component';
import { SaleOrderListComponent } from './sale-order-list/sale-order-list.component';
import { SaleOrdersDotkhamCuComponent } from './sale-orders-dotkham-cu/sale-orders-dotkham-cu.component';

const routes: Routes = [
  {
    path: '',
    component: SaleOrderListComponent
  },
  {
    path: 'form',
    component: SaleOrderCreateUpdateComponent
  },
  {
    path: 'dotkham',
    component: SaleOrdersDotkhamCuComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SaleOrdersRoutingModule { }
