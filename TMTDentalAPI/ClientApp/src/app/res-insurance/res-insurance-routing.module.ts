import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { ResInsuranceDebitComponent } from "./res-insurance-debit/res-insurance-debit.component";
import { ResInsuranceDetailComponent } from "./res-insurance-detail/res-insurance-detail.component";
import { ResInsuranceListComponent } from "./res-insurance-list/res-insurance-list.component";
import { ResInsuranceHistoriesComponent } from "./res-insurance-histories/res-insurance-histories.component";

const routes: Routes = [
  {
    path: "",
    component: ResInsuranceListComponent,
  },
  {
    path: "detail",
    component: ResInsuranceDetailComponent,
    children: [
      { path: '', redirectTo: 'debit', pathMatch: 'full' },
      { path: 'debit', component: ResInsuranceDebitComponent },
      { path: 'histories', component: ResInsuranceHistoriesComponent },
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ResInsuranceRoutingModule { }
