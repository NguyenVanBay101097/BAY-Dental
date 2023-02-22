import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AppointmentRoutingModule } from './appointment-routing.module';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AppointmentCuDialogComponent } from './appointment-cu-dialog/appointment-cu-dialog.component';
import { SharedModule } from '../shared/shared.module';
import { AppointmentKanbanComponent } from './appointment-kanban/appointment-kanban.component';
import { AppointmentDetailDialogComponent } from './appointment-detail-dialog/appointment-detail-dialog.component';
import { MyCustomNgbModule } from '../shared/my-custom-ngb.module';
import { AppointmentOverCancelComponent } from './appointment-over-cancel/appointment-over-cancel.component';
import { AppointmentFilterExportExcelDialogComponent } from './appointment-filter-export-excel-dialog/appointment-filter-export-excel-dialog.component';

@NgModule({
  declarations: [
    AppointmentCuDialogComponent,
    AppointmentKanbanComponent,
    AppointmentDetailDialogComponent,
    AppointmentOverCancelComponent,
    AppointmentFilterExportExcelDialogComponent
  ],
  imports: [
    CommonModule,
    AppointmentRoutingModule,
    MyCustomKendoModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    MyCustomNgbModule
  ],
  entryComponents: [AppointmentCuDialogComponent, AppointmentDetailDialogComponent, AppointmentFilterExportExcelDialogComponent],
  providers: [],
})
export class AppointmentModule { }
