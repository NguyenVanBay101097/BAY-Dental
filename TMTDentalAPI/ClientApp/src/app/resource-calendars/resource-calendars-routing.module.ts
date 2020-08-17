import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ResourceCalendarListComponent } from './resource-calendar-list/resource-calendar-list.component';

const routes: Routes = [
  { path: '', component: ResourceCalendarListComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ResourceCalendarsRoutingModule { }
