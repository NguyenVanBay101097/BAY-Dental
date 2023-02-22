import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FacebookComponent } from './facebook/facebook.component';
import { FacebookDashboardComponent } from './facebook-dashboard/facebook-dashboard.component';
import { FacebookPageManagementComponent } from './facebook-page-management/facebook-page-management.component';
import { FacebookPageMarketingCampaignsComponent } from './facebook-page-marketing-campaigns/facebook-page-marketing-campaigns.component';
import { FacebookPageMarketingCampaignCreateUpdateComponent } from './facebook-page-marketing-campaign-create-update/facebook-page-marketing-campaign-create-update.component';
import { FacebookPageMarketingCampaignListComponent } from './facebook-page-marketing-campaign-list/facebook-page-marketing-campaign-list.component';
import { FacebookPageMarketingCustomerListComponent } from './facebook-page-marketing-customer-list/facebook-page-marketing-customer-list.component';
import { FacebookMassMessagingsComponent } from './facebook-mass-messagings/facebook-mass-messagings.component';
import { FacebookMassMessagingListComponent } from './facebook-mass-messaging-list/facebook-mass-messaging-list.component';
import { FacebookMassMessagingCreateUpdateComponent } from './facebook-mass-messaging-create-update/facebook-mass-messaging-create-update.component';
import { FacebookPageListComponent } from './facebook-page-list/facebook-page-list.component';

const routes: Routes = [
  { path: 'facebook-connect', component: FacebookComponent },
  { path: 'facebook-dashboard', component: FacebookDashboardComponent },
  { path: 'channels', component: FacebookPageListComponent },
  {
    path: 'channel/:id',
    component: FacebookPageManagementComponent,
    children: [
      {
        path: '',
        redirectTo: 'customers',
        pathMatch: 'full'
      },
      {
        path: 'customers',
        component: FacebookPageMarketingCustomerListComponent,
      },
    ]
  },
  {
    path: 'facebook-management',
    component: FacebookPageManagementComponent,
    children: [
      {
        path: '',
        redirectTo: 'campaigns',
        pathMatch: 'full'
      },
      {
        path: 'campaigns',
        component: FacebookPageMarketingCampaignsComponent,
        children: [
          {
            path: '',
            component: FacebookPageMarketingCampaignListComponent
          },
          {
            path: 'form',
            component: FacebookPageMarketingCampaignCreateUpdateComponent
          },
        ]
      },
      {
        path: 'mass-messagings',
        component: FacebookMassMessagingsComponent,
        children: [
          {
            path: '',
            component: FacebookMassMessagingListComponent
          },
          {
            path: 'form',
            component: FacebookMassMessagingCreateUpdateComponent
          },
        ]
      },
      {
        path: 'customers',
        component: FacebookPageMarketingCustomerListComponent,
      },
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SocialsChannelRoutingModule { }
