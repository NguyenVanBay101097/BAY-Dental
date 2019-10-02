import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountJournalsRoutingModule } from './account-journals-routing.module';
import { AccountJournalService } from './account-journal.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    AccountJournalsRoutingModule
  ],
  providers: [
    AccountJournalService
  ]
})
export class AccountJournalsModule { }
