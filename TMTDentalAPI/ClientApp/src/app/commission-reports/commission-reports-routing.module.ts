import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { CommissionReportListComponent } from './commission-report-list/commission-report-list.component';

const routes: Routes = [
  {
    path: "",
    component: CommissionReportListComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CommissionReportsRoutingModule {}