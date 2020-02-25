import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FacebookConfigEstablishComponent } from './facebook-config-establish/facebook-config-establish.component';

const routes: Routes = [
  {
    path: 'facebook-config',
    component: FacebookConfigEstablishComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FacebookConfigRoutingModule { }
