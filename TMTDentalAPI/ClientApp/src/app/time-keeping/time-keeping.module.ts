import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TimeKeepingRoutingModule } from './time-keeping-routing.module';
import { TimeKeepingViewCalendarComponent } from './time-keeping-view-calendar/time-keeping-view-calendar.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TimeKeepingSetupDialogComponent } from './time-keeping-setup-dialog/time-keeping-setup-dialog.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { TimeKeepingSettingDialogComponent } from './time-keeping-setting-dialog/time-keeping-setting-dialog.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { TimeKeepingImportFileComponent } from './time-keeping-import-file/time-keeping-import-file.component';
import { HrsModule } from '../hrs/hrs.module';
import { TimeKeepingDateFilterComponent } from './time-keeping-date-filter/time-keeping-date-filter.component';
import { TimeKeepingForallDialogComponent } from './time-keeping-forall-dialog/time-keeping-forall-dialog.component';

@NgModule({
  declarations: [
    TimeKeepingViewCalendarComponent,
    TimeKeepingSetupDialogComponent,
    TimeKeepingSettingDialogComponent,
    TimeKeepingImportFileComponent,
    TimeKeepingDateFilterComponent,
    TimeKeepingForallDialogComponent
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
