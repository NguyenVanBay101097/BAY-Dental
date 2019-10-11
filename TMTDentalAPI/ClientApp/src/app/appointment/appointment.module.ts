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

@NgModule({
  declarations: [AppointmentListComponent, AppointmentCreateUpdateComponent, AppointmentCuDialogComponent, AppointmentAdvanceSearchComponent],
  imports: [
    CommonModule,
    AppointmentRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule
  ],
  entryComponents: [AppointmentCreateUpdateComponent, PartnerInfoComponent, EmployeeInfoComponent, AppointmentCuDialogComponent],
  providers: [
    AppointmentService
  ]
})
export class AppointmentModule { }
