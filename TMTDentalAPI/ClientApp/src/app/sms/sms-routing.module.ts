import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SmsTemplateListComponent } from './sms-template-list/sms-template-list.component';

const routes: Routes = [
  { path: 'message-templates', component: SmsTemplateListComponent },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SmsRoutingModule { }
