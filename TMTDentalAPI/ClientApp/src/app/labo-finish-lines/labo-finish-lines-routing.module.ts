import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LaboFinishLineListComponent } from './labo-finish-line-list/labo-finish-line-list.component';

const routes: Routes = [
  {
    path: '',
    component: LaboFinishLineListComponent
  }
];

@NgModule({
  imports: [
    [RouterModule.forChild(routes)]
  ],
  exports: [RouterModule]
})
export class LaboFinishLinesRoutingModule { }
