import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { LaboBridgeListComponent } from './labo-bridge-list/labo-bridge-list.component';

const routes: Routes = [
  {
    path: '',
    component: LaboBridgeListComponent
  }
];
@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports:[
RouterModule
  ]
})
export class LaboBridgesRoutingModule { }
