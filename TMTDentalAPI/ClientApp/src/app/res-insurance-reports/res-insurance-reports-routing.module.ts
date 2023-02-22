import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ResInsuranceDebitComponent } from './res-insurance-debit/res-insurance-debit.component';
import { ResInsuranceDetailComponent } from './res-insurance-detail/res-insurance-detail.component';
import { ResInsuranceHistoriesComponent } from './res-insurance-histories/res-insurance-histories.component';
import { ResInsuranceReportsOverviewComponent } from './res-insurance-reports-overview/res-insurance-reports-overview.component';


const routes: Routes = [
  {
    path: '',
    component: ResInsuranceReportsOverviewComponent
  },
  {
    path: ":id",
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
  exports: [RouterModule]
})
export class ResInsuranceReportsRoutingModule { }
