import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TimeKeepingViewCalendarComponent } from './time-keeping-view-calendar/time-keeping-view-calendar.component';
import { TimeKeepingWorkEntryTypeComponent } from './time-keeping-work-entry-type/time-keeping-work-entry-type.component';

const routes: Routes = [
  
  { path: '', component: TimeKeepingViewCalendarComponent },

  { path: 'types', component: TimeKeepingWorkEntryTypeComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TimeKeepingRoutingModule { }
