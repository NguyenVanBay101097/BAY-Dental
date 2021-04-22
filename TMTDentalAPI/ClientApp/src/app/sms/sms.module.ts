import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SmsRoutingModule } from './sms-routing.module';
import { SmsTemplateListComponent } from './sms-template-list/sms-template-list.component';
import { SmsTemplateCrUpComponent } from './sms-template-cr-up/sms-template-cr-up.component';

@NgModule({
  declarations: [SmsTemplateListComponent, SmsTemplateCrUpComponent],
  imports: [
    CommonModule,
    SmsRoutingModule
  ]
})
export class SmsModule { }
