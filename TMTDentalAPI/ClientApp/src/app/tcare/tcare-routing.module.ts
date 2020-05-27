import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TcareCampaignCreateUpdateComponent } from './tcare-campaign-create-update/tcare-campaign-create-update.component';
import { TcareCampaignListComponent } from './tcare-campaign-list/tcare-campaign-list.component';

const routes: Routes = [
  { path: 'tcare-campaign/:id', component: TcareCampaignCreateUpdateComponent },
  { path: 'tcare-campaigns', component: TcareCampaignListComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TcareRoutingModule { }
