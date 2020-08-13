import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ServiceCardTypeListComponent } from './service-card-type-list/service-card-type-list.component';

const routes: Routes = [
  {
    path: '',
    component: ServiceCardTypeListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ServiceCardTypesRoutingModule { }
