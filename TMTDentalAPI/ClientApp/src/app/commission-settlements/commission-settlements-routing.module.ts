import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommissionSettlementReportDetailComponent } from './commission-settlement-report-detail/commission-settlement-report-detail.component';
import { CommissionSettlementReportListComponent } from './commission-settlement-report-list/commission-settlement-report-list.component';
import { CommissionSettlementReportComponent } from './commission-settlement-report/commission-settlement-report.component';

const routes: Routes = [
  {
    path: 'report',
    component: CommissionSettlementReportComponent,
    children: [
      { path: '', redirectTo: 'list', pathMatch: 'full' },
      { path: 'list', component: CommissionSettlementReportListComponent },
      { path: 'detail', component: CommissionSettlementReportDetailComponent },
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CommissionSettlementsRoutingModule { }
