import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CardTypeListComponent } from './card-type-list/card-type-list.component';
import { CardTypeCreateUpdateComponent } from './card-type-create-update/card-type-create-update.component';

const routes: Routes = [
  {
    path: '',
    component: CardTypeListComponent
  },
  {
    path: 'create',
    component: CardTypeCreateUpdateComponent
  },
  {
    path: 'edit/:id',
    component: CardTypeCreateUpdateComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CardTypesRoutingModule { }
