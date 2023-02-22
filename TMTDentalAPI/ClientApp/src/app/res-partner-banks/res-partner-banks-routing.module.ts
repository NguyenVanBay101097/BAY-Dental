import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ResPartnerBankListComponent } from './res-partner-bank-list/res-partner-bank-list.component';

const routes: Routes = [
  {
    path: 'res-partner-banks',
    component: ResPartnerBankListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ResPartnerBanksRoutingModule { }
