import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ResourceCalendarsRoutingModule } from './resource-calendars-routing.module';
import { ResourceCalendarListComponent } from './resource-calendar-list/resource-calendar-list.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ResourceCalendarCreateUpdateComponent } from './resource-calendar-create-update/resource-calendar-create-update.component';
import { SharedModule } from '../shared/shared.module';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { ResourceCalendarAttendanceCreateUpdateDialogComponent } from './resource-calendar-attendance-create-update-dialog/resource-calendar-attendance-create-update-dialog.component';

@NgModule({
  declarations: [
    ResourceCalendarListComponent,
    ResourceCalendarCreateUpdateComponent,
    ResourceCalendarAttendanceCreateUpdateDialogComponent,],
  imports: [
    CommonModule,
    ResourceCalendarsRoutingModule,
    NgbModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    MyCustomKendoModule,
    DragDropModule
  ],
  schemas: [NO_ERRORS_SCHEMA],
  entryComponents: [ResourceCalendarAttendanceCreateUpdateDialogComponent]
})
export class ResourceCalendarsModule { }
