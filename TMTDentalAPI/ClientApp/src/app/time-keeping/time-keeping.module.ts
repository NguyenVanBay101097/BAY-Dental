import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TimeKeepingRoutingModule } from './time-keeping-routing.module';
import { TimeKeepingViewCalendarComponent } from './time-keeping-view-calendar/time-keeping-view-calendar.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TimeKeepingSetupDialogComponent } from './time-keeping-setup-dialog/time-keeping-setup-dialog.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TimeKeepingSettingDialogComponent } from './time-keeping-setting-dialog/time-keeping-setting-dialog.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';

@NgModule({
  declarations: [TimeKeepingViewCalendarComponent, TimeKeepingSetupDialogComponent, TimeKeepingSettingDialogComponent],
  imports: [
    CommonModule,
    TimeKeepingRoutingModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    MyCustomKendoModule
  ],
  entryComponents: [TimeKeepingSetupDialogComponent, TimeKeepingSettingDialogComponent]
})
export class TimeKeepingModule { }
