import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { ReportPartnerSourceListComponent } from './report-partner-source-list/report-partner-source-list.component';

const routes: Routes = [
  {
    path: "",
    component: ReportPartnerSourceListComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ReportPartnerSourcesRoutingModule {}