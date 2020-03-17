import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FacebookComponent } from './facebook/facebook.component';
import { FacebookDashboardComponent } from './facebook-dashboard/facebook-dashboard.component';
import { FacebookPageManagementComponent } from './facebook-page-management/facebook-page-management.component';
import { FacebookPageMarketingCampaignsComponent } from './facebook-page-marketing-campaigns/facebook-page-marketing-campaigns.component';
import { FacebookPageMarketingCampaignCreateUpdateComponent } from './facebook-page-marketing-campaign-create-update/facebook-page-marketing-campaign-create-update.component';
import { FacebookPageMarketingCampaignListComponent } from './facebook-page-marketing-campaign-list/facebook-page-marketing-campaign-list.component';

const routes: Routes = [
  { path: 'facebook-connect', component: FacebookComponent },
  { path: 'facebook-dashboard', component: FacebookDashboardComponent },
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
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SocialsChannelRoutingModule { }
