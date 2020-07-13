import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountAccountsRoutingModule } from './account-accounts-routing.module';
import { AccountAccountListComponent } from './account-account-list/account-account-list.component';
import { AccountAccountFormComponent } from './account-account-form/account-account-form.component';
import { AccountAccountService } from './account-account.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [AccountAccountListComponent, AccountAccountFormComponent],
  imports: [
    CommonModule,
    AccountAccountsRoutingModule, 
    MyCustomKendoModule, 
    FormsModule
  ], providers: [
    AccountAccountService
  ]
})
export class AccountAccountsModule { }
