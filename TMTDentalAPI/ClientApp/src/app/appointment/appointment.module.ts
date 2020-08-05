import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AppointmentRoutingModule } from './appointment-routing.module';
import { AppointmentListComponent } from './appointment-list/appointment-list.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { AppointmentCreateUpdateComponent } from './appointment-create-update/appointment-create-update.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AppointmentCuDialogComponent } from './appointment-cu-dialog/appointment-cu-dialog.component';
import { AppointmentAdvanceSearchComponent } from './appointment-advance-search/appointment-advance-search.component';
import { SharedModule } from '../shared/shared.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AppointmentDateFilterComponent } from './appointment-date-filter/appointment-date-filter.component';
import { AppointmentStateFilterComponent } from './appointment-state-filter/appointment-state-filter.component';
import { AppointmentCalendarComponent } from './appointment-calendar/appointment-calendar.component';
import { AppointmentKanbanComponent } from './appointment-kanban/appointment-kanban.component';
import { AppointmentDetailDialogComponent } from './appointment-detail-dialog/appointment-detail-dialog.component';

@NgModule({
  declarations: [AppointmentListComponent, AppointmentCreateUpdateComponent, AppointmentCuDialogComponent, AppointmentAdvanceSearchComponent, AppointmentDateFilterComponent, AppointmentStateFilterComponent, AppointmentCalendarComponent, AppointmentKanbanComponent, AppointmentDetailDialogComponent],
  imports: [
    CommonModule,
    AppointmentRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    NgbModule
  ],
  entryComponents: [AppointmentCreateUpdateComponent, AppointmentCuDialogComponent, AppointmentDetailDialogComponent],
  providers: [],
  exports: [
    AppointmentCreateUpdateComponent
  ]
})
export class AppointmentModule { }
