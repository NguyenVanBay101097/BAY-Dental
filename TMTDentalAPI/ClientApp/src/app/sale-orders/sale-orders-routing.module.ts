import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SaleOrderCreateUpdateComponent } from './sale-order-create-update/sale-order-create-update.component';

const routes: Routes = [
  {
    path: 'sale-orders/create',
    component: SaleOrderCreateUpdateComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SaleOrdersRoutingModule { }
