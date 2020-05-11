import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AppointmentRoutingModule } from './appointment-routing.module';
import { AppointmentListComponent } from './appointment-list/appointment-list.component';
import { AppointmentService } from './appointment.service';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { AppointmentCreateUpdateComponent } from './appointment-create-update/appointment-create-update.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { PartnerInfoComponent } from '../partners/partner-info/partner-info.component';
import { AppointmentCuDialogComponent } from './appointment-cu-dialog/appointment-cu-dialog.component';
import { EmployeeInfoComponent } from '../employees/employee-info/employee-info.component';
import { AppointmentAdvanceSearchComponent } from './appointment-advance-search/appointment-advance-search.component';
import { SharedModule } from '../shared/shared.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AppointmentDateFilterComponent } from './appointment-date-filter/appointment-date-filter.component';
import { AppointmentStateFilterComponent } from './appointment-state-filter/appointment-state-filter.component';
import { AppointmentCalendarComponent } from './appointment-calendar/appointment-calendar.component';
import { AppointmentVMService } from './appointment-vm.service';
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
  entryComponents: [AppointmentCreateUpdateComponent, PartnerInfoComponent, EmployeeInfoComponent, AppointmentCuDialogComponent, AppointmentDetailDialogComponent],
  providers: [
    AppointmentService,
  ]
})
export class AppointmentModule { }
