import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CardTypeListComponent } from './card-type-list/card-type-list.component';
import { CardTypeCreateUpdateComponent } from './card-type-create-update/card-type-create-update.component';

const routes: Routes = [
  {
    path: 'card-types',
    component: CardTypeListComponent
  },
  {
    path: 'card-types/create',
    component: CardTypeCreateUpdateComponent
  },
  {
    path: 'card-types/edit/:id',
    component: CardTypeCreateUpdateComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CardTypesRoutingModule { }
