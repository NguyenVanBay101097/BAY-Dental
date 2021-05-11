import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppointmentKanbanComponent } from './appointment-kanban/appointment-kanban.component';
import { AppointmentOverCancelComponent } from './appointment-over-cancel/appointment-over-cancel.component';

const routes: Routes = [
  {
    path: 'kanban',
    component: AppointmentKanbanComponent,
  },
  {
    path: 'over-cancel',
    component: AppointmentOverCancelComponent,
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AppointmentRoutingModule { }
