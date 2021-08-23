import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PurchaseOrderCreateUpdateComponent } from './purchase-order-create-update/purchase-order-create-update.component';
import { PurchaseOrderListComponent } from './purchase-order-list/purchase-order-list.component';

const routes: Routes = [
  {
    path: 'order',
    component: PurchaseOrderListComponent,
    data: { type: 'order' }
  },
  {
    path: 'refund',
    component: PurchaseOrderListComponent,
    data: { type: 'refund' }
  },
  {
    path: 'order/create',
    component: PurchaseOrderCreateUpdateComponent,
    data: { type: 'order' }
  },
  {
    path: 'refund/create',
    component: PurchaseOrderCreateUpdateComponent,
    data: { type: 'refund' }
  },
  {
    path: 'order/edit/:id',
    component: PurchaseOrderCreateUpdateComponent,
    data: { type: 'order' }
  },
  {
    path: 'refund/edit/:id',
    component: PurchaseOrderCreateUpdateComponent,
    data: { type: 'refund' }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PurchaseOrdersRoutingModule { }
