import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LaboBiteJointListComponent } from './labo-bite-joint-list/labo-bite-joint-list.component';

const routes: Routes = [
  {
    path: '',
    component: LaboBiteJointListComponent
  }
]; 
@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class LaboBiteJointsRoutingModule { }
