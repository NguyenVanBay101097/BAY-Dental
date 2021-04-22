import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SmsAccountSettingComponent } from './sms-account-setting/sms-account-setting.component';
import { SmsTemplateListComponent } from './sms-template-list/sms-template-list.component';

const routes: Routes = [
  { path: 'templates', component: SmsTemplateListComponent },
  { path: 'accounts', component: SmsAccountSettingComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SmsRoutingModule { }
