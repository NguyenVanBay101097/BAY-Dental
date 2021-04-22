import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SmsRoutingModule } from './sms-routing.module';
import { SmsTemplateListComponent } from './sms-template-list/sms-template-list.component';
import { SmsTemplateCrUpComponent } from './sms-template-cr-up/sms-template-cr-up.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { NgbModalModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SmsTemplateContentComponent } from './sms-template-content/sms-template-content.component';

@NgModule({
  declarations: [SmsTemplateListComponent, SmsTemplateCrUpComponent, SmsTemplateContentComponent],
  imports: [
    CommonModule,
    SmsRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    NgbModalModule,
    NgbModule
  ],
  entryComponents: [
    SmsTemplateCrUpComponent
  ]
})
export class SmsModule { }
