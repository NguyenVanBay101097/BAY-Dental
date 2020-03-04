import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MarketingCampaignListComponent } from './marketing-campaign-list/marketing-campaign-list.component';
import { MarketingCampaignCreateUpdateComponent } from './marketing-campaign-create-update/marketing-campaign-create-update.component';


const routes: Routes = [
  { path: 'marketing-campaigns', component: MarketingCampaignListComponent },
  { path: 'marketing-campaigns/form', component: MarketingCampaignCreateUpdateComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MarketingCampaignsRoutingModule { }
