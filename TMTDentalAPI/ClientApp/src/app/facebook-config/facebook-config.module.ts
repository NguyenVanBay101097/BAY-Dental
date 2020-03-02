import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FacebookConfigRoutingModule } from './facebook-config-routing.module';
import { FacebookConfigEstablishComponent } from './facebook-config-establish/facebook-config-establish.component';
import { FacebookConfigService } from './shared/facebook-config.service';
import { FacebookConfigSettingComponent } from './facebook-config-setting/facebook-config-setting.component';
import { FacebookConfigPageMapCustomerComponent } from './facebook-config-page-map-customer/facebook-config-page-map-customer.component';
import { FacebookOAuthService } from './shared/facebook-oauth.service';
import { FacebookConfigPageService } from './shared/facebook-config-page.service';
import { FacebookConfigPageConversationsComponent } from './facebook-config-page-conversations/facebook-config-page-conversations.component';
import { FacebookConfigPageConversationListComponent } from './facebook-config-page-conversation-list/facebook-config-page-conversation-list.component';
import { FacebookConfigPageCustomerUpdateComponent } from './facebook-config-page-customer-update/facebook-config-page-customer-update.component';
import { FacebookUserProfileService } from './shared/facebook-user-profile.service';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [FacebookConfigEstablishComponent, FacebookConfigSettingComponent, FacebookConfigPageMapCustomerComponent, FacebookConfigPageConversationsComponent, FacebookConfigPageConversationListComponent, FacebookConfigPageCustomerUpdateComponent],
  imports: [
    CommonModule,
    FacebookConfigRoutingModule,
    FormsModule
  ],
  providers: [
    FacebookConfigService,
    FacebookOAuthService,
    FacebookConfigPageService,
    FacebookUserProfileService
  ]
})
export class FacebookConfigModule { }
