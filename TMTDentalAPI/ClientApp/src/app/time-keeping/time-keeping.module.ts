import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '../shared/shared.module';
import { TimeKeepingDateFilterComponent } from './time-keeping-date-filter/time-keeping-date-filter.component';
import { TimeKeepingForallDialogComponent } from './time-keeping-forall-dialog/time-keeping-forall-dialog.component';
import { TimeKeepingImportFileComponent } from './time-keeping-import-file/time-keeping-import-file.component';
import { TimeKeepingPopoverComponent } from './time-keeping-popover/time-keeping-popover.component';
import { TimeKeepingRoutingModule } from './time-keeping-routing.module';
import { TimeKeepingSettingDialogComponent } from './time-keeping-setting-dialog/time-keeping-setting-dialog.component';
import { TimeKeepingSetupDialogComponent } from './time-keeping-setup-dialog/time-keeping-setup-dialog.component';
import { TimeKeepingViewCalendarComponent } from './time-keeping-view-calendar/time-keeping-view-calendar.component';


@NgModule({
  declarations: [
    TimeKeepingViewCalendarComponent,
    TimeKeepingSetupDialogComponent,
    TimeKeepingSettingDialogComponent,
    TimeKeepingImportFileComponent,
    TimeKeepingDateFilterComponent,
    TimeKeepingForallDialogComponent,
    TimeKeepingPopoverComponent
  ],
  imports: [
    CommonModule,
    TimeKeepingRoutingModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    MyCustomKendoModule
  ],
  entryComponents: [
    TimeKeepingSetupDialogComponent,
    TimeKeepingSettingDialogComponent,
    TimeKeepingImportFileComponent,
    TimeKeepingForallDialogComponent
  ],
})
export class TimeKeepingModule { }
