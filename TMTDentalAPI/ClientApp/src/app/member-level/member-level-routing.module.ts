import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MemberLevelCreateUpdateComponent } from './member-level-create-update/member-level-create-update.component';
import { MemberLevelListComponent } from './member-level-list/member-level-list.component';
import { MemberLevelManagementComponent } from './member-level-management/member-level-management.component';
import { MemberLevelResolve } from './member-level-resolve';

const routes: Routes = [
  { 
    path: 'management', 
    component: MemberLevelManagementComponent ,
    resolve: {
      memberLevelsResolve: MemberLevelResolve
    }
  },
  { path: 'create', component: MemberLevelCreateUpdateComponent },
  { path: 'list', component: MemberLevelListComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MemberLevelRoutingModule { }
