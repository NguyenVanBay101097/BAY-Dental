import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FacebookConfigEstablishComponent } from './facebook-config-establish/facebook-config-establish.component';
import { FacebookConfigSettingComponent } from './facebook-config-setting/facebook-config-setting.component';
import { FacebookConfigPageConversationsComponent } from './facebook-config-page-conversations/facebook-config-page-conversations.component';

const routes: Routes = [
  {
    path: 'facebook-config/connect',
    component: FacebookConfigEstablishComponent
  },
  {
    path: 'facebook-config/:id/setting',
    component: FacebookConfigSettingComponent
  },
  {
    path: 'facebook-config/conversations',
    component: FacebookConfigPageConversationsComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FacebookConfigRoutingModule { }
