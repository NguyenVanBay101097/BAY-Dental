import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// import { CommissionListComponent } from './commission-list/commission-list.component';
import { CommissionCreateUpdateComponent } from './commission-create-update/commission-create-update.component';
import { CommissionListV2Component } from './commission-list-v2/commission-list-v2.component';

const routes: Routes = [
  {
    path: '',
    component: CommissionListV2Component
  },
  {
    path: 'form',
    component: CommissionCreateUpdateComponent
  },
  {
    path: 'employee',
    loadChildren: () => import('../commission-settlements/commission-settlements.module').then(m => m.CommissionSettlementsModule),
  },
  {
    path: 'agent',
    loadChildren: () => import('../agents/agents.module').then(m => m.AgentsModule),
  },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CommissionsRoutingModule { }
