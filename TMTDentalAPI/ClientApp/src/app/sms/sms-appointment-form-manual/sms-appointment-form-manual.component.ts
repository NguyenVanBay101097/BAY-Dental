import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AppointmentPaged } from 'src/app/appointment/appointment';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { SmsManualDialogComponent } from '../sms-manual-dialog/sms-manual-dialog.component';

@Component({
  selector: 'app-sms-appointment-form-manual',
  templateUrl: './sms-appointment-form-manual.component.html',
  styleUrls: ['./sms-appointment-form-manual.component.css']
})
export class SmsAppointmentFormManualComponent implements OnInit {
  formGroup: FormGroup;
  filteredTemplate: any[];
  gridData: any;
  skip: number = 0;
  limit: number = 20;
  isRowSelected: any[];
  search: string = '';
  selectedIds: string[] = [];
  searchUpdate = new Subject<string>();
  dateFrom: Date;
  dateTo: Date;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  public today: Date = new Date();
  public next7days: Date = new Date(new Date(new Date().setDate(new Date().getDate() + 7)).toDateString());

  constructor(
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private intlService: IntlService,
    private appointmentService: AppointmentService
  ) { }

  ngOnInit() {
    this.dateFrom = this.today;
    this.dateTo = this.next7days;
    this.loadDataFromApi();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    var val = new AppointmentPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.dateTimeFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTimeTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:59");

    this.appointmentService.getPaged(val).pipe(
      map((response: any) =>
      (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe((res) => {
      this.gridData = res;
    }, err => {
      console.log(err);
    }
    )
  }

  pageChange(event) {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  onSend() {
    if (this.selectedIds.length == 0) {
      this.notify("Bạn phải chọn khách hàng trước khi gửi tin", false);
    }
    else {
      var modalRef = this.modalService.open(SmsManualDialogComponent, { size: "lg", windowClass: "o_technical_modal" });
      modalRef.componentInstance.title = "Tin nhắn nhắc lịch hẹn";
      modalRef.componentInstance.ids = this.selectedIds ? this.selectedIds : [];
      modalRef.componentInstance.isAppointmentReminder = true;
      modalRef.componentInstance.templateTypeTab = "appointment";
      modalRef.result.then(
        result => {
        }
      )
    }
  }

  notify(title, isSuccess = true) {
    this.notificationService.show({
      content: title,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: isSuccess ? 'success' : 'error', icon: true },
    });
  }
}
