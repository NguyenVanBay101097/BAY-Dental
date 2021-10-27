import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CardCardsMemberComponent } from './card-cards-member/card-cards-member.component';
import { ServiceCardCardListComponent } from './service-card-card-list/service-card-card-list.component';
import { ServiceCardCardsManagementComponent } from './service-card-cards-management/service-card-cards-management.component';
import { ServiceCardCardsPreferentialComponent } from './service-card-cards-preferential/service-card-cards-preferential.component';

const routes: Routes = [
  {
    path: '',
    component: ServiceCardCardsManagementComponent,
    children: [
      { path: '', redirectTo: 'preferential', pathMatch: 'full' },
      { path: 'preferential', component: ServiceCardCardsPreferentialComponent },
      { path: 'member', component: CardCardsMemberComponent },

    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ServiceCardCardsRoutingModule { }
