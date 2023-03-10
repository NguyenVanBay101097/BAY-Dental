import { Component, OnInit } from '@angular/core';
import { AppointmentService } from '../appointment.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { AppointmentBasic, AppointmentPaged, SchedulerConfig, ApplicationUserSimple, AppointmentPatch, AppointmentSearch } from '../appointment';
import { WindowRef, WindowService, WindowCloseResult, DialogService, DialogRef, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { FormGroup, FormControl } from '@angular/forms';
import { formatDate } from '@angular/common';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { NotificationService } from '@progress/kendo-angular-notification';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AppointmentVMService } from '../appointment-vm.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { AppointmentCreateUpdateComponent } from 'src/app/shared/appointment-create-update/appointment-create-update.component';

@Component({
  selector: 'app-appointment-list',
  templateUrl: './appointment-list.component.html',
  styleUrls: ['./appointment-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  },
  providers: [
    AppointmentVMService
  ]
})
export class AppointmentListComponent implements OnInit {

  constructor(private service: AppointmentService, private dialogService: DialogService, private intlService: IntlService,
    private notificationService: NotificationService, private modalService: NgbModal,
    public appointmentVMService: AppointmentVMService) { }

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

  public weekStart: Date = new Date(new Date().setDate(new Date().getDate() - new Date().getDay() + (new Date().getDay() == 0 ? -6 : 1)));
  public weekEnd: Date = new Date(new Date().setDate(new Date().getDate() - new Date().getDay() + (new Date().getDay() == 0 ? -6 : 1) + 6));
  public today: Date = new Date(new Date().toDateString());
  public next3days: Date = new Date(new Date(new Date().setDate(new Date().getDate() + 3)).toDateString());

  search: string;
  stateFilter: string;
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


  events: any;
  resources: any[] = [{
    name: 'State',
    data: [
      { text: '??ang h???n', value: 'confirmed', color: '#04c835' },
      { text: 'H???y h???n', value: 'cancel', color: '#cc0000' },
      { text: 'Ho??n t???t', value: 'done', color: '#666666' },
      { text: '??ang ch???', value: 'waiting', color: '#0080ff' },
      { text: 'Qu?? h???n', value: 'expired', color: '#ffbf00' }
    ],
    field: 'state',
    valueField: 'value',
    textField: 'text',
    colorField: 'color'
  }];


  ngOnInit() {
    // this.updateExpiredAppointment();
    // this.showGridView();
    // this.searchChange();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadAppointments();
      });

    this.appointmentVMService.filter.subscribe(data => {
      this.fromDateList = data.start;
      this.toDateList = data.end;
      this.loadAppointments();
    });

    this.appointmentVMService.eventEdit.subscribe(id => {
      this.openModal(id, null);
    });

    this.appointmentVMService.eventDelete.subscribe(id => {
      this.deleteAppointment2(id);
    });

    this.fromDateList = this.today;
    this.toDateList = this.next3days;
    this.appointmentVMService.setDateRange(this.fromDateList, this.toDateList);
  }

  loadAppointments() {
    var val = new AppointmentSearch();
    if (this.search) {
      val.search = this.search;
    }
    if (this.fromDateList) {
      val.dateTimeFrom = this.intlService.formatDate(this.fromDateList, 'd', 'en-US');
    }
    if (this.toDateList) {
      val.dateTimeTo = this.intlService.formatDate(this.toDateList, 'd', 'en-US');
    }

    if (this.stateFilter) {
      val.state = this.stateFilter;
    }

    this.appointmentVMService.query(val);
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

  onAdvanceSearchChange(filter) {
    this.fromDateList = filter.dateFrom;
    this.toDateList = filter.dateTo;
    this.confirmed = filter.confirmedState;
    this.done = filter.doneState;
    this.cancel = filter.cancelState;
    this.loadByView();
  }

  onDateSearchChange(filter) {
    this.fromDateList = filter.dateFrom;
    this.toDateList = filter.dateTo;
    this.appointmentVMService.setDateRange(this.fromDateList, this.toDateList);
    // this.loadAppointments();
  }

  onStateSearchChange(state) {
    this.stateFilter = state;
    this.appointmentVMService.setState(state);
  }

  changeDateRangeFilter() {
    this.fromDateList = new Date(this.formFilter.get('frDate').value);
    this.toDateList = new Date(this.formFilter.get('toDate').value);
    var frDate = formatDate(new Date(this.formFilter.get('frDate').value), 'd', 'en-US');
    var toDate = formatDate(new Date(this.formFilter.get('toDate').value), 'd', 'en-US');
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
        return '???? t???i';
      case 'cancel':
        return 'H???y h???n';
      default:
        return '??ang h???n';
    }
  }

  refreshData() {
    this.appointmentVMService.refreshData();
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
        this.events = appointmentBasicList.map(rs1 => (<any>{
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
    appointmentPaged.dateTimeFrom = this.intlService.formatDate(this.schedulerShow ? this.fromDateScheduler : this.fromDateList, 'd', 'en-US');
    appointmentPaged.dateTimeTo = this.intlService.formatDate(this.schedulerShow ? this.toDateScheduler : this.toDateList, 'd', 'en-US');
    appointmentPaged.state = stateList.join(',');
    return appointmentPaged;
  }

  //X??c ?????nh c??c l???ch h???n qu?? h???n
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
    // this.loadToday();
    this.loadAppointmentList();
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
        content: 'Kh??ng th??? t???o l???ch h???n trong th???i gian n??y',
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
  //       title: 'L???ch h???n',
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
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.appointId = id;
    modalRef.componentInstance.timeConfig = time;
    modalRef.result.then(
      rs => {
        if (rs) {
          this.appointmentVMService.announceApCreate(rs.id);
        }
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
          title: '?????i l???ch h???n',
          content: 'B???n ch???c ch???n mu???n thay ?????i l???ch h???n n??y ?',
          width: 450,
          height: 200,
          minWidth: 250,
          actions: [
            { text: 'H???y', value: false },
            { text: '?????ng ??', primary: true, value: true }
          ]
        });
        dialogRef.result.subscribe(
          rs => {
            if (!(rs instanceof DialogCloseResult)) {
              if (rs['value']) {
                var appPatch = new AppointmentPatch();
                var ar = [];

                appPatch.date = this.intlService.formatDate(e.end, 'd', 'en-US');
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
                      content: 'C???p nh???t th??nh c??ng',
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
          content: 'Kh??ng th??? di chuy???n l???ch h???n ?????n th???i gian n??y',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'error', icon: true }
        });
      }
    } else {
      this.notificationService.show({
        content: 'Kh??ng th??? di chuy???n l???ch h???n n??y',
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

  deleteAppointment2(id) {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'X??a l???ch h???n';
    modalRef.componentInstance.body = 'B???n ch???c ch???n mu???n x??a l???ch h???n n??y?';
    modalRef.result.then(() => {
      this.service.removeAppointment(id).subscribe(() => {
        this.loadAppointments();
      });
    });
  }

  deleteAppointment(id, state) {
    const dialogRef: DialogRef = this.dialogService.open({
      title: 'X??a cu???c h???n',
      content: 'B???n ch???c ch???n mu???n x??a cu???c h???n n??y ?',
      width: 450,
      height: 200,
      minWidth: 250,
      actions: [
        { text: 'Tho??t', value: false },
        { text: '?????ng ??', primary: true, value: true }
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
  }

  openEmployeeInfo(empId: string) {
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

  //Setting th???i gian cho scheduler
  configScheduler() {
    this.schedulerConfig = new SchedulerConfig();
    //Kho???ng th???i gian hi???n th???
    this.schedulerConfig.startDisplay = '07:30';
    this.schedulerConfig.endDisplay = '21:00';
    this.schedulerConfig.selectedDate = new Date();
    //0:Ng??y - 1:Tu???n - 2:Th??ng
    this.schedulerConfig.indexViewNum = 0;

    this.schedulerConfig.slotFillDay = 0.3;
    this.schedulerConfig.slotFillWeek = 1;
    this.schedulerConfig.slotDivisions = 2;
    this.schedulerConfig.slotDuration = 30;
    this.schedulerConfig.scrollTime = this.scrollTime;
    // this.schedulerConfig.numberOfDays = 10;

    //Th???i gian l??m vi???c
    //8h-17h30
    this.schedulerConfig.workDayStart = '08:00';
    this.schedulerConfig.workDayEnd = '20:30';

    //th??? 2-th??? 7
    this.schedulerConfig.workWeekStart = 1;
    this.schedulerConfig.workWeekEnd = 6;
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadAppointmentList();
  }

  //C???p nh???t tr???ng th??i c???a c??c l???ch h???n qu?? h???n
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
