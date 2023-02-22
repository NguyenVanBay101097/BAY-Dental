import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ResBankListComponent } from './res-bank-list/res-bank-list.component';

const routes: Routes = [
  {
    path: 'res-banks',
    component: ResBankListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ResBanksRoutingModule { }
