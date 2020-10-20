import { TcareMessagingListComponent } from './tcare-messaging-list/tcare-messaging-list.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TcareScenarioCrUpComponent } from './tcare-scenario-cr-up/tcare-scenario-cr-up.component';
import { TcareScenarioListComponent } from './tcare-scenario-list/tcare-scenario-list.component';
import { TcareMessageTemplateListComponent } from './tcare-message-template-list/tcare-message-template-list.component';

const routes: Routes = [
  { path: 'scenarios/form', component: TcareScenarioCrUpComponent },
  { path: 'scenarios', component: TcareScenarioListComponent },
  { path: 'messagings', component: TcareMessagingListComponent },
  { path: 'message-templates', component: TcareMessageTemplateListComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TcareRoutingModule { }
