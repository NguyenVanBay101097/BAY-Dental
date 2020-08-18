import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SamplePrescriptionListComponent } from './sample-prescription-list/sample-prescription-list.component';

const routes: Routes = [
  {
    path: '',
    component: SamplePrescriptionListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SamplePrescriptionsRoutingModule { }