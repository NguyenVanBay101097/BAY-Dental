import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PurchaseOrderCreateUpdateComponent } from './purchase-order-create-update/purchase-order-create-update.component';
import { PurchaseOrderListComponent } from './purchase-order-list/purchase-order-list.component';

const routes: Routes = [
  {
    path: 'orders',
    component: PurchaseOrderListComponent
  },
  {
    path: 'orders/create',
    component: PurchaseOrderCreateUpdateComponent
  },
  {
    path: 'orders/edit/:id',
    component: PurchaseOrderCreateUpdateComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PurchaseOrdersRoutingModule { }
