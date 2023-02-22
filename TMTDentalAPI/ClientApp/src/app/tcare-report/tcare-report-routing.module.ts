import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TCareReportListComponent } from './tcare-report-list/tcare-report-list.component';

const routes: Routes = [
  {
    path: 'tcare-reports',
    component: TCareReportListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TcareReportRoutingModule { }