import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ToothDiagnosisListComponent } from './tooth-diagnosis-list/tooth-diagnosis-list.component';

const routes: Routes = [
  {
    path: '',
    component: ToothDiagnosisListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ToothDiagnosisRoutingModule { }
