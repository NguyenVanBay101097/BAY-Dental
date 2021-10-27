import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HrJobListComponent } from './hr-job-list/hr-job-list.component';

const routes: Routes = [
  {
    path: '',
    component: HrJobListComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HrJobsRoutingModule { }
