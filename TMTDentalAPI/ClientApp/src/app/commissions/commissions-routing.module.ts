import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommissionListComponent } from './commission-list/commission-list.component';
import { CommissionCreateUpdateComponent } from './commission-create-update/commission-create-update.component';

const routes: Routes = [
  {
    path: 'commissions',
    component: CommissionListComponent
  },
  {
    path: 'commissions/form',
    component: CommissionCreateUpdateComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CommissionsRoutingModule { }
