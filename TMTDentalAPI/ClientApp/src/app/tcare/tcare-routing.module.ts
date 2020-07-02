import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TcareCampaignCreateUpdateComponent } from './tcare-campaign-create-update/tcare-campaign-create-update.component';
import { TcareCampaignListComponent } from './tcare-campaign-list/tcare-campaign-list.component';
import { TcareScenarioCrUpComponent } from './tcare-scenario-cr-up/tcare-scenario-cr-up.component';
import { TcareScenarioListComponent } from './tcare-scenario-list/tcare-scenario-list.component';

const routes: Routes = [
  { path: 'tcare-scenario/:id', component: TcareScenarioCrUpComponent },
  { path: 'tcare-scenarios', component: TcareScenarioListComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TcareRoutingModule { }
