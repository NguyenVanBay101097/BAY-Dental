import { Component, OnInit, Input, ViewChild, ElementRef, ViewChildren, AfterViewInit } from '@angular/core';
import { AppointmentVMService } from '../appointment-vm.service';
import { AppointmentService } from '../appointment.service';
import { forkJoin, Subject } from 'rxjs';
import { AppointmentSearchByDate, AppointmentBasic, AppointmentPaged } from '../appointment';
import { IntlService } from '@progress/kendo-angular-intl';
import { PagedResult2 } from 'src/app/core/paged-result-2';
import * as _ from 'lodash';
import { NgbModal, NgbDropdownToggle } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { debounceTime, distinctUntilChanged, tap, switchMap, map } from 'rxjs/operators';
import { DotKhamService } from 'src/app/dot-khams/dot-kham.service';
import { DotkhamEntitySearchBy, DotKhamPaged } from 'src/app/dot-khams/dot-khams';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Router } from '@angular/router';
import { SaleOrderCreateDotKhamDialogComponent } from 'src/app/sale-orders/sale-order-create-dot-kham-dialog/sale-order-create-dot-kham-dialog.component';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { UserSimple } from 'src/app/users/user-simple';
import { UserPaged, UserService } from 'src/app/users/user.service';
import { AppointmentCreateUpdateComponent } from 'src/app/shared/appointment-create-update/appointment-create-update.component';
import { EmployeeBasic, EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { RevenueTimeReportPar } from 'src/app/account-invoice-reports/account-invoice-report.service';
//
import { Calendar, EventInput } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGrigPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import { FullCalendarComponent } from '@fullcalendar/angular';
import { DateRangeInput } from '@fullcalendar/core/datelib/date-range';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { MyDateRange } from '../my-date-range';
@Component({
  selector: 'app-appointment-kanban',
  templateUrl: './appointment-kanban.component.html',
  styleUrls: ['./appointment-kanban.component.css'],
  host: { 'class': 'h-100' }
})
export class AppointmentKanbanComponent implements OnInit {
  @ViewChild('dropdownMenuBtn', { static: false }) dropdownMenuBtn: NgbDropdownToggle;
  dateFrom: Date;
  dateTo: Date;
  state: string;
  userId: string;
  search: string;
  searchUpdate = new Subject<string>();

  showDropdown = false;
  dateList: Date[];

  // permission
  canAppointmentCreate = this.checkPermissionService.check(["Basic.Appointment.Create"]);
  canAppointmentEdit = this.checkPermissionService.check(["Basic.Appointment.Update"]);
  canAppointmentDelete = this.checkPermissionService.check(["Basic.Appointment.Delete"]);
  canEmployeeRead = this.checkPermissionService.check(["Catalog.Employee.Read"]);
  canCustomerLink = this.checkPermissionService.check(["Basic.Partner.Read"]);

  // public today: Date = new Date(new Date().toDateString());
  // public next3days: Date = new Date(new Date(new Date().setDate(new Date().getDate() + 3)).toDateString());

  appointmentByDate: { [id: string]: AppointmentBasic[]; } = {};

  states: { text: string, value: string, bgColor?: string }[] = [
    { text: 'Tất cả', value: '', bgColor: '' },
    { text: 'Đang hẹn', value: 'confirmed', bgColor: '#007BFF' },
    // { text: 'Chờ khám', value: 'waiting', class: 'text-warning' },
    // { text: 'Đang khám', value: 'examination', class: 'text-info' },
    // { text: 'Hoàn thành', value: 'done', class: 'text-success' },
    { text: 'Đã đến', value: 'arrived', bgColor: '#28A745' },
    { text: 'Hủy hẹn', value: 'cancel', bgColor: '#EB3B5B' },
    { text: 'Quá hạn', value: 'overdue', bgColor: '#FFC107' }
  ];

  stateSelected: string = this.states[0].value;
  listEmployees: EmployeeBasic[] = [];
  employeeSelected: string = '';

  viewKanban: string = "th-large"; // "th-large", "list-ul"
  gridData: GridDataResult;
  limit: number = 20;
  offset: number = 0
  loading: boolean = false;
  // events = [];
  // calendarApi: Calendar;
  // calendarPlugins = [dayGridPlugin, timeGrigPlugin, interactionPlugin];
  // @ViewChild('fullcalendar',{static: true}) calendarComponent: FullCalendarComponent;
  // titleDateToolbar: string = "";
  // viewToolbar: string = "timeGridWeek"; // "timeGridDay", "timeGridWeek", "dayGridMonth"
  // validRange: DateRangeInput;
  listData = [];
  doctors = [];
  // New Calendar //
  calendarTableEl = null;
  calendarTheadEl = null;
  calendarTbodyEl = null;
  // Dialog
  contDialogCalendarEl = null;
  selectStatusEl = null;
  inputCustomerNameEl = null;
  inputCustomerPhoneEl = null;
  inputReferrerNameEl = null;

  today = new Date();
  todayFormat = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate(), 0, 0, 0);
  currentDay = 0;
  currentWeek = 0;
  currentMonth = this.todayFormat.getMonth();
  currentYear = this.todayFormat.getFullYear();

  daysOfWeek = ["Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật"];
  // months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]; 
  months = ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"];

  timePeriod = 'month';
  firstTime = 6; // format 24h : 0h - 23h
  lastTime = 23; // format 24h : 0h - 23h
  dateAppointmentFormat = null; // format "year month day hour"
  appointmentIDChoose = null;
  dataAppointmentsGrouped = null;

  // dataAppointments: any[] = [
  //   {
  //     id: 0,
  //     date: new Date(2021, 7, 5, 8, 30, 5),
  //     state: 'confirmed',
  //     partnerName: 'Thomas K. Wilson',
  //     partnerPhone: 1909123123,
  //     doctorName: 'Wendy R. Sherman'
  //   },
  //   {
  //     id: 1,
  //     date: new Date(2021, 7, 4, 8, 15),
  //     state: 'arrived',
  //     partnerName: 'Jeff Bezos',
  //     partnerPhone: 1909123123,
  //     doctorName: 'Anne Mulcahy'
  //   },
  //   {
  //     id: 2,
  //     date: new Date(2021, 7, 2, 8),
  //     state: 'overdue',
  //     partnerName: 'Thomas K. Wilson',
  //     partnerPhone: 1909123123,
  //     doctorName: 'Anne Mulcahy'
  //   },
  //   {
  //     id: 3,
  //     date: new Date(2021, 7, 1, 8, 30),
  //     state: 'arrived',
  //     partnerName: 'Jeff Bezos',
  //     partnerPhone: 1909123123,
  //     doctorName: 'Thomas K. Wilson'
  //   },
  //   {
  //     id: 4,
  //     date: new Date(2021, 7, 4, 8, 45, 10),
  //     state: 'cancel',
  //     partnerName: 'Tom Holland',
  //     partnerPhone: 1909123123,
  //     doctorName: 'Barbara Conan'
  //   },
  //   {
  //     id: 5,
  //     date: new Date(2021, 7, 9, 8),
  //     state: 'confirmed',
  //     partnerName: 'Uzumaki Naruto',
  //     partnerPhone: 1909123123,
  //     doctorName: 'Uchiha Sasuke'
  //   }
  // ];

  dataAppointments: any[] = [];
  titleToolbar = "";

  constructor(
    private appointmentService: AppointmentService,
    private intlService: IntlService,
    private modalService: NgbModal,
    private dotkhamService: DotKhamService,
    private notificationService: NotificationService,
    private router: Router,
    private employeeService: EmployeeService,
    private checkPermissionService: CheckPermissionService,
    private elementRef: ElementRef
  ) { }
  ngAfterViewInit(): void {
    //Called after ngAfterContentInit when the component's view has been initialized. Applies to components only.
    //Add 'implements AfterViewInit' to the class.
    // this.calendarApi = this.calendarComponent.getApi();
    // this.calendarApi.changeView(this.viewToolbar);
    // this.titleDateToolbar = this.getDateToolbar();
    this.loadGridData();
    // console.log(this.calendarApi.view.activeStart);
    // console.log(this.calendarApi.view.activeEnd);

    //     document.getElementsByClassName("fc-next-button")[0].addEventListener("click",()=> {

    //     });

    // New Calendar //
    this.getElements();
    // this.dataAppointments = this.dataAppointments.map(v => ({
    //   ...v,
    //   dateFormat: new Date(v.date).setHours(0, 0, 0, 0),
    //   dateHour: v.date.getHours()
    // }));
    // this.groupByDataAppointments(); // group by 

    // this.showCalendarMonth(this.currentYear, this.currentMonth);
  }
  ngOnInit() {
    var curr = new Date; // get current date
    var first = curr.getDate() - curr.getDay() + 1; // First day is the day of the month - the day of the week
    var last = first + 6; // last day is the first day + 6

    this.dateFrom = new Date(curr.setDate(first));
    this.dateTo = new Date(curr.setDate(last));

    // this.dateList = this.getDateList();
    this.loadData();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadData();
        this.loadGridData();
      });

    // this.loadListEmployees();
    // this.loadDoctorList();
  }

  loadDoctorList() {
    var val = {
      dateTimeFrom: this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '',
      dateTimeTo: this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : ''
    };
    this.appointmentService.getListDoctor(val).subscribe(res => {
      this.doctors = res;
    })
  }
  loadListEmployees() {
    var paged = new EmployeePaged();
    paged.isDoctor = true;
    this.employeeService.getEmployeePaged(paged).subscribe((res) => {
      this.listEmployees = res.items;
    });
  }

  onChangeEmployee(employeeId) {
    this.employeeSelected = employeeId;
    this.loadData();
    this.loadGridData();
  }

  onChangeDate(e) {
    this.dateFrom = e.dateFrom;
    this.dateTo = e.dateTo;
    this.loadData();
    this.loadGridData();
  }

  onChangeState(state) {
    this.state = state;
    this.loadData();
    this.loadGridData();
  }

  createAppointment() {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Đặt lịch hẹn";
    modalRef.result.then(result => {
      this.loadData();
      this.loadGridData();
    }, () => { });

    // modalRef.componentInstance.getBtnDeleteObs.subscribe(() => {
    //   this.loadData();
    //   this.loadGridData();
    // })
  }

  refreshData() {
    this.loadData();
    this.loadGridData();
  }

  loadData() {
    this.resetData();
    var val = new AppointmentPaged();
    val.limit = 1000;
    val.state = this.state || '';
    val.search = this.search || '';
    val.doctorId = this.employeeSelected || '';
    val.dateTimeFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
    val.dateTimeTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';

    this.appointmentService.getPaged(val).subscribe((result: any) => {
      this.addAppointments(result);
      this.listData = [];
      console.log(result.items);

      for (const item of result.items) {
        const event = {
          id: item.id,
          date: new Date(item.date),
          state: item.state,
          partnerName: item.partnerName,
          partnerId: item.partnerId,
          partnerPhone: item.partnerPhone,
          doctorName: item.doctorName
        }
        this.listData.push(event);
      }
      this.dataAppointments = [...this.listData];
      this.dataAppointments = this.dataAppointments.map(v => ({
        ...v,
        dateFormat: new Date(v.date).setHours(0, 0, 0, 0),
        dateHour: v.date.getHours()
      }));

      this.groupByDataAppointments();
      this.jump_today()
      // this.showCalendarMonth(this.currentYear, this.currentMonth);

    }, (error: any) => {
      console.log(error);
    });
  }

  resetData() {
    this.appointmentByDate = Object.assign({});
  }

  deleteAppointment(appointment: AppointmentBasic) {
    const modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa lịch hẹn';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa lịch hẹn này?';
    modalRef.result.then(() => {
      this.appointmentService.removeAppointment(appointment.id).subscribe(() => {
        var date = new Date(appointment.date);
        var key = date.toDateString();
        if (this.appointmentByDate[key]) {
          var index = _.findIndex(this.appointmentByDate[key], o => o.id == appointment.id);
          if (index != -1) {
            this.appointmentByDate[key].splice(index, 1);
          }
        }
      });
    });
  }

  editAppointment(appointment: AppointmentBasic) {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.appointId = appointment.id;
    modalRef.result.then(() => {
      this.loadData();
      this.loadGridData();
      this.appointmentService.getBasic(appointment.id).subscribe(item => {
        var date = new Date(item.date);
        var key = date.toDateString();
        if (this.appointmentByDate[key]) {
          var index = _.findIndex(this.appointmentByDate[key], o => o.id == item.id);
          if (index != -1) {
            this.appointmentByDate[key][index] = item;
          }
        }
      });
    }, () => {
    });
  }

  addAppointmentById(id) {
    this.appointmentService.getBasic(id).subscribe(item => {
      var date = new Date(item.date);
      var key = date.toDateString();
      var index = _.findIndex(this.dateList, o => o.toDateString() == date.toDateString());
      if (index == -1) {
        this.dateList.push(date);
      }

      if (!this.appointmentByDate[key]) {
        this.appointmentByDate[key] = [];
      }

      this.appointmentByDate[key].unshift(item);
    });
  }

  formatDate(date) {
    var d = new Date(date),
      month = '' + (d.getMonth() + 1),
      day = '' + d.getDate(),
      year = d.getFullYear();

    if (month.length < 2)
      month = '0' + month;
    if (day.length < 2)
      day = '0' + day;

    return [year, month, day].join('-');
  }

  addAppointments(paged: PagedResult2<AppointmentBasic>) {
    // for (var i = 0; i < paged.items.length; i++) {
    //   var item = paged.items[i];
    //   var date = new Date(item.date);
    //   var key = date.toDateString();
    //   if (!this.appointmentByDate[key]) {
    //     this.appointmentByDate[key] = [];
    //   }

    //   this.appointmentByDate[key].push(item);
    // }
    // this.events = [];
    // for (var i = 0; i < paged.items.length; i++) {
    //   var item = paged.items[i];
    //   var d = new Date();
    //   d.setHours(0, 0, 0, 0);
    //   let date = new Date(item.date);
    //   this.events = this.events.concat([
    //     <EventInput>{
    //       title: item.partnerName,
    //       date: this.intlService.formatDate(date, 'yyyy-MM-ddTHH:mm:ss'),
    //       backgroundColor: new Date(item.date) >= d ? (this.states.find(x => x.value == item.state) ? this.states.find(x => x.value == item.state).bgColor : '') : '#FFC107',
    //       id: item.id,
    //       textColor: 'white'
    //     }
    //   ]);
    // }
  }
  calendarEvents = [
    { title: 'event 1', date: '2019-04-01' }
  ];

  getDateList() {
    if (!this.dateFrom || !this.dateTo) {
      return [];
    }
    var list = [];
    var oneDay = 1000 * 60 * 60 * 24;
    var days = (this.dateTo.getTime() - this.dateFrom.getTime()) / oneDay;
    if (days > 30) {
      alert('Vui lòng xem tối đa 30 ngày để đảm bảo tốc độ phần mềm');
      return [];
    }
    for (var i = 0; i < days + 1; i++) {
      var date = new Date(this.dateFrom.toDateString());
      date.setDate(this.dateFrom.getDate() + i);
      list.push(date);
    }
    return list;
  }

  getPreviousDotKham(id) {
    var paged = new DotKhamPaged;
    paged.appointmentId = id;
    paged.limit = 1;
    this.dotkhamService.getPaged(paged).subscribe(rs => {
      if (rs.items.length) {
        this.router.navigate(['/dot-khams/edit/' + rs.items[0].id]);
      } else {
        this.notificationService.show({
          content: 'Không có đợt khám nào từ lịch hẹn này.',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'error', icon: true }
        });
      }
    });
  }

  createDotKham(id) {
    var paged = new DotKhamPaged;
    paged.appointmentId = id;
    paged.limit = 1;
    this.dotkhamService.getPaged(paged).subscribe((rs: any) => {
      if (rs.items.length) {
        console.log(rs.items);
        let modalRef = this.modalService.open(SaleOrderCreateDotKhamDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.title = 'Tạo đợt khám';
        modalRef.componentInstance.saleOrderId = rs.items[0].saleOrderId;

        modalRef.result.then(result => {
          if (result.view) {
            this.router.navigate(['/dot-khams/edit/', result.result.id]);
          } else {
          }
        }, () => {
        });
      } else {
        this.notificationService.show({
          content: 'Không có đợt khám nào từ lịch hẹn này.',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'error', icon: true }
        });
      }
    });
  }

  exportExcelFile() {
    var val = new AppointmentPaged();
    val.limit = 1000;
    val.state = this.state || '';
    val.search = this.search || '';
    val.doctorId = this.employeeSelected || '';
    val.dateTimeFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
    val.dateTimeTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';

    this.appointmentService.exportExcel(val).subscribe((result: any) => {
      let filenam = 'DanhSachLichHen';
      let newBlob = new Blob([result], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement('a');
      link.href = data;
      link.download = filenam;
      link.click();
      setTimeout(() => {
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }

  handleEventClick(e) {
    var id = e.event._def.publicId;
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.appointId = id;
    modalRef.result.then(() => {
      this.loadData();
      this.appointmentService.getBasic(id).subscribe(item => {
        var date = new Date(item.date);
        var key = date.toDateString();
        if (this.appointmentByDate[key]) {
          var index = _.findIndex(this.appointmentByDate[key], o => o.id == item.id);
          if (index != -1) {
            this.appointmentByDate[key][index] = item;
          }
        }
      });
    }, () => {
    });
  }

  handleEventRender(e) {
    //     debugger;
    //    e.el.innerHTML = '<div>fdasf</div> <br> <button class="a"   (click)="a()">ddd</button>';
    //    (e.el.getElementsByClassName('a'))[0].click(function(){
    //     console.log('a')
    //  });

  }
  // dateToday() {
  //   var currentStart = this.calendarApi.view.currentStart;
  //   var currentEnd = this.calendarApi.view.currentEnd;
  //   var today = this.calendarApi.getNow();
  //   if (today < currentStart || today > currentEnd) 
  //     this.calendarApi.today();
  //   this.titleDateToolbar = this.getDateToolbar();
  //   this.loadGridData();
  // }
  // datePrev() {
  //   this.calendarApi.prev();
  //   this.titleDateToolbar = this.getDateToolbar();
  //   this.loadGridData();
  // }
  // dateNext() {
  //   this.calendarApi.next();
  //   this.titleDateToolbar = this.getDateToolbar();
  //   this.loadGridData();
  // }
  // getDateToolbar() {
  //   var currentStart = this.calendarApi.view.currentStart;
  //   currentStart.setDate(currentStart.getDate() + 1);
  //   var currentStartString = ("0" + currentStart.getUTCDate()).slice(-2) + "/" + 
  //                           ("0" + (currentStart.getUTCMonth()+1)).slice(-2) + "/" + 
  //                           currentStart.getUTCFullYear();

  //   var currentEnd = this.calendarApi.view.currentEnd;
  //   var currentEndString = ("0" + currentEnd.getUTCDate()).slice(-2) + "/" + 
  //                         ("0" + (currentEnd.getUTCMonth()+1)).slice(-2) + "/" + 
  //                         currentEnd.getUTCFullYear();

  //   if (currentStartString == currentEndString) {
  //     return currentStartString;
  //   } else {
  //     return `${currentStartString} - ${currentEndString}`;
  //   }
  // }
  // changeViewToolbar(event) {
  //   this.viewToolbar = event;
  //   this.calendarApi.changeView(this.viewToolbar);
  //   this.titleDateToolbar = this.getDateToolbar();
  //   this.loadGridData();
  // }
  changeViewKanban(event) {
    this.viewKanban = event;
  }
  loadGridData() {
    var val = new AppointmentPaged();
    val.limit = this.limit;
    val.offset = this.offset;
    val.doctorId = this.employeeSelected || '';
    // val.dateTimeFrom
    // val.dateTimeTo

    // var currentStart = this.calendarApi.view.currentStart;
    // var currentEnd = this.calendarApi.view.currentEnd;
    // currentEnd.setDate(currentEnd.getDate() - 1);
    // if (this.dateFrom || this.dateTo) {
    //   var dateRange_1 = new MyDateRange(currentStart, currentEnd);
    //   const dateFrom_temp = this.dateFrom ? this.dateFrom : (this.dateTo >= currentStart ? currentStart : this.dateTo);
    //   const dateTo_temp = this.dateTo ? this.dateTo : (this.dateFrom <= currentEnd ? currentEnd : this.dateFrom);
    //   var dateRange_2 = new MyDateRange(dateFrom_temp, dateTo_temp);
    //   const result_intersection = this.intersectionTwoDateRange(dateRange_1, dateRange_2);
    //   if (result_intersection) {
    //     val.dateTimeFrom = this.intlService.formatDate(result_intersection.start, 'yyyy-MM-dd');
    //     val.dateTimeTo = this.intlService.formatDate(result_intersection.end, 'yyyy-MM-dd');
    //   } else {
    //     this.gridData = null;
    //     return;
    //   }
    // } else {
    //   val.dateTimeFrom = this.intlService.formatDate(currentStart, 'yyyy-MM-dd');
    //   val.dateTimeTo = this.intlService.formatDate(currentEnd, 'yyyy-MM-dd');
    // }

    this.appointmentService.getPaged(val).pipe(
      map((response: any) =>
      (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe((result) => {
      this.gridData = result;
      this.loading = false;
    }, (error) => {
      console.log(error);
      this.loading = false;
    })
  }
  pageChange(event: PageChangeEvent): void {
    this.offset = event.skip;
    this.loadGridData();
  }
  computeNameSerivc(items: any[]) {
    var serviceName = "";
    if (items && items.length > 0) {
      serviceName = items.map(x => x.name).join(', ');
    }
    return serviceName;
  }
  intersectionTwoDateRange(a: MyDateRange, b: MyDateRange) {
    var min: MyDateRange = a.start < b.start ? a : b;
    var max: MyDateRange = min == a ? b : a;
    if (min.end < max.start) return null;
    return new MyDateRange(max.start, min.end < max.end ? min.end : max.end);
  }

  // New Calendar //
  getElements() {
    this.calendarTableEl = document.getElementById('calendar-table');
    this.calendarTheadEl = document.getElementById('calendar-thead');
    this.calendarTbodyEl = document.getElementById('calendar-tbody');
    // Dialog
    this.contDialogCalendarEl = document.getElementById('cont-dialog-calendar');
    this.selectStatusEl = document.getElementById('select-status') as HTMLInputElement;
    this.inputCustomerNameEl = document.getElementById('input-customer-name') as HTMLInputElement;
    this.inputCustomerPhoneEl = document.getElementById('input-customer-phone') as HTMLInputElement;
    this.inputReferrerNameEl = document.getElementById('input-referrer-name') as HTMLInputElement;
  }

  groupByDataAppointments() {
    if (this.dataAppointments.length > 0) {
      this.dataAppointmentsGrouped = this.dataAppointments.reduce(function (r, a) {
        r[a['dateFormat']] = r[a['dateFormat']] || [];
        r[a['dateFormat']].push(a);
        return r;
      }, Object.create(null));
    }
  }

  next() {
    if (this.timePeriod === 'day') {
      this.currentDay++;
      this.showCalendarDay(this.currentYear, this.currentMonth, null, this.currentDay);
    } else if (this.timePeriod === 'week') {
      this.currentWeek++;
      this.showCalendarWeek(this.currentYear, this.currentMonth, this.currentWeek);
    } else {
      // month
      this.currentMonth = (this.currentMonth + 1) % 12;
      this.currentYear = this.currentMonth === 11 ? this.currentYear + 1 : this.currentYear;
      this.showCalendarMonth(this.currentYear, this.currentMonth);
    }
  }

  previous() {
    if (this.timePeriod === 'day') {
      this.currentDay--;
      this.showCalendarDay(this.currentYear, this.currentMonth, null, this.currentDay);
    } else if (this.timePeriod === 'week') {
      this.currentWeek--;
      this.showCalendarWeek(this.currentYear, this.currentMonth, this.currentWeek);
    } else {
      // month
      this.currentMonth = this.currentMonth === 0 ? 11 : this.currentMonth - 1;
      this.currentYear = this.currentMonth === 0 ? this.currentYear - 1 : this.currentYear;
      this.showCalendarMonth(this.currentYear, this.currentMonth);
    }
  }

  jump_today() {
    this.currentDay = 0;
    this.currentWeek = 0;
    this.currentMonth = this.todayFormat.getMonth();
    this.currentYear = this.todayFormat.getFullYear();
    if (this.timePeriod === 'day') {
      this.showCalendarDay(this.currentYear, this.currentMonth, null, null);
    } else if (this.timePeriod === 'week') {
      this.showCalendarWeek(this.currentYear, this.currentMonth, this.currentWeek);
    } else {
      // month
      this.showCalendarMonth(this.currentYear, this.currentMonth);
    }
  }

  changeTimePeriod(event) {
    // if (this.timePeriod === event) return;
    // this.timePeriod = event;
    this.jump_today();
    // console.log(event);
  }

  setTitleToolbar(firstDate = null, lastDate = null, month = null, year = null) {
    if (this.timePeriod === 'day') {
      if (firstDate !== null) {
        var firstDateString =
          ('0' + firstDate.getDate()).slice(-2) +
          '/' +
          ('0' + (firstDate.getMonth() + 1)).slice(-2) +
          '/' +
          firstDate.getFullYear();
        this.titleToolbar = firstDateString;
      }
    } else if (this.timePeriod === 'week') {
      if (firstDate !== null && lastDate !== null) {
        var firstDateString =
          ('0' + firstDate.getDate()).slice(-2) +
          '/' +
          ('0' + (firstDate.getMonth() + 1)).slice(-2) +
          '/' +
          firstDate.getFullYear();
        var lastDateString =
          ('0' + lastDate.getDate()).slice(-2) +
          '/' +
          ('0' + (lastDate.getMonth() + 1)).slice(-2) +
          '/' +
          lastDate.getFullYear();
        this.titleToolbar = `${firstDateString} - ${lastDateString}`;
      }
    } else {
      // month
      if (month !== null && year !== null) {
        this.titleToolbar = this.months[month] + ', ' + year;
      }
    }
  }

  showCalendarThead(date = null) {
    this.calendarTheadEl.textContent = '';
    let row = document.createElement('tr');
    if (this.timePeriod === 'day') {
      if (date !== null) {
        let cell = document.createElement('th');
        cell.classList.add('th-v2');
        cell.colSpan = 2;
        if (
          date.getDate() === this.todayFormat.getDate() &&
          date.getMonth() === this.todayFormat.getMonth() &&
          date.getFullYear() === this.todayFormat.getFullYear()
        ) {
          cell.innerHTML = `
                      <div class="now">${('0' + date.getDate()).slice(-2)}</div>
                      <div>${this.daysOfWeek[(date.getDay() + 6) % 7]}</div>
                  `;
        } else {
          cell.innerHTML = `
                      <div>${('0' + date.getDate()).slice(-2)}</div>
                      <div>${this.daysOfWeek[(date.getDay() + 6) % 7]}</div>
                  `;
        }
        row.appendChild(cell);
      }
    } else if (this.timePeriod === 'week') {
      if (date !== null) {
        this.daysOfWeek.forEach(el => {
          let cell = document.createElement('th');
          cell.classList.add('th-v2');
          if (
            date.getDate() === this.todayFormat.getDate() &&
            date.getMonth() === this.todayFormat.getMonth() &&
            date.getFullYear() === this.todayFormat.getFullYear()
          ) {
            cell.innerHTML = `
                          <div class="now">${('0' + date.getDate()).slice(
              -2
            )}</div>
                          <div>${el}</div>
                      `;
          } else {
            cell.innerHTML = `
                          <div>${('0' + date.getDate()).slice(-2)}</div>
                          <div>${el}</div>
                      `;
          }
          row.appendChild(cell);
          date.setDate(date.getDate() + 1);
        });
      }
    } else {
      // month
      this.daysOfWeek.forEach(el => {
        let cell = document.createElement('th');
        cell.textContent = el;
        row.appendChild(cell);
      });
    }
    this.calendarTheadEl.append(row);
  }

  addCellTimeNow(date) {
    var d = new Date();
    var h = new Date(d.getFullYear(), d.getMonth(), d.getDate(), d.getHours() + 1, 0, 0, 0);
    var e = h.getTime() - d.getTime();
    if (e > 100) {
      // some arbitrary time period
      window.setTimeout(() => {
        this.addCellTimeNow(date);
      }, e);
    }

    if (
      date.getDate() === this.todayFormat.getDate() &&
      date.getMonth() === this.todayFormat.getMonth() &&
      date.getFullYear() === this.todayFormat.getFullYear()
    ) {
      const hour = new Date().getHours();
      const cellTimePast = document.getElementById((hour - 1 < 0 ? 23 : hour - 1).toString());
      const cellTimeNow = document.getElementById(hour.toString());
      if (cellTimePast) {
        cellTimePast.classList.remove('now');
      }
      if (cellTimeNow) {
        cellTimeNow.classList.add('now');
      }
    }
  }

  showCalendarDay(year, month, day, changeDay) {
    day = day === null ? this.todayFormat.getDate() : day;
    changeDay = changeDay === null ? 0 : changeDay;
    let firstDate = new Date(year, month, day + changeDay);

    this.setTitleToolbar(firstDate);

    this.showCalendarThead(firstDate);

    this.calendarTbodyEl.innerHTML = ''; // Clear calendar-tbody
    this.calendarTableEl.style.tableLayout = 'auto';

    /// check add events
    let dataAppointmentsGroupedHour = null;
    if (this.dataAppointmentsGrouped !== null) {
      let dateFormat = new Date(year, month, day + changeDay).setHours(0);
      if (this.dataAppointmentsGrouped[dateFormat]) {
        dataAppointmentsGroupedHour = this.dataAppointmentsGrouped[dateFormat].reduce(
          function (r, a) {
            r[a.dateHour] = r[a.dateHour] || [];
            r[a.dateHour].push(a);
            return r;
          },
          Object.create(null)
        );
      }
    }

    // Create cells in calendar-tbody
    for (let i = this.firstTime; i <= this.lastTime; i++) {
      let row = document.createElement('tr');
      let cellTime = document.createElement('td');
      cellTime.classList.add('td-time');
      cellTime.id = i.toString();
      cellTime.textContent = `${i < 10 ? `0${i}` : i}:00 - ${i + 1 < 10 ? `0${i + 1}` : i + 1
        }:00`;
      let cell = document.createElement('td');
      cell.classList.add('td-day');
      cell.id = `${year}-${month < 10 ? `0${month}` : month}-${day < 10 ? `0${day}` : day
        }-${i < 10 ? `0${i}` : i}`;

      let cell_dateEvent = document.createElement('div');
      cell_dateEvent.classList.add('list-data-event-day');

      /// check add events
      if (dataAppointmentsGroupedHour && dataAppointmentsGroupedHour[i]) {
        dataAppointmentsGroupedHour[i].forEach(dataAppointment => {
          cell_dateEvent.appendChild(this.getEventDayNWeek(dataAppointment));
        });
      }

      /// td-day-overwrite
      let tdDayOverwrite = document.createElement('div');
      tdDayOverwrite.classList.add('td-day-overwrite');

      cell_dateEvent.appendChild(tdDayOverwrite);
      cell.appendChild(cell_dateEvent);
      row.appendChild(cellTime);
      row.appendChild(cell);
      this.calendarTbodyEl.appendChild(row);
    }
    this.addCellTimeNow(firstDate);
    // load addEventListener
    var btn_edit_appointment_list = this.elementRef.nativeElement.querySelectorAll('.edit-appointment');
    btn_edit_appointment_list.forEach(btn_edit_appointment => {
      btn_edit_appointment.addEventListener('click', this.showPopup.bind(this, parseInt(btn_edit_appointment.id)));
    });
    var btn_delete_appointment_list = this.elementRef.nativeElement.querySelectorAll('.delete-appointment');
    btn_delete_appointment_list.forEach(btn_delete_appointment => {
      btn_delete_appointment.addEventListener('click', this.deleteAppointment.bind(this, parseInt(btn_delete_appointment.id)));
    });
  }

  showCalendarWeek(year, month, week) {
    let firstDay = this.todayFormat.getDate() - this.todayFormat.getDay() + 1 + 7 * week; // First day is the day of the month - the day of the week
    let lastDay = firstDay + 6; // last day is the first day + 6
    let firstDate = new Date(year, month, firstDay);
    let lastDate = new Date(year, month, lastDay);

    this.setTitleToolbar(firstDate, lastDate);

    this.showCalendarThead(firstDate);

    this.calendarTbodyEl.innerHTML = ''; // Clear calendar-tbody
    this.calendarTableEl.style.tableLayout = 'fixed';

    // Create cells in calendar-tbody
    let row = document.createElement('tr');
    for (let i = 0; i < this.daysOfWeek.length; i++) {
      let cell = document.createElement('td');
      cell.classList.add('td-week');
      /// td-week click
      cell.addEventListener('click', () => {
        this.timePeriod = 'day';
        this.showCalendarDay(year, month, firstDay + i, null);
      });

      let cell_dateEvent = document.createElement('div');
      cell_dateEvent.classList.add('list-data-event-week');

      /// check add events
      if (this.dataAppointmentsGrouped !== null) {
        let dateFormat = new Date(year, month, firstDay + i).setHours(0);
        if (this.dataAppointmentsGrouped[dateFormat]) {
          this.dataAppointmentsGrouped[dateFormat].forEach(dataAppointment => {
            cell_dateEvent.appendChild(this.getEventDayNWeek(dataAppointment));
          });
        }
      }

      /// td-week-overwrite
      let tdWeekOverwrite = document.createElement('div');
      tdWeekOverwrite.classList.add('td-week-overwrite');

      cell_dateEvent.appendChild(tdWeekOverwrite);
      cell.appendChild(cell_dateEvent);
      row.appendChild(cell);
    }
    this.calendarTbodyEl.appendChild(row);
    // load addEventListener
    var btn_edit_appointment_list = this.elementRef.nativeElement.querySelectorAll('.edit-appointment');
    btn_edit_appointment_list.forEach(btn_edit_appointment => {
      btn_edit_appointment.addEventListener('click', this.showPopup.bind(this, btn_edit_appointment.id));
    });
    var btn_delete_appointment_list = this.elementRef.nativeElement.querySelectorAll('.delete-appointment');
    btn_delete_appointment_list.forEach(btn_delete_appointment => {
      btn_delete_appointment.addEventListener('click', this.deleteAppointment.bind(this, btn_delete_appointment.id));
    });
  }

  showCalendarMonth(year, month) {
    let firstDay = (new Date(year, month).getDay() + 6) % 7;
    let daysInMonth = 32 - new Date(year, month, 32).getDate();

    this.setTitleToolbar(null, null, month, year);

    this.showCalendarThead();

    this.calendarTbodyEl.innerHTML = ''; // Clear calendar-tbody
    this.calendarTableEl.style.tableLayout = 'fixed';

    // Create cells in calendar-tbody
    let date = 1;
    for (let i = 0; i < 6; i++) {
      let row = document.createElement('tr');

      for (let j = 0; j < 7; j++) {
        let cell = document.createElement('td');
        cell.classList.add('td-month');
        cell.id = date.toString();
        let cell_dateText = document.createElement('div');
        cell_dateText.classList.add('date-text');
        let cell_dateEvent = document.createElement('div');
        cell_dateEvent.classList.add('date-event');
        if (i === 0 && j < firstDay) {
          cell_dateText.textContent = '';
        } else if (date > daysInMonth) {
          if (j === 0) {
            break;
          } else {
            cell_dateText.textContent = '';
          }
        } else {
          cell.classList.add('active');
          /// td-month click
          cell.addEventListener('click', () => {
            this.timePeriod = 'day';
            this.showCalendarDay(year, month, parseInt(cell.id), null);
          });

          cell_dateText.textContent = (date < 10 ? `0${date}` : date).toString();

          /// check add events
          if (this.dataAppointmentsGrouped !== null) {
            let dateFormat = new Date(year, month, date).setHours(0);
            if (this.dataAppointmentsGrouped[dateFormat]) {
              let dataAppointmentsGroupedStatus = this.dataAppointmentsGrouped[
                dateFormat
              ].reduce(function (r, a) {
                r[a.state] = r[a.state] || [];
                r[a.state].push(a);
                return r;
              }, Object.create(null));
              let eventMonthString = '';
              for (const [key, value] of (<any>Object).entries(
                dataAppointmentsGroupedStatus
              )) {
                eventMonthString += this.getEventMonth(key, value.length);
              }
              cell_dateEvent.innerHTML = eventMonthString;
            }
          }

          date++;
        }
        cell.appendChild(cell_dateText);
        cell.appendChild(cell_dateEvent);
        row.appendChild(cell);
      }
      this.calendarTbodyEl.appendChild(row);
    }
  }

  convertDateToId(date, hasHours = true) {
    let result = `${date.getFullYear()}-${('0' + date.getMonth()).slice(-2)}-${(
      '0' + date.getDate()
    ).slice(-2)}`;
    if (hasHours) {
      result += `-${('0' + date.getHours()).slice(-2)}`;
    }
    return result;
  }

  getEventDayNWeek(appointment, isDay = true) {
    if (!appointment) {
      return document.createElement('div');
    }

    let statusShow = '';
    let classEvent = '';
    switch (appointment.state) {
      case 'confirmed':
        statusShow = 'Đang hẹn';
        classEvent = 'event-confirmed';
        break;
      case 'arrived':
        statusShow = 'Đã đến';
        classEvent = 'event-arrived';
        break;
      case 'cancel':
        statusShow = 'Hủy hẹn';
        classEvent = 'event-cancel';
        break;
      case 'overdue':
        statusShow = 'Quá hạn';
        classEvent = 'event-overdue';
        break;
      default:
        break;
    }

    let dateEventV2El = document.createElement('div');
    dateEventV2El.classList.add("date-event-v2");
    dateEventV2El.classList.add(`${classEvent}`);
    dateEventV2El.id = `appointment-${appointment.id}`;

    dateEventV2El.addEventListener('click', el => {
      // this.showPopup(parseInt(dateEventV2El.id.replace('appointment-', '')));
      // this.showPopup(appointment.id);
      el.stopPropagation();
    });

    const htmlString = `
          <div class="header">
              <div class="status">${statusShow}</div>
              <div class="action">
                  <div class="edit edit-appointment" id="${appointment.id}">
                    <i class="fas fa-pen"></i>
                  </div>
                  <div class="delete delete-appointment" id="${appointment.id}">
                    <i class="fas fa-sign-out-alt"></i>
                  </div>
              </div>
          </div>
          <a href="#" class="customer-name">${appointment.partnerName}</a>
          <div class="content phone">
              <i class="fas fa-phone"></i>
              <span>${appointment.partnerPhone}</span>
          </div>
          <div class="content time">
              <i class="fas fa-info-circle"></i>
              <span>
                  ${`${('0' + appointment.date.getHours()).slice(-2)}:00 - ${(
        '0' +
        (parseInt(appointment.date.getHours()) + 1)
      ).slice(-2)}:00`}
              </span>
          </div>
          <div class="content referrer">
              <i class="fas fa-user-plus"></i>
              <span>${appointment.doctorName}</span>
          </div>
      `;
    dateEventV2El.innerHTML = htmlString;

    return dateEventV2El;
  }

  getEventMonth(status, count) {
    let statusShow = '';
    let classBg = '';
    switch (status) {
      case 'confirmed':
        statusShow = 'Đang hẹn';
        classBg = 'bg-confirmed';
        break;
      case 'arrived':
        statusShow = 'Đã đến';
        classBg = 'bg-arrived';
        break;
      case 'cancel':
        statusShow = 'Hủy hẹn';
        classBg = 'bg-cancel';
        break;
      case 'overdue':
        statusShow = 'Quá hạn';
        classBg = 'bg-overdue';
        break;
      default:
        break;
    }
    const htmlString = `
          <div class="dot-status">
              <div class="dot-sematic ${classBg}"></div>
              <span>${statusShow}: ${count}</span>
          </div>
      `;
    return htmlString;
  }

  showPopup(id = null) {
    console.log("id: " + id);
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.appointId = id;
    modalRef.componentInstance.title = id ? "Cập nhật lịch hẹn" : "Đặt lịch hẹn";
    modalRef.result.then(result => {
      this.loadData();
      this.loadGridData();
    }, () => { });

    // this.contDialogCalendarEl.classList.remove('hidden');
    // if (id !== null) {
    //   this.appointmentIDChoose = id;
    //   const dataAppointment = this.dataAppointments.find(
    //     x => x.id === this.appointmentIDChoose
    //   );
    //   if (dataAppointment) {
    //     this.dateAppointmentFormat = dataAppointment['dateFormat'];
    //     this.selectStatusEl.value = dataAppointment.status;
    //     this.inputCustomerNameEl.value = dataAppointment.partnerName;
    //     this.inputCustomerPhoneEl.value = dataAppointment.partnerPhone.toString();
    //     this.inputReferrerNameEl.value = dataAppointment.doctorName;
    //   }
    // }
  }

  closePopup() {
    this.contDialogCalendarEl.classList.add('hidden');
    this.appointmentIDChoose = null;
    this.dateAppointmentFormat = null;
    this.selectStatusEl.value = 'confirmed';
    this.inputCustomerNameEl.value = '';
    this.inputCustomerPhoneEl.value = '';
    this.inputReferrerNameEl.value = '';
  }

  saveAppointment() {
    if (this.appointmentIDChoose !== null) {
      const dataAppointmentIndex = this.dataAppointments.findIndex(
        x => x.id === this.appointmentIDChoose
      );
      if (dataAppointmentIndex >= 0) {
        this.dataAppointments[dataAppointmentIndex].status = this.selectStatusEl.value;
        this.dataAppointments[dataAppointmentIndex].partnerName = this.inputCustomerNameEl.value;
        this.dataAppointments[dataAppointmentIndex].partnerPhone = parseInt(this.inputCustomerPhoneEl.value);
        this.dataAppointments[dataAppointmentIndex].doctorName = this.inputReferrerNameEl.value;
        let dateEventV2El = document.getElementById(
          `appointment-${this.dataAppointments[dataAppointmentIndex].id}`
        );
        const dateEventV2NewEl = this.getEventDayNWeek(
          this.dataAppointments[dataAppointmentIndex]
        );
        dateEventV2El.replaceWith(dateEventV2NewEl);
      }
    } else {
      const dataAppointment = {
        id: this.dataAppointments.length,
        date: new Date(this.dateAppointmentFormat),
        dateFormat: new Date(this.dateAppointmentFormat).setHours(0, 0, 0, 0),
        dateHour: new Date(this.dateAppointmentFormat).getHours(),
        status: this.selectStatusEl.value,
        partnerName: this.inputCustomerNameEl.value,
        partnerPhone: this.inputCustomerPhoneEl.value,
        doctorName: this.inputReferrerNameEl.value
      };
      this.dataAppointments.push(dataAppointment);

      if (this.dateAppointmentFormat !== null) {
        const listDataEventDay = document.getElementById(
          this.convertDateToId(this.dateAppointmentFormat, true)
        ).firstChild; // list-data-event-day
        const tdDayOverwriteEl = listDataEventDay.lastChild;
        listDataEventDay.removeChild(tdDayOverwriteEl);
        if (listDataEventDay) {
          listDataEventDay.appendChild(this.getEventDayNWeek(dataAppointment));
        }
        listDataEventDay.appendChild(tdDayOverwriteEl);
      }
    }
    // localStorage.setItem("dataAppointments", JSON.stringify(dataAppointments));
    this.groupByDataAppointments();
    this.closePopup();
  }

  // deleteAppointment(id) {
  //   this.appointmentIDChoose = id;
  //   if (this.appointmentIDChoose !== null) {
  //     const dataAppointmentIndex = this.dataAppointments.findIndex(
  //       x => x.id === this.appointmentIDChoose
  //     );
  //     if (dataAppointmentIndex >= 0) {
  //       this.dataAppointments.splice(dataAppointmentIndex, 1);
  //     }
  //     let dateEventV2El = document.getElementById(
  //       `appointment-${this.appointmentIDChoose}`
  //     );
  //     if (dateEventV2El) {
  //       dateEventV2El.remove();
  //     }
  //   }
  //   // localStorage.setItem("dataAppointments", JSON.stringify(dataAppointments));
  //   this.groupByDataAppointments();
  // }
}
