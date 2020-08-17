import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ResourceCalendarsRoutingModule } from './resource-calendars-routing.module';
import { ResourceCalendarListComponent } from './resource-calendar-list/resource-calendar-list.component';
import { ResourceCalendarCrupDialogComponent } from './resource-calendar-crup-dialog/resource-calendar-crup-dialog.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { SharedModule } from '@progress/kendo-angular-grid';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [ResourceCalendarListComponent, ResourceCalendarCrupDialogComponent],
  imports: [
    CommonModule,
    ResourceCalendarsRoutingModule,
    NgbModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    MyCustomKendoModule
  ],
  entryComponents: [
    ResourceCalendarCrupDialogComponent
  ]
})
export class ResourceCalendarsModule { }
