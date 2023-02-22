import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RoutingListComponent } from './routing-list/routing-list.component';
import { RoutingCreateUpdateComponent } from './routing-create-update/routing-create-update.component';

const routes: Routes = [
  {
    path: 'routings',
    component: RoutingListComponent
  },
  {
    path: 'routings/create',
    component: RoutingCreateUpdateComponent
  },
  {
    path: 'routings/edit/:id',
    component: RoutingCreateUpdateComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RoutingsRoutingModule { }
