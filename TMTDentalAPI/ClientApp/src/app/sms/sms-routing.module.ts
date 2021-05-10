import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SmsAccountListComponent } from './sms-account-list/sms-account-list.component';
import { SmsAccountSettingComponent } from './sms-account-setting/sms-account-setting.component';
import { SmsAppointmentFormAutomaticComponent } from './sms-appointment-form-automatic/sms-appointment-form-automatic.component';
import { SmsAppointmentFormManualComponent } from './sms-appointment-form-manual/sms-appointment-form-manual.component';
import { SmsAppointmentFormComponent } from './sms-appointment-form/sms-appointment-form.component';
import { SmsBirthdayFormAutomaticComponent } from './sms-birthday-form-automatic/sms-birthday-form-automatic.component';
import { SmsBirthdayFormManualComponent } from './sms-birthday-form-manual/sms-birthday-form-manual.component';
import { SmsBirthdayFormComponent } from './sms-birthday-form/sms-birthday-form.component';
import { SmsStatisticComponent } from './sms-statistic/sms-statistic.component';
import { SmsTemplateListComponent } from './sms-template-list/sms-template-list.component';

const routes: Routes = [
  { path: '', redirectTo: 'birthday-partners', pathMatch: 'full' },
  {
    path: 'birthday-partners', component: SmsBirthdayFormComponent,
    children: [
      { path: '', redirectTo: 'manually', pathMatch: 'full' },
      { path: 'auto', component: SmsBirthdayFormAutomaticComponent },
      { path: 'manually', component: SmsBirthdayFormManualComponent }
    ]
  },
  {
    path: 'appointment-reminder', component: SmsAppointmentFormComponent,
    children: [
      { path: '', redirectTo: 'manually', pathMatch: 'full' },
      { path: 'auto', component: SmsAppointmentFormAutomaticComponent },
      { path: 'manually', component: SmsAppointmentFormManualComponent }
    ]
  },
  { path: 'templates', component: SmsTemplateListComponent },
  { path: 'statistic', component: SmsStatisticComponent },
  { path: 'accounts', component: SmsAccountListComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SmsRoutingModule { }
