import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AgentCommissionListComponent } from './agent-commission-list/agent-commission-list.component';
import { AgentListComponent } from './agent-list/agent-list.component';

const routes: Routes = [
  {
    path: '',
    component: AgentListComponent
  },
  {
    path: 'commission',
    component: AgentCommissionListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AgentRoutingModule { }
