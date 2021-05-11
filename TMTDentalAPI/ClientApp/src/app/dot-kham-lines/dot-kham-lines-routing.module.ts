import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DotKhamLineListComponent } from './dot-kham-line-list/dot-kham-line-list.component';

const routes: Routes = [
  {
    path: 'history',
    component: DotKhamLineListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DotKhamLinesRoutingModule { }
