import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LaboOrderExportExportedComponent } from './labo-order-export-exported/labo-order-export-exported.component';
import { LaboOrderExportNotExportComponent } from './labo-order-export-not-export/labo-order-export-not-export.component';
import { LaboOrderExportComponent } from './labo-order-export/labo-order-export.component';
import { LaboOrderListComponent } from './labo-order-list/labo-order-list.component';
import { OrderLaboListComponent } from './order-labo-list/order-labo-list.component';

const routes: Routes = [
  {
    path: 'service',
    component: LaboOrderListComponent
  },
  {
    path: 'order',
    component: OrderLaboListComponent
  },
  {
    path: 'export',
    component: LaboOrderExportComponent,
    children: [
      { path: '', redirectTo: 'exported', pathMatch: 'full' },
      { path: 'exported', component: LaboOrderExportExportedComponent},
      { path: 'not-export', component: LaboOrderExportNotExportComponent}
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LaboOrdersRoutingModule { }
