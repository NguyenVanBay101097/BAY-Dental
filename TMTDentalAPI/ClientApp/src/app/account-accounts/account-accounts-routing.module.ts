import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccountAccountListComponent } from './account-account-list/account-account-list.component';

const routes: Routes = [
  {
    path: 'account-accounts',
    component: AccountAccountListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountAccountsRoutingModule { }
