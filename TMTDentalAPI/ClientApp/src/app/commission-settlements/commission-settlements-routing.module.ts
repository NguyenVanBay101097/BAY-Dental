import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommissionSettlementReportListComponent } from './commission-settlement-report-list/commission-settlement-report-list.component';

const routes: Routes = [
  {
    path: "",
    component: CommissionSettlementReportListComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CommissionSettlementsRoutingModule { }
