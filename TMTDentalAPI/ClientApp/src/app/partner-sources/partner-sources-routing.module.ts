import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { PartnerSourceListComponent } from "./partner-source-list/partner-source-list.component";

const routes: Routes = [
  {
    path: "partner-sources",
    component: PartnerSourceListComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PartnerSourcesRoutingModule {}
