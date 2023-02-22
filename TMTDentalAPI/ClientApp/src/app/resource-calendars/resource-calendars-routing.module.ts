import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ResourceCalendarListComponent } from './resource-calendar-list/resource-calendar-list.component';
import { ResourceCalendarCreateUpdateComponent } from './resource-calendar-create-update/resource-calendar-create-update.component';
import { ResourceCalendarAttendanceCreateUpdateDialogComponent } from './resource-calendar-attendance-create-update-dialog/resource-calendar-attendance-create-update-dialog.component';

const routes: Routes = [
  { path: '', component: ResourceCalendarListComponent },
  { path: 'form', component: ResourceCalendarCreateUpdateComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ResourceCalendarsRoutingModule { }
