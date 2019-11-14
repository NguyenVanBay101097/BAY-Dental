import { Component, OnInit } from '@angular/core';
import { SchedulerEvent, EventStyleArgs, RemoveEvent, DateChangeEvent } from '@progress/kendo-angular-scheduler';
import { AppointmentVMService } from '../appointment-vm.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { AppointmentService } from '../appointment.service';

@Component({
  selector: 'app-appointment-calendar',
  templateUrl: './appointment-calendar.component.html',
  styleUrls: ['./appointment-calendar.component.css']
})
export class AppointmentCalendarComponent implements OnInit {

  public selectedDate: Date = new Date(2019, 10, 12);
  public events: SchedulerEvent[] = [];

  constructor(private appointmentVMService: AppointmentVMService, private modalService: NgbModal,
    private appointmentService: AppointmentService) {
    this.appointmentVMService.events$.subscribe(result => {
      this.events = result;
    });
  }

  ngOnInit() {
  }

  public getEventClass = (args: EventStyleArgs) => {
    return args.event.dataItem.state;
  }

  public onDateChange(args: DateChangeEvent): void {
    this.appointmentVMService.filter.emit(args.dateRange);
  }

  eventDblClickHandler(e) {
    this.appointmentVMService.eventEdit.emit(e.event.id);
  }

  public removeHandler(e): void {
    this.appointmentVMService.eventDelete.emit(e.event.id);
  }
}
