import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CardCardListComponent } from './card-card-list/card-card-list.component';

const routes: Routes = [
  {
    path: 'card-cards',
    component: CardCardListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CardCardsRoutingModule { }
