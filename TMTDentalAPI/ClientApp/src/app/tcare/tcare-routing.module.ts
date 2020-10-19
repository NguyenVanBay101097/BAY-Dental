import { TcareMessagingListComponent } from './tcare-messaging-list/tcare-messaging-list.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TcareScenarioCrUpComponent } from './tcare-scenario-cr-up/tcare-scenario-cr-up.component';
import { TcareScenarioListComponent } from './tcare-scenario-list/tcare-scenario-list.component';

const routes: Routes = [
  { path: 'scenarios/form', component: TcareScenarioCrUpComponent },
  { path: 'scenarios', component: TcareScenarioListComponent },
  { path: 'messagings', component: TcareMessagingListComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TcareRoutingModule { }
