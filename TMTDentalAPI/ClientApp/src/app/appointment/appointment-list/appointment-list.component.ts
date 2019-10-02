import { Component, OnInit } from '@angular/core';
import { AppointmentService } from '../appointment.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { AppointmentBasic, AppointmentPaged, SchedulerConfig, ApplicationUserSimple, AppointmentPatch } from '../appointment';
import { AppointmentCreateUpdateComponent } from '../appointment-create-update/appointment-create-update.component';
import { WindowRef, WindowService, WindowCloseResult, DialogService, DialogRef, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { FormGroup, FormControl } from '@angular/forms';
import { PartnerInfoComponent } from 'src/app/partners/partner-info/partner-info.component';
import { SchedulerEvent } from '@progress/kendo-angular-scheduler';
import { formatDate } from '@angular/common';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-appointment-list',
  templateUrl: './appointment-list.component.html',
  styleUrls: ['./appointment-list.component.css']
})
export class AppointmentListComponent implements OnInit {

  constructor(private service: AppointmentService, private windowService: WindowService,
    private dialogService: DialogService, private intlService: IntlService, private notificationService: NotificationService) { }

  gridView: GridDataResult;
  windowOpened: boolean;
  customerInfoOpened: boolean;
  ab: AppointmentBasic[];
  limit = 20;
  loading = false;
  skip = 0;
  stopTypingTimer: any;
  schedulerConfig: SchedulerConfig;

  schedulerShow: boolean;

  fromDateList: Date;
  toDateList: Date;
  fromDateScheduler: Date;
  toDateScheduler: Date;

  searchCustomer: string;
  searchDoctor: string;
  searchAppoint: string;
  searchCustomerUpdate = new Subject<string>();
  searchDoctorUpdate = new Subject<string>();
  searchAppointUpdate = new Subject<string>();

  confirmed: boolean;
  cancel: boolean;
  done: boolean;
  expired: boolean;
  waiting: boolean;

  formFilter = new FormGroup({
    searchCustomer: new FormControl(),
    searchDoctor: new FormControl(),
    searchAppoint: new FormControl(),
    frDate: new FormControl(),
    toDate: new FormControl(),
    confirmed: new FormControl(),
    done: new FormControl(),
    cancel: new FormControl(),
    expired: new FormControl(),
    waiting: new FormControl()
  })


  events: SchedulerEvent[];
  resources: any[] = [{
    name: 'State',
    data: [
      { text: 'Đang hẹn', value: 'confirmed', color: '#04c835' },
      { text: 'Đã hủy', value: 'cancel', color: '#cc0000' },
      { text: 'Kết thúc', value: 'done', color: '#666666' },
      { text: 'Đang chờ', value: 'waiting', color: '#0080ff' },
      { text: 'Quá hạn', value: 'expired', color: '#ffbf00' }
    ],
    field: 'state',
    valueField: 'value',
    textField: 'text',
    colorField: 'color'
  }];


  ngOnInit() {
    this.updateExpiredAppointment();
    this.showSchedulerDefault();
    this.searchChange();
    // this.updateTime();
  }

  searchChange() {
    this.searchCustomerUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadByView();
      });

    this.searchDoctorUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadByView();
      });

    this.searchAppointUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadByView();
      });
  }

  changeDateRangeFilter() {
    this.fromDateList = new Date(this.formFilter.get('frDate').value);
    this.toDateList = new Date(this.formFilter.get('toDate').value);
    var frDate = formatDate(new Date(this.formFilter.get('frDate').value), 'g', 'en-US');
    var toDate = formatDate(new Date(this.formFilter.get('toDate').value), 'g', 'en-US');
    var d1 = new Date(frDate);
    var d2 = new Date(toDate);
    var diff = (d2.getTime() - d1.getTime()) / (3600 * 24000);
    this.schedulerConfig.numberOfDays = diff;

    clearTimeout(this.stopTypingTimer);
    this.stopTypingTimer = setTimeout(() => {
      this.loadAppointmentList();
    }, 300);
  }

  checkState() {
    this.confirmed = this.formFilter.get('confirmed').value;
    this.cancel = this.formFilter.get('cancel').value;
    this.done = this.formFilter.get('done').value;
    this.expired = this.formFilter.get('expired').value;
    this.waiting = this.formFilter.get('waiting').value;
    this.stopTypingTimer = setTimeout(() => {
      this.loadByView();
    }, 300);
  }

  getState(state: string) {
    switch (state) {
      case 'done':
        return 'Kết thúc';
      case 'cancel':
        return 'Đã hủy';
      case 'waiting':
        return 'Đang chờ';
      case 'expired':
        return 'Quá hạn';
      default:
        return 'Đang hẹn';
    }
  }

  changeDateRangeScheduler(e) {
    this.fromDateScheduler = new Date(e.dateRange.start);
    this.toDateScheduler = new Date(e.dateRange.end);
    this.loadDataToScheduler();
  }

  loadAppointmentList() {
    this.loading = true;
    var apnPaged = this.getFilterValue();
    apnPaged.limit = 20;
    apnPaged.searchDoctor = this.searchDoctor;
    apnPaged.searchAppointment = this.searchAppoint;
    apnPaged.searchCustomer = this.searchCustomer;
    this.service.loadAppointmentList(apnPaged).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridView = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  loadDataToScheduler() {
    var appointmentBasicList = new Array<AppointmentBasic>();
    var apnPaged = this.getFilterValue();
    apnPaged.limit = 0;
    apnPaged.searchDoctor = this.searchDoctor;
    apnPaged.searchAppointment = this.searchAppoint;
    apnPaged.searchCustomer = this.searchCustomer;
    this.service.loadAppointmentList(apnPaged).subscribe(
      rs => {
        appointmentBasicList = rs['items'] as AppointmentBasic[];
        this.events = appointmentBasicList.map(rs1 => (<SchedulerEvent>{
          id: rs1.id,
          isAllDay: false,
          start: new Date(rs1.date),
          end: new Date(rs1.date),
          title: rs1.name,
          description: 'KH: ' + rs1.partner.name + ' - BS: ' + rs1.user.name,
          state: rs1.state,
          partnerId: rs1.partnerId
        })
        );
      }
    )
  }

  getFilterValue(): AppointmentPaged {
    var appointmentPaged = new AppointmentPaged();
    var stateList = new Array<string>();

    if (this.confirmed) {
      stateList.push('confirmed');
    }
    if (this.cancel) {
      stateList.push('cancel');
    }
    if (this.done) {
      stateList.push('done');
    }
    if (this.expired) {
      stateList.push('expired');
    }
    if (this.waiting) {
      stateList.push('waiting');
    }

    appointmentPaged = this.formFilter.value;
    appointmentPaged.offset = this.skip;
    appointmentPaged.dateTimeFrom = this.intlService.formatDate(this.schedulerShow ? this.fromDateScheduler : this.fromDateList, 'g', 'en-US');
    appointmentPaged.dateTimeTo = this.intlService.formatDate(this.schedulerShow ? this.toDateScheduler : this.toDateList, 'g', 'en-US');
    appointmentPaged.state = stateList.join(',');
    return appointmentPaged;
  }

  //Xác định các lịch hẹn quá hạn
  handleExpiredAppointments() {
    var today = new Date();
    var yesterday = new Date();
    yesterday.setDate(today.getDate() - 1);


  }

  loadFullWeek() {
    var today = new Date();
    var day = today.getDay();
    this.fromDateList = new Date(today.getFullYear(), today.getMonth(), today.getDate() - (day - 1));
    this.toDateList = new Date(today.getFullYear(), today.getMonth(), today.getDate() + (7 - day), 24);
    this.formFilter.get('frDate').setValue(this.fromDateList);
    this.formFilter.get('toDate').setValue(this.toDateList);
  }

  showSchedulerDefault() {
    this.loadDataToScheduler();
    this.configScheduler();
    this.schedulerShow = true;
  }

  showGridView() {
    this.loadAppointmentList();
    this.schedulerShow = false;
    this.loadFullWeek();
  }

  eventDblClick(e) {
    if (e.event.dataItem.state == 'confirmed' || 'waiting') {
      this.openWindow(e.event.id, null);
    }
  }

  slotDblClick(e) {
    var today = new Date();
    if (e.start >= today) {
      this.openWindow(null, e.start);
    }
  }

  searching() {
    debounceTime(400),
      distinctUntilChanged(),
      this.loadByView();
  }

  loadByView() {
    if (this.schedulerShow) {
      this.loadDataToScheduler();
    }
    else {
      this.loadAppointmentList();
    }
  }

  cancelOrRemoveEvent(e) {
    console.log(e);
    this.deleteAppointment(e.event.id, e.event.state);
  }

  openWindow(id: string, time: Date) {
    const windowRef: WindowRef = this.windowService.open(
      {
        title: 'Lịch hẹn',
        content: AppointmentCreateUpdateComponent,
        resizable: false,
        width: 930
      });
    this.windowOpened = true;

    const instance = windowRef.content.instance;
    instance.appointId = id;
    instance.timeConfig = time;

    windowRef.result.subscribe(
      (result) => {
        this.windowOpened = false;
        console.log(result);
        console.log(result instanceof WindowCloseResult);
        if (!(result instanceof WindowCloseResult)) {
          this.loadByView();
        }
      }
    )
  }

  dragEvent(e) {
    console.clear();
    console.log(e.end);
    if (e.dataItem.state.toString() == "confirmed" || e.dataItem.state.toString() == "expired") {
      var today = new Date();
      if ((e.end.toString() !== e.event.start.toString()) && e.end >= today) {
        const dialogRef: DialogRef = this.dialogService.open({
          title: 'Đổi lịch hẹn',
          content: 'Bạn chắc chắn muốn thay đổi lịch hẹn này ?',
          width: 450,
          height: 200,
          minWidth: 250,
          actions: [
            { text: 'Hủy', value: false },
            { text: 'Đồng ý', primary: true, value: true }
          ]
        });
        dialogRef.result.subscribe(
          rs => {
            if (!(rs instanceof DialogCloseResult)) {
              if (rs['value']) {
                var appPatch = new AppointmentPatch();
                var ar = [];

                appPatch.date = this.intlService.formatDate(e.end, 'g', 'en-US');
                if (e.dataItem.state.toString() == "expired") {
                  appPatch.state = "confirmed";
                }

                for (var p in appPatch) {
                  var o = { op: 'replace', path: '/' + p, value: appPatch[p] };
                  ar.push(o);
                }

                this.service.patch(e.event.id, ar).subscribe(
                  () => {
                    this.notificationService.show({
                      content: 'Cập nhật thành công',
                      hideAfter: 3000,
                      position: { horizontal: 'center', vertical: 'top' },
                      animation: { type: 'fade', duration: 400 },
                      type: { style: 'success', icon: true }
                    });
                    this.loadDataToScheduler();
                  },
                  er => {
                    console.log(er);
                  }
                )//subscr create                  
              }
            }//if ! DialogCloseResult
          }
        )
      } else if (e.end < today) {
        this.notificationService.show({
          content: 'Không thể dời lịch hẹn về thời gian này',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'error', icon: true }
        });
      }
    }
  }

  updateOnly(id, state) {
    var apnPatch = new AppointmentPatch;
    var ar = [];
    apnPatch.id = id;
    apnPatch.state = state;
    for (var p in apnPatch) {
      var o = { op: 'replace', path: '/' + p, value: apnPatch[p] };
      ar.push(o);
    }
    this.service.patch(id, ar).subscribe(
      rs => {
        this.loadAppointmentList();
      }
    )
  }

  deleteAppointment(id, state) {
    const dialogRef: DialogRef = this.dialogService.open({
      title: 'Xóa cuộc hẹn',
      content: 'Bạn chắc chắn muốn xóa cuộc hẹn này ?',
      width: 450,
      height: 200,
      minWidth: 250,
      actions: [
        { text: 'Thoát', value: false },
        { text: 'Đồng ý', primary: true, value: true }
      ]
    });
    dialogRef.result.subscribe(
      rs => {
        console.log(rs);
        if (!(rs instanceof DialogCloseResult)) {
          if (rs['value']) {
            this.service.removeAppointment(id).subscribe(
              () => {
                this.loadByView();
              }
            );
          }
        }
      }
    )
  }

  openCustomerInfo(cusId: string) {
    const dialogRef: DialogRef = this.dialogService.open({
      title: 'Thông tin Khách hàng',
      content: PartnerInfoComponent,
      width: 700,
      height: 500,
      minWidth: 250,
      actions: [
        { text: 'Đóng', value: false },
        { text: 'Tạo lịch hẹn', primary: true, value: true }
      ]
    });

    this.customerInfoOpened = true;
    const instance = dialogRef.content.instance;
    instance.partnerId = cusId;
  }

  //Setting thời gian cho scheduler
  configScheduler() {
    this.schedulerConfig = new SchedulerConfig();
    this.schedulerConfig.startDisplay = '06:00';
    this.schedulerConfig.endDisplay = '22:00';
    this.schedulerConfig.selectedDate = new Date();
    this.schedulerConfig.indexViewNum = 1;

    this.schedulerConfig.slotFillDay = 0.3;
    this.schedulerConfig.slotFillWeek = 1;
    this.schedulerConfig.slotDivisions = 2;
    this.schedulerConfig.slotDuration = 30;


    this.schedulerConfig.numberOfDays = 10;

    //Thời gian làm việc
    //8h-17h30
    this.schedulerConfig.workDayStart = '08:00';
    this.schedulerConfig.workDayEnd = '17:30';

    ////thứ 2-thứ 7
    this.schedulerConfig.workWeekStart = 1;
    this.schedulerConfig.workWeekEnd = 6;
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadAppointmentList();
  }

  //Cập nhật trạng thái của các lịch hẹn quá hạn
  updateExpiredAppointment() {
    this.service.patchMulti().subscribe(
      rs => {
        this.loadByView();
      }
    );
  }



  // timeCount: Date = new Date;
  // updateTime() {
  //   setInterval(() => {
  //     this.timeCount = new Date;
  //     var ar = 
  //     ar.forEach(element => {

  //     });
  //   }, 1000
  //   )
  // }
}
