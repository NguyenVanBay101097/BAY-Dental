import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SaleOrderCreateUpdateComponent } from './sale-order-create-update/sale-order-create-update.component';
import { SaleOrderListComponent } from './sale-order-list/sale-order-list.component';
import { SaleOrderManagementComponent } from './sale-order-management/sale-order-management.component';
import { SaleOrderResolver } from './sale-order.resolver';

const routes: Routes = [
  {
    path: '',
    component: SaleOrderListComponent
  },
  {
    path: 'form',
    component: SaleOrderCreateUpdateComponent,
    // resolve: { 
    //   saleOrder: SaleOrderResolver 
    // }
  },
  
  {
    path: 'management',
    component: SaleOrderManagementComponent
  },
  {
    path: ':id',
    component: SaleOrderCreateUpdateComponent,
    resolve: { 
      saleOrder: SaleOrderResolver 
    }
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SaleOrdersRoutingModule { }
