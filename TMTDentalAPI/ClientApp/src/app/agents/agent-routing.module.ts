import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AgentListComponent } from './agent-list/agent-list.component';

const routes: Routes = [
  {
    path: 'list',
    component: AgentListComponent
  },
  // {
  //   path: 'commission',
  //   component: AgentCommissionListComponent
  // }, {
  //   path: 'commission/:id',
  //   component: AgentCommissionFormComponent,
  //   children: [
  //     { path: '', redirectTo: 'detail', pathMatch: 'full' },
  //     { path: 'detail', component: AgentCommmissionFormDetailComponent },
  //     { path: 'history', component: AgentCommmissionHistoryComponent },
  //   ]
  // },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AgentRoutingModule { }
