import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ServiceCardTypeListComponent } from './service-card-type-list/service-card-type-list.component';

const routes: Routes = [
  {
    path: 'service-card-types',
    component: ServiceCardTypeListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ServiceCardTypesRoutingModule { }
