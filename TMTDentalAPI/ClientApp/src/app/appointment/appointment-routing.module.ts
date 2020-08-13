import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppointmentKanbanComponent } from './appointment-kanban/appointment-kanban.component';

const routes: Routes = [
  {
    path: 'kanban',
    component: AppointmentKanbanComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AppointmentRoutingModule { }
