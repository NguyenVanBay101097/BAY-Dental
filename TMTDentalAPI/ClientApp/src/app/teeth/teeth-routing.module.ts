import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ToothSelectDialogComponent } from './tooth-select-dialog/tooth-select-dialog.component';

const routes: Routes = [
  {
    path: 'teet-map',
    component: ToothSelectDialogComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TeethRoutingModule { }
