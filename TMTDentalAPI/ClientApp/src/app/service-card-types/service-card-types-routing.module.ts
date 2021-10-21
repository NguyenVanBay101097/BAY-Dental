import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MemberCardListComponent } from './service-card-type-list/member-card-list/member-card-list.component';
import { PreferentialCardCreateUpdateComponent } from './service-card-type-list/preferential-card-create-update/preferential-card-create-update.component';
import { PreferentialCardListComponent } from './service-card-type-list/preferential-card-list/preferential-card-list.component';
import { ServiceCardTypeListComponent } from './service-card-type-list/service-card-type-list.component';

const routes: Routes = [
  {
    path: '',
    component: ServiceCardTypeListComponent,
    children: [
      { path: '', redirectTo: 'preferential-cards', pathMatch: 'full' },
      {path: 'member-cards', component: MemberCardListComponent},
      {path: 'preferential-cards', component: PreferentialCardListComponent},
    ]
  },
  {path: 'preferential-cards/form', component: PreferentialCardCreateUpdateComponent}

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ServiceCardTypesRoutingModule { }
