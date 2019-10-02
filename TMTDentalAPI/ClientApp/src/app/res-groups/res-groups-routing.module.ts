import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ResGroupListComponent } from './res-group-list/res-group-list.component';
import { ResGroupCreateUpdateComponent } from './res-group-create-update/res-group-create-update.component';

const routes: Routes = [
  {
    path: 'res-groups',
    component: ResGroupListComponent
  },
  {
    path: 'res-groups/create',
    component: ResGroupCreateUpdateComponent
  },
  {
    path: 'res-groups/edit/:id',
    component: ResGroupCreateUpdateComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ResGroupsRoutingModule { }
