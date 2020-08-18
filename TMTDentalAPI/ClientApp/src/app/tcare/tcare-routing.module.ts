import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TcareScenarioCrUpComponent } from './tcare-scenario-cr-up/tcare-scenario-cr-up.component';
import { TcareScenarioListComponent } from './tcare-scenario-list/tcare-scenario-list.component';

const routes: Routes = [
  { path: 'scenario/:id', component: TcareScenarioCrUpComponent },
  { path: 'scenarios', component: TcareScenarioListComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TcareRoutingModule { }
