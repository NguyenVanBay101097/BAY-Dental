import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppointmentListComponent } from './appointment-list/appointment-list.component';
import { AppointmentCalendarComponent } from './appointment-calendar/appointment-calendar.component';
import { AppointmentKanbanComponent } from './appointment-kanban/appointment-kanban.component';

const routes: Routes = [
  {
    path: 'appointments',
    component: AppointmentListComponent,
    children: [
      { path: '', redirectTo: 'time', pathMatch: 'full' },
      { path: 'calendar', component: AppointmentCalendarComponent },
      { path: 'kanban', component: AppointmentKanbanComponent }
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AppointmentRoutingModule { }
