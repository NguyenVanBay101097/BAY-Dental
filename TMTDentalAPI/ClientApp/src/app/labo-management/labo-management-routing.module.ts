import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LaboManagementComponent } from '../catalog/labo-management/labo-management.component';
import { LaboBiteJointListComponent } from '../labo-bite-joints/labo-bite-joint-list/labo-bite-joint-list.component';
import { LaboBridgeListComponent } from '../labo-bridges/labo-bridge-list/labo-bridge-list.component';
import { LaboFinishLineListComponent } from '../labo-finish-lines/labo-finish-line-list/labo-finish-line-list.component';
import { ProductLaboAttachListComponent } from '../products/product-labo-attach-list/product-labo-attach-list.component';
import { ProductLaboListComponent } from '../products/product-labo-list/product-labo-list.component';


const routes: Routes = [
  {
    path: '',
    component: LaboManagementComponent,
    children: [
      { path: '', redirectTo: 'product-labos', pathMatch: 'full' },
      {
        path: 'labo-bite-joints',
        component: LaboBiteJointListComponent,
      },
      {
        path: 'labo-bridges',
        component: LaboBridgeListComponent,
      },
      {
        path: 'labo-finish-lines',
        component: LaboFinishLineListComponent
      },
      { path: 'product-labos', component: ProductLaboListComponent },
      { path: 'labo-attachs', component: ProductLaboAttachListComponent }
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LaboManagementRoutingModule { }
