import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TimeKeepingRoutingModule } from './time-keeping-routing.module';
import { TimeKeepingViewCalendarComponent } from './time-keeping-view-calendar/time-keeping-view-calendar.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TimeKeepingSetupDialogComponent } from './time-keeping-setup-dialog/time-keeping-setup-dialog.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [TimeKeepingViewCalendarComponent, TimeKeepingSetupDialogComponent],
  imports: [
    CommonModule,
    TimeKeepingRoutingModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule
  ],
  entryComponents: [TimeKeepingSetupDialogComponent]
})
export class TimeKeepingModule { }
