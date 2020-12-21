import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
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
