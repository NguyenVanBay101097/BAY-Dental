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
import { SmsAccountSettingComponent } from './sms-account-setting/sms-account-setting.component';
import { SmsBirthdayFormComponent } from './sms-birthday-form/sms-birthday-form.component';
import { SmsBirthdayFormManualComponent } from './sms-birthday-form-manual/sms-birthday-form-manual.component';
import { SmsBirthdayFormAutomaticComponent } from './sms-birthday-form-automatic/sms-birthday-form-automatic.component';
import { SmsAppointmentFormAutomaticComponent } from './sms-appointment-form-automatic/sms-appointment-form-automatic.component';
import { SmsAppointmentFormManualComponent } from './sms-appointment-form-manual/sms-appointment-form-manual.component';
import { SmsAppointmentFormComponent } from './sms-appointment-form/sms-appointment-form.component';

@NgModule({
  declarations: [SmsTemplateListComponent, SmsTemplateCrUpComponent, SmsTemplateContentComponent, SmsAccountSettingComponent, SmsBirthdayFormComponent, SmsBirthdayFormManualComponent, SmsBirthdayFormAutomaticComponent, SmsAppointmentFormAutomaticComponent, SmsAppointmentFormManualComponent, SmsAppointmentFormComponent],
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
