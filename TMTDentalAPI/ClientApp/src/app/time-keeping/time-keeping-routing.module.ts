import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TimeKeepingViewCalendarComponent } from './time-keeping-view-calendar/time-keeping-view-calendar.component';

const routes: Routes = [
  
  { path: '', component: TimeKeepingViewCalendarComponent },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TimeKeepingRoutingModule { }
