import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FacebookComponent } from './facebook/facebook.component';
import { FacebookDashboardComponent } from './facebook-dashboard/facebook-dashboard.component';

const routes: Routes = [
  { path: 'facebook-connect', component: FacebookComponent },
  { path: 'facebook-dashboard', component: FacebookDashboardComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SocialsChannelRoutingModule { }
