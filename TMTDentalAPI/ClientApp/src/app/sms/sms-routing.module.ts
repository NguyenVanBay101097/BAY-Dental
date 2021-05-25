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
import { SmsCampaignDetailComponent } from './sms-campaign-detail/sms-campaign-detail.component';
import { SmsCampaignListComponent } from './sms-campaign-list/sms-campaign-list.component';
import { SmsMessageDetailStatisticComponent } from './sms-message-detail-statistic/sms-message-detail-statistic.component';
import { SmsMessageStatisticComponent } from './sms-message-statistic/sms-message-statistic.component';
import { SmsStatisticComponent } from './sms-statistic/sms-statistic.component';
import { SmsTemplateListComponent } from './sms-template-list/sms-template-list.component';
import { SmsThanksFormAutomaticComponent } from './sms-thanks-form-automatic/sms-thanks-form-automatic.component';
import { SmsThanksFormManualComponent } from './sms-thanks-form-manual/sms-thanks-form-manual.component';

const routes: Routes = [
  { path: '', redirectTo: 'birthday-partners', pathMatch: 'full' },
  {
    path: 'birthday-partners', component: SmsBirthdayFormComponent,
    children: [
      { path: '', redirectTo: 'manually', pathMatch: 'full' },
      { path: 'auto', component: SmsBirthdayFormAutomaticComponent },
      { path: 'manually', component: SmsBirthdayFormManualComponent },
      { path: 'statistic', component: SmsMessageDetailStatisticComponent }
    ]
  },
  {
    path: 'thanks-customer', component: SmsAppointmentFormComponent,
    children: [
      { path: '', redirectTo: 'manually', pathMatch: 'full' },
      { path: 'auto', component: SmsThanksFormAutomaticComponent },
      { path: 'manually', component: SmsThanksFormManualComponent },
      { path: 'statistic', component: SmsMessageDetailStatisticComponent }
    ]
  },
  {
    path: 'appointment-reminder', component: SmsAppointmentFormComponent,
    children: [
      { path: '', redirectTo: 'manually', pathMatch: 'full' },
      { path: 'auto', component: SmsAppointmentFormAutomaticComponent },
      { path: 'manually', component: SmsAppointmentFormManualComponent },
      { path: 'statistic', component: SmsMessageDetailStatisticComponent }
    ]
  },
  { path: 'templates', component: SmsTemplateListComponent },
  { path: 'statistic', component: SmsStatisticComponent },
  { path: 'accounts', component: SmsAccountListComponent },
  { path: 'campaign', component: SmsCampaignListComponent, },
  { path: 'campaign/:id', component: SmsCampaignDetailComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SmsRoutingModule { }
