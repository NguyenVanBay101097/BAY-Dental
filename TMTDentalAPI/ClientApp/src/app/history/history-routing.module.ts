import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HistoriesListComponent } from './histories-list/histories-list.component';

const routes: Routes = [
  {
    path: '', component: HistoriesListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HistoryRoutingModule { }
