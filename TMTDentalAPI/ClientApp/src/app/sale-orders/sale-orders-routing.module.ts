import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SaleOrderCreateUpdateComponent } from './sale-order-create-update/sale-order-create-update.component';
import { SaleOrderListComponent } from './sale-order-list/sale-order-list.component';
import { SaleOrderManagementComponent } from './sale-order-management/sale-order-management.component';
import { TreatmentProcessServiceListComponent } from './treatment-process-service-list/treatment-process-service-list.component';

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
    path: 'management',
    component: SaleOrderManagementComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SaleOrdersRoutingModule { }
