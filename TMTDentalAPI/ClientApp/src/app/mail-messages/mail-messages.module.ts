import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MailMessagesRoutingModule } from './mail-messages-routing.module';
import { MailMessageService } from './mail-message.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    MailMessagesRoutingModule
  ],
  providers: [
    MailMessageService
  ]
})
export class MailMessagesModule { }
