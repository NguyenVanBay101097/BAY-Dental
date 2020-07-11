import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DotKhamStepReportComponent } from './dot-kham-step-report/dot-kham-step-report.component';

const routes: Routes = [
  { path: 'dot-kham-report', component: DotKhamStepReportComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DotKhamStepsRoutingModule { }
