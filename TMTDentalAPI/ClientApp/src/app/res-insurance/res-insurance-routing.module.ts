import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { ResInsuranceListComponent } from "./res-insurance-list/res-insurance-list.component";

const routes: Routes = [
  {
    path: "",
    component: ResInsuranceListComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ResInsuranceRoutingModule { }
