import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AgentCommissionFormComponent } from './agent-commission-form/agent-commission-form.component';
import { AgentCommissionListComponent } from './agent-commission-list/agent-commission-list.component';
import { AgentCommmissionFormDetailComponent } from './agent-commmission-form-detail/agent-commmission-form-detail.component';
import { AgentCommmissionHistoryComponent } from './agent-commmission-history/agent-commmission-history.component';
import { AgentListComponent } from './agent-list/agent-list.component';

const routes: Routes = [
  {
    path: '',
    component: AgentListComponent
  },
  {
    path: 'commission',
    component: AgentCommissionListComponent
  }, {
    path: 'commission/:id',
    component: AgentCommissionFormComponent,
    children: [
      { path: '', redirectTo: 'detail', pathMatch: 'full' },
      { path: 'detail', component: AgentCommmissionFormDetailComponent },
      { path: 'history', component: AgentCommmissionHistoryComponent },   
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AgentRoutingModule { }
