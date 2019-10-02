import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DotKhamCreateUpdateComponent } from './dot-kham-create-update/dot-kham-create-update.component';
import { DotKhamListComponent } from './dot-kham-list/dot-kham-list.component';

const routes: Routes = [
  {
    path: 'dot-khams',
    component: DotKhamListComponent
  },
  {
    path: 'dot-khams/create',
    component: DotKhamCreateUpdateComponent
  },
  {
    path: 'dot-khams/edit/:id',
    component: DotKhamCreateUpdateComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DotKhamsRoutingModule { }
