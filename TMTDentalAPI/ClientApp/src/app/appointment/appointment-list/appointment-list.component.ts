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
import { EmployeeInfoComponent } from 'src/app/employees/employee-info/employee-info.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-appointment-list',
  templateUrl: './appointment-list.component.html',
  styleUrls: ['./appointment-list.component.css']
})
export class AppointmentListComponent implements OnInit {

  constructor(private service: AppointmentService, private dialogService: DialogService, private intlService: IntlService,
    private notificationService: NotificationService, private modalService: NgbModal) { }

  gridView: GridDataResult;
  windowOpened: boolean;
  customerInfoOpened: boolean;
  ab: AppointmentBasic[];
  limit = 20;
  loading = false;
  skip = 0;
  stopTypingTimer: any;
  schedulerConfig: SchedulerConfig;
  scrollTime: string;

  schedulerShow: boolean;

  fromDateList: Date;
  toDateList: Date;
  fromDateScheduler: Date;
  toDateScheduler: Date;

  search: string;
  // searchDoctor: string;
  // searchAppoint: string;
  searchUpdate = new Subject<string>();
  // searchDoctorUpdate = new Subject<string>();
  // searchAppointUpdate = new Subject<string>();

  confirmed: boolean;
  cancel: boolean;
  done: boolean;
  expired: boolean;
  waiting: boolean;

  formFilter = new FormGroup({
    search: new FormControl(),
    // searchDoctor: new FormControl(),
    // searchAppoint: new FormControl(),
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
      { text: 'Hoàn tất', value: 'done', color: '#666666' },
      { text: 'Đang chờ', value: 'waiting', color: '#0080ff' },
      { text: 'Quá hạn', value: 'expired', color: '#ffbf00' }
    ],
    field: 'state',
    valueField: 'value',
    textField: 'text',
    colorField: 'color'
  }];


  ngOnInit() {
    // this.updateExpiredAppointment();
    this.showGridView();
    this.searchChange();
    // this.updateTime();
  }

  searchChange() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadByView();
      });

    // this.searchDoctorUpdate.pipe(
    //   debounceTime(400),
    //   distinctUntilChanged())
    //   .subscribe(value => {
    //     this.loadByView();
    //   });

    // this.searchAppointUpdate.pipe(
    //   debounceTime(400),
    //   distinctUntilChanged())
    //   .subscribe(value => {
    //     this.loadByView();
    //   });
  }

  changeDateRangeFilter() {
    this.fromDateList = new Date(this.formFilter.get('frDate').value);
    this.toDateList = new Date(this.formFilter.get('toDate').value);
    var frDate = formatDate(new Date(this.formFilter.get('frDate').value), 'g', 'en-US');
    var toDate = formatDate(new Date(this.formFilter.get('toDate').value), 'g', 'en-US');
    var d1 = new Date(frDate);
    var d2 = new Date(toDate);
    var diff = (d2.getTime() - d1.getTime()) / (3600 * 24000);
    // this.schedulerConfig.numberOfDays = diff;

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
        return 'Hoàn tất';
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
    if ((e.dateRange.start.getFullYear() == e.dateRange.end.getFullYear()) && (e.dateRange.start.getMonth() == e.dateRange.end.getMonth())) {
      this.fontSize();
    }
    this.loadDataToScheduler();
  }

  loadAppointmentList() {
    this.loading = true;
    var apnPaged = this.getFilterValue();
    apnPaged.limit = 20;
    apnPaged.search = this.search;
    // apnPaged.searchAppointment = this.searchAppoint;
    // apnPaged.searchCustomer = this.searchCustomer;
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
    apnPaged.search = this.search;
    // apnPaged.searchAppointment = this.searchAppoint;
    // apnPaged.searchCustomer = this.searchCustomer;
    this.service.loadAppointmentList(apnPaged).subscribe(
      rs => {
        appointmentBasicList = rs['items'] as AppointmentBasic[];
        // this.setScrollTime(appointmentBasicList);
        this.events = appointmentBasicList.map(rs1 => (<SchedulerEvent>{
          id: rs1.id,
          isAllDay: false,
          start: new Date(rs1.date),
          end: new Date(rs1.date),
          title: rs1.partner.name,
          description: rs1.doctor ? 'BS. ' + rs1.doctor.name : '',
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

  loadToday() {
    var today = new Date();
    this.fromDateList = new Date(today.getFullYear(), today.getMonth(), today.getDate());
    this.toDateList = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 23, 59, 59);
    this.formFilter.get('frDate').setValue(this.fromDateList);
    this.formFilter.get('toDate').setValue(this.toDateList);
    console.log(this.fromDateList);
    console.log(this.toDateList);
  }

  showSchedulerDefault() {
    this.loadDataToScheduler();
    this.configScheduler();
    this.schedulerShow = true;
  }

  showGridView() {
    this.loadToday();
    // this.loadAppointmentList();
    this.schedulerShow = false;
  }

  eventDblClick(e) {
    if (e.event.dataItem.state == 'confirmed' || 'waiting') {
      this.openModal(e.event.id, null);
    }
  }

  slotDblClick(e) {
    var today = new Date();
    if (e.start >= today) {
      this.openModal(null, e.start);
    } else {
      this.notificationService.show({
        content: 'Không thể tạo lịch hẹn trong thời gian này',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
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

  // openWindow(id: string, time: Date) {
  //   const windowRef: WindowRef = this.windowService.open(
  //     {
  //       title: 'Lịch hẹn',
  //       content: AppointmentCreateUpdateComponent,
  //       resizable: false,
  //       width: 930
  //     });
  //   this.windowOpened = true;

  //   const instance = windowRef.content.instance;
  //   instance.appointId = id;
  //   instance.timeConfig = time;

  //   windowRef.result.subscribe(
  //     (result) => {
  //       this.windowOpened = false;
  //       console.log(result);
  //       console.log(result instanceof WindowCloseResult);
  //       if (!(result instanceof WindowCloseResult)) {
  //         this.loadByView();
  //       }
  //     }
  //   )
  // }

  openModal(id: string, time: Date) {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.appointId = id;
    modalRef.componentInstance.timeConfig = time;
    modalRef.result.then(
      rs => {
        this.loadByView();
      },
      er => { }
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
          content: 'Không thể di chuyển lịch hẹn đến thời gian này',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'error', icon: true }
        });
      }
    } else {
      this.notificationService.show({
        content: 'Không thể di chuyển lịch hẹn này',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
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
      height: 620,
      actions: [
        { text: 'Đóng', value: false },
        // { text: 'Tạo lịch hẹn', primary: true, value: true }
      ]
    });

    this.customerInfoOpened = true;
    const instance = dialogRef.content.instance;
    instance.partnerId = cusId;
  }

  openEmployeeInfo(empId: string) {
    const dialogRef: DialogRef = this.dialogService.open({
      title: 'Thông tin Nhân viên',
      content: EmployeeInfoComponent,
      width: 700,
      height: 600,
      minWidth: 250,
      actions: [
        { text: 'Đóng', value: false },
        // { text: 'Tạo lịch hẹn', primary: true, value: true }
      ]
    });

    this.customerInfoOpened = true;
    const instance = dialogRef.content.instance;
    instance.id = empId;
  }

  // setScrollTime(list: AppointmentBasic[]) {
  //   var time = new Date(list.sort((x, y) => parseInt(y.date) - parseInt(x.date))[0].date);
  //   this.scrollTime = time.getHours() + ':' + time.getMinutes();
  //   console.log(this.scrollTime);
  // }

  fontSize() {
    if (this.fromDateScheduler.getDate() + 1 != this.toDateScheduler.getDate()) {
      return "0.7";
    } else {
      return "1";
    }
  }

  //Setting thời gian cho scheduler
  configScheduler() {
    this.schedulerConfig = new SchedulerConfig();
    //Khoảng thời gian hiển thị
    this.schedulerConfig.startDisplay = '07:30';
    this.schedulerConfig.endDisplay = '21:00';
    this.schedulerConfig.selectedDate = new Date();
    //0:Ngày - 1:Tuần - 2:Tháng
    this.schedulerConfig.indexViewNum = 0;

    this.schedulerConfig.slotFillDay = 0.3;
    this.schedulerConfig.slotFillWeek = 1;
    this.schedulerConfig.slotDivisions = 2;
    this.schedulerConfig.slotDuration = 30;
    this.schedulerConfig.scrollTime = this.scrollTime;
    // this.schedulerConfig.numberOfDays = 10;

    //Thời gian làm việc
    //8h-17h30
    this.schedulerConfig.workDayStart = '08:00';
    this.schedulerConfig.workDayEnd = '20:30';

    //thứ 2-thứ 7
    this.schedulerConfig.workWeekStart = 1;
    this.schedulerConfig.workWeekEnd = 6;
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadAppointmentList();
  }

  //Cập nhật trạng thái của các lịch hẹn quá hạn
  // updateExpiredAppointment() {
  //   this.service.patchMulti(2).subscribe(
  //     rs => { }
  //   );
  // }



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
