import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CardCardListComponent } from './card-card-list/card-card-list.component';
import { CardCardCuDialogComponent } from './card-card-cu-dialog/card-card-cu-dialog.component';

const routes: Routes = [
  {
    path: '',
    component: CardCardListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CardCardsRoutingModule { }
