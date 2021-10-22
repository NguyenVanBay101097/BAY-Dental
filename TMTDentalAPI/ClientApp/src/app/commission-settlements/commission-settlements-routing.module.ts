import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AgentCommissionFormComponent } from '../agents/agent-commission-form/agent-commission-form.component';
import { AgentCommissionListComponent } from '../agents/agent-commission-list/agent-commission-list.component';
import { AgentCommmissionFormDetailComponent } from '../agents/agent-commmission-form-detail/agent-commmission-form-detail.component';
import { AgentCommmissionHistoryComponent } from '../agents/agent-commmission-history/agent-commmission-history.component';
import { CommissionSettlementReportDetailComponent } from './commission-settlement-report-detail/commission-settlement-report-detail.component';
import { CommissionSettlementReportListComponent } from './commission-settlement-report-list/commission-settlement-report-list.component';
import { CommissionSettlementReportComponent } from './commission-settlement-report/commission-settlement-report.component';

const routes: Routes = [
  {
    path: 'employee',
    component: CommissionSettlementReportComponent,
    children: [
      { path: '', redirectTo: 'list', pathMatch: 'full' },
      { path: 'list', component: CommissionSettlementReportListComponent },
      { path: 'detail', component: CommissionSettlementReportDetailComponent },
    ]
  },
  {
    path: 'agent',
    children: [
      { path: '', redirectTo: 'list', pathMatch: 'full' },
      {
        path: 'list',
        component: AgentCommissionListComponent
      }
    ]
  },
  {
    path: 'agent/:id',
    component: AgentCommissionFormComponent,
    children: [
      { path: '', redirectTo: 'detail', pathMatch: 'full' },
      { path: 'detail', component: AgentCommmissionFormDetailComponent },
      { path: 'history', component: AgentCommmissionHistoryComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CommissionSettlementsRoutingModule { }
