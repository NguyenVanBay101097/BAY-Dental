import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ServiceCardCardListComponent } from './service-card-card-list/service-card-card-list.component';

const routes: Routes = [
  {
    path: '',
    component: ServiceCardCardListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ServiceCardCardsRoutingModule { }
