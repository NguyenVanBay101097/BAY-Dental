import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LaboBiteJointListComponent } from '../labo-bite-joints/labo-bite-joint-list/labo-bite-joint-list.component';
import { LaboBridgeListComponent } from '../labo-bridges/labo-bridge-list/labo-bridge-list.component';
import { LaboFinishLineListComponent } from '../labo-finish-lines/labo-finish-line-list/labo-finish-line-list.component';
import { ProductLaboAttachListComponent } from '../products/product-labo-attach-list/product-labo-attach-list.component';
import { ProductLaboListComponent } from '../products/product-labo-list/product-labo-list.component';
import { LaboManagementComponent } from './labo-management/labo-management.component';
import { LaboOrderExportComponent } from './labo-order-export/labo-order-export.component';
import { LaboOrderListComponent } from './labo-order-list/labo-order-list.component';
import { LaboOrderStatisticsComponent } from './labo-order-statistics/labo-order-statistics.component';
import { OrderLaboListComponent } from './order-labo-list/order-labo-list.component';

const routes: Routes = [
  {
    path: '',
    component: LaboOrderListComponent
  },
  {
    path: 'order',
    component: OrderLaboListComponent
  },
  {
    path: 'export',
    component: LaboOrderExportComponent
  },
  {
    path: 'labo-managerment',
    component: LaboManagementComponent,
    children: [
      { path: '', redirectTo: 'product-labos', pathMatch: 'full' },
      { path: 'product-labos', component: ProductLaboListComponent },
      { path: 'labo-finish-lines', component: LaboFinishLineListComponent },
      { path: 'labo-attachs', component: ProductLaboAttachListComponent },
      { path: 'labo-bridges', component: LaboBridgeListComponent },
      { path: 'labo-bite-joints', component: LaboBiteJointListComponent },
    ]
  }

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LaboOrdersRoutingModule { }
