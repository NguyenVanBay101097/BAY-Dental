import { Component, Inject, NgZone, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import * as signalR from '@microsoft/signalr';
import { NgbDropdownToggle, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import * as moment from 'moment';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { ReceiveAppointmentService } from 'src/app/customer-receipt/receive-appointment.service';
import { EmployeeBasic, EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { AppointmentCreateUpdateComponent } from 'src/app/shared/appointment-create-update/appointment-create-update.component';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { ReceiveAppointmentDialogComponent } from 'src/app/shared/receive-appointment-dialog/receive-appointment-dialog.component';
import { PrintService } from 'src/app/shared/services/print.service';
import {
  AppointmentPaged
} from '../appointment';
import { AppointmentFilterExportExcelDialogComponent } from '../appointment-filter-export-excel-dialog/appointment-filter-export-excel-dialog.component';
import { AppointmentSignalRService } from '../appointment-signalr.service';
import { AppointmentService } from '../appointment.service';

@Component({
  encapsulation: ViewEncapsulation.None, //<<<<< this one! 
  // To css active with innerHTML
  selector: 'app-appointment-kanban',
  templateUrl: './appointment-kanban.component.html',
  styleUrls: ['./appointment-kanban.component.css'],
  host: { 'class': 'h-100' }
})
export class AppointmentKanbanComponent implements OnInit {
  @ViewChild('dropdownMenuBtn') dropdownMenuBtn: NgbDropdownToggle;
  dateFrom: Date;
  dateTo: Date;
  state: string;
  userId: string;
  search: string;
  searchUpdate = new Subject<string>();
  isLateFilter: boolean;
  // showDropdown = false;
  // dateList: Date[];

  // permission
  canAppointmentCreate = this.checkPermissionService.check(["Basic.Appointment.Create"]);
  canAppointmentEdit = this.checkPermissionService.check(["Basic.Appointment.Update"]);
  canAppointmentDelete = this.checkPermissionService.check(["Basic.Appointment.Delete"]);
  canEmployeeRead = this.checkPermissionService.check(["Catalog.Employee.Read"]);
  canCustomerLink = this.checkPermissionService.check(["Basic.Partner.Read"]);

  // public today: Date = new Date(new Date().toDateString());
  // public next3days: Date = new Date(new Date(new Date().setDate(new Date().getDate() + 3)).toDateString());

  // appointmentByDate: { [id: string]: AppointmentBasic[]; } = {};

  states: { text: string, value: string }[] = [
    { text: 'Tất cả', value: '' },
    { text: 'Đang hẹn', value: 'confirmed' },
    { text: 'Đã đến', value: 'done' },
    { text: 'Hủy hẹn', value: 'cancel' },
    { text: 'Quá hẹn', value: 'overdue' }
  ];
  stateSelected: string = '';

  types: { text: string, value: string }[] = [
    { text: 'Tất cả', value: '' },
    { text: 'Tái khám', value: 'repeat' },
    { text: 'Khám mới', value: 'new' }
  ];
  typeSelected: string = '';
  isRepeatCustomer: boolean;

  listEmployees: EmployeeBasic[] = [];
  filterEmployeeId: string; //id bác sĩ filter, undefined thì là tất cả, có giá trị là lọc bác sĩ
  filterDoctorIsNull: any; //lọc bác sĩ chưa xác định, undefined là ko lọc, true/false là lọc chưa xác định

  viewKanban: string = "calendar"; // "calendar", "list"
  gridData: GridDataResult;
  limit: number = 1000;
  offset: number = 0;
  loading: boolean = false;
  // events = [];
  // calendarApi: Calendar;
  // calendarPlugins = [dayGridPlugin, timeGrigPlugin, interactionPlugin];
  // @ViewChild('fullcalendar',{static: true}) calendarComponent: FullCalendarComponent;
  // titleDateToolbar: string = "";
  // viewToolbar: string = "timeGridWeek"; // "timeGridDay", "timeGridWeek", "dayGridMonth"
  // validRange: DateRangeInput;
  doctors = [];
  // New Calendar //
  calendarTableEl = null;
  calendarTheadEl = null;
  calendarTbodyEl = null;

  today = new Date();
  todayFormat = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate(), 0, 0, 0);
  differenceDay = 0;
  differenceWeek = 0;
  differenceMonth = 0;
  currentDate = this.todayFormat.getDate();
  currentMonth = this.todayFormat.getMonth();
  currentYear = this.todayFormat.getFullYear();

  daysOfWeek = ["Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật"];
  months = ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"];

  timePeriod = 'week';
  firstTime = 6; // format 24h : 0h - 23h
  lastTime = 23; // format 24h : 0h - 23h
  dataAppointmentsGrouped = null;
  dataAppointments = [];
  appointments: any[] = []; //source
  filterAppointments: any[] = []; //filter client
  titleToolbar = "";
  clientFilter: boolean = false;
  dataAppointmentsForFilter: any[] = [];
  connection: any;

  connectionEstablished = false;

  constructor(
    private appointmentService: AppointmentService,
    private intlService: IntlService,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private router: Router,
    private employeeService: EmployeeService,
    private checkPermissionService: CheckPermissionService,
    private receiveAppointmentService: ReceiveAppointmentService,
    private printService: PrintService,
    private authService: AuthService,
    @Inject('BASE_API') private baseApi: string,
    private _ngZone: NgZone,
    private appointmentSignalRService: AppointmentSignalRService
  ) { }

  ngOnInit() {
    // New Calendar //
    this.getElements();

    this.processDates();

    this.loadDataFromApi(); // Render Calendar

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.processSearch(value);
      });

    this.loadListEmployees();

    this.appointmentSignalRService.connectionEstablished$.subscribe((res) => {
      this._ngZone.run(() => {
        this.connectionEstablished = res;
      });
    });

    this.appointmentSignalRService.appointmentChanged$.subscribe((res: any) => {
      this._ngZone.run(() => {
        const idx = this.appointments.findIndex((x: any) => x.id === res.id);
        if (idx != -1) {
          this.appointments[idx] = res;
        }
        else {
          this.appointments.push(res);
        }

        this.filterDataClient();
      });
    });

    this.appointmentSignalRService.appointmentDeleted$.subscribe((res: any) => {
      this._ngZone.run(() => {
        const idx = this.appointments.findIndex((x: any) => x.id === res);
        if (idx != -1) {
          this.appointments.splice(idx, 1);
          this.filterDataClient();
        }
      });
    });
  }

  processSearch(value: string) {
    //  lọc theo từ khóa tìm kiếm
    this.search = value;
    this.filterDataClient(); // Render Calendar
  }

  processDates() {
    if (this.timePeriod == 'week') {
      let firstDay = this.todayFormat.getDate() - (this.todayFormat.getDay() == 0 ? 7 : this.todayFormat.getDay()) + 1 + 7 * this.differenceWeek; // First day is the day of the month - the day of the week
      let lastDay = firstDay + 6; // last day is the first day + 6
      this.dateFrom = new Date(this.currentYear, this.currentMonth, firstDay);
      this.dateTo = new Date(this.currentYear, this.currentMonth, lastDay);
    } else if (this.timePeriod == 'day') {
      this.dateFrom = new Date(this.currentYear, this.currentMonth, this.todayFormat.getDate() + this.differenceDay);
      this.dateTo = new Date(this.currentYear, this.currentMonth, this.todayFormat.getDate() + this.differenceDay);
    } else if (this.timePeriod == 'month') {
      const beginMonth = new Date(this.currentYear, this.currentMonth + this.differenceMonth, 1);
      let daysInMonth = new Date(beginMonth.getFullYear(), beginMonth.getMonth() + 1, 0).getDate();;
      const endMonth = new Date(beginMonth.getFullYear(), beginMonth.getMonth(), daysInMonth);

      this.dateFrom = new Date(beginMonth.getFullYear(), beginMonth.getMonth(), beginMonth.getDate() - (beginMonth.getDay() == 0 ? 6 : beginMonth.getDay() - 1));
      this.dateTo = new Date(endMonth.getFullYear(), endMonth.getMonth(), endMonth.getDate() + (endMonth.getDay() == 0 ? 0 : 7 - endMonth.getDay()));
    }
  }

  // loadDoctorList() {
  //   var val = {
  //     dateTimeFrom: this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '',
  //     dateTimeTo: this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : ''
  //   };
  //   this.appointmentService.getListDoctor(val).subscribe(res => {
  //     this.doctors = res;
  //   })
  // }

  loadListEmployees() {
    var paged = new EmployeePaged();
    paged.isDoctor = true;
    this.employeeService.getEmployeePaged(paged).subscribe((res) => {
      this.listEmployees = res.items;
    });
  }

  onChangeEmployee(employeeId = undefined) {
    this.filterEmployeeId = employeeId;
    this.filterDoctorIsNull = undefined;
    this.loadDataFromApi(); // Render Calendar
  }

  onFilterDoctorIsNull() {
    this.filterEmployeeId = undefined;
    this.filterDoctorIsNull = true;
    this.loadDataFromApi();
  }

  onChangeState(state) {
    this.stateSelected = state;
    if (state == 'overdue') {
      this.state = 'confirmed';
      this.isLateFilter = true;
    } else if (state == 'confirmed') {
      this.state = 'confirmed';
      this.isLateFilter = false;
    } else {
      this.state = state;
      this.isLateFilter = undefined;
    }

    this.filterDataClient();
  }

  onChangeType(type) {
    this.typeSelected = type;
    if (type === 'repeat') {
      this.isRepeatCustomer = true;
    } else if (type === 'new') {
      this.isRepeatCustomer = false;
    } else {
      this.isRepeatCustomer = undefined;
    }

    this.filterDataClient();
  }

  // createAppointment() {
  //   const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
  //   modalRef.componentInstance.title = "Đặt lịch hẹn";
  //   modalRef.result.then(result => {
  //     this.loadData();
  //   }, () => { });
  // }

  refreshData() {
    this.loadDataFromApi(); // Render Calendar
  }

  filterDataClient() {
    let sourceAppoinments = this.appointments.slice();
    if (this.state) {
      sourceAppoinments = sourceAppoinments.filter(x => x.state == this.state);
    }

    if (this.isLateFilter != undefined) {
      sourceAppoinments = sourceAppoinments.filter(x => x.isLate == this.isLateFilter);
    }

    if (this.isRepeatCustomer != undefined) {
      sourceAppoinments = sourceAppoinments.filter(x => x.isRepeatCustomer == this.isRepeatCustomer);
    }

    if (this.search) {
      sourceAppoinments = sourceAppoinments.filter(x => {
        return _.includes(x.partnerName.toLowerCase(), this.search.toLowerCase()) || _.includes(x.partnerPhone.toLowerCase(), this.search.toLowerCase());
      });
    }

    this.filterAppointments = sourceAppoinments;
    this.renderCalendar();
  }

  onCountState(state: string): number {
    if (state === 'overdue') {
      return this.appointments.filter(x => x.state == 'confirmed' && x.isLate).length;
    } else if (state == 'confirmed') {
      return this.appointments.filter(x => x.state == state && !x.isLate).length;
    } else if (state) {
      return this.appointments.filter(x => x.state == state).length;
    } else {
      return this.appointments.length;
    }
  }

  onCountType(value: any): number {
    return value ? this.appointments.filter(x => {
      return value === 'repeat' ? Boolean(x.isRepeatCustomer) : Boolean(!x.isRepeatCustomer)
    }).length : this.appointments.length;
  }

  loadDataFromApi() {
    var val = new AppointmentPaged();
    val.limit = this.limit;
    val.offset = this.offset;
    // val.state = this.state || '';
    // if (this.isLateFilter) {
    //   val.isLate = this.isLateFilter;
    // }
    val.search = this.search || '';
    val.doctorId = this.filterEmployeeId || '';
    val.dateTimeFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
    val.dateTimeTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';
    val.doctorIsNull = this.filterDoctorIsNull || '';
    // if (this.isRepeatCustomer != undefined) {
    //   val.isRepeatCustomer = this.isRepeatCustomer;
    // }

    this.appointmentService.getPaged(val).pipe(
      map((response: any) =>
      (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe((result: any) => {
      this.gridData = result;
      this.loading = false;
      this.appointments = result.data;
      this.filterDataClient();
    }, (error: any) => {
      console.log(error);
      this.loading = false;
    });
  }

  // resetData() {
  //   this.appointmentByDate = Object.assign({});
  // }

  // deleteAppointment(appointment: AppointmentBasic) {
  //   const modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
  //   modalRef.componentInstance.title = 'Xóa lịch hẹn';
  //   modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa lịch hẹn này?';
  //   modalRef.result.then(() => {
  //     this.appointmentService.removeAppointment(appointment.id).subscribe(() => {
  //       var date = new Date(appointment.date);
  //       var key = date.toDateString();
  //       if (this.appointmentByDate[key]) {
  //         var index = _.findIndex(this.appointmentByDate[key], o => o.id == appointment.id);
  //         if (index != -1) {
  //           this.appointmentByDate[key].splice(index, 1);
  //         }
  //       }
  //     });
  //   });
  // }

  // editAppointment(appointment: AppointmentBasic) {
  //   const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { size: 'xl', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
  //   modalRef.componentInstance.appointId = appointment.id;
  //   modalRef.result.then(() => {
  //     this.loadData();
  // this.appointmentService.getBasic(appointment.id).subscribe(item => {
  //   var date = new Date(item.date);
  //   var key = date.toDateString();
  //   if (this.appointmentByDate[key]) {
  //     var index = _.findIndex(this.appointmentByDate[key], o => o.id == item.id);
  //     if (index != -1) {
  //       this.appointmentByDate[key][index] = item;
  //     }
  //   }
  // });
  //   }, () => {
  //   });
  // }

  // addAppointmentById(id) {
  //   this.appointmentService.getBasic(id).subscribe(item => {
  //     var date = new Date(item.date);
  //     var key = date.toDateString();
  //     var index = _.findIndex(this.dateList, o => o.toDateString() == date.toDateString());
  //     if (index == -1) {
  //       this.dateList.push(date);
  //     }

  //     if (!this.appointmentByDate[key]) {
  //       this.appointmentByDate[key] = [];
  //     }

  //     this.appointmentByDate[key].unshift(item);
  //   });
  // }

  // getDateList() {
  //   if (!this.dateFrom || !this.dateTo) {
  //     return [];
  //   }
  //   var list = [];
  //   var oneDay = 1000 * 60 * 60 * 24;
  //   var days = (this.dateTo.getTime() - this.dateFrom.getTime()) / oneDay;
  //   if (days > 30) {
  //     alert('Vui lòng xem tối đa 30 ngày để đảm bảo tốc độ phần mềm');
  //     return [];
  //   }
  //   for (var i = 0; i < days + 1; i++) {
  //     var date = new Date(this.dateFrom.toDateString());
  //     date.setDate(this.dateFrom.getDate() + i);
  //     list.push(date);
  //   }
  //   return list;
  // }

  // getPreviousDotKham(id) {
  //   var paged = new DotKhamPaged;
  //   paged.appointmentId = id;
  //   paged.limit = 1;
  //   this.dotkhamService.getPaged(paged).subscribe(rs => {
  //     if (rs.items.length) {
  //       this.router.navigate(['/dot-khams/edit/' + rs.items[0].id]);
  //     } else {
  //       this.notificationService.show({
  //         content: 'Không có đợt khám nào từ lịch hẹn này.',
  //         hideAfter: 3000,
  //         position: { horizontal: 'center', vertical: 'top' },
  //         animation: { type: 'fade', duration: 400 },
  //         type: { style: 'error', icon: true }
  //       });
  //     }
  //   });
  // }

  // createDotKham(id) {
  //   var paged = new DotKhamPaged;
  //   paged.appointmentId = id;
  //   paged.limit = 1;
  //   this.dotkhamService.getPaged(paged).subscribe((rs: any) => {
  //     if (rs.items.length) {
  //       let modalRef = this.modalService.open(SaleOrderCreateDotKhamDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
  //       modalRef.componentInstance.title = 'Tạo đợt khám';
  //       modalRef.componentInstance.saleOrderId = rs.items[0].saleOrderId;

  //       modalRef.result.then(result => {
  //         if (result.view) {
  //           this.router.navigate(['/dot-khams/edit/', result.result.id]);
  //         } else {
  //         }
  //       }, () => {
  //       });
  //     } else {
  //       this.notificationService.show({
  //         content: 'Không có đợt khám nào từ lịch hẹn này.',
  //         hideAfter: 3000,
  //         position: { horizontal: 'center', vertical: 'top' },
  //         animation: { type: 'fade', duration: 400 },
  //         type: { style: 'error', icon: true }
  //       });
  //     }
  //   });
  // }

  exportExcelFile() {
    var val = new AppointmentPaged();
    val.limit = 1000;
    val.state = this.state || '';
    if (this.isLateFilter) {
      val.isLate = this.isLateFilter;
    }
    val.search = this.search || '';
    val.doctorId = this.filterEmployeeId || '';
    val.dateTimeFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
    val.dateTimeTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';
    if (this.isRepeatCustomer != undefined) {
      val.isRepeatCustomer = this.isRepeatCustomer;
    }

    const modalRef = this.modalService.open(AppointmentFilterExportExcelDialogComponent, { size: 'md', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xuất dữ liệu lịch hẹn';
    modalRef.componentInstance.dateFrom = new Date(this.dateFrom);
    modalRef.componentInstance.dateTo = new Date(this.dateTo);
    modalRef.result.then(result => {
      if (result.state === 'period') {
        val.state = '';
        val.search = '';
        val.doctorId = '';
        val.dateTimeFrom = result.dateFrom ? this.intlService.formatDate(result.dateFrom, 'yyyy-MM-dd') : '';
        val.dateTimeTo = result.dateTo ? this.intlService.formatDate(result.dateTo, 'yyyy-MM-dd') : '';
      }

      this.appointmentService.exportExcel2(val).subscribe((result: any) => {
        let filename = 'DanhSachLichHen';
        let newBlob = new Blob([result], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        let data = window.URL.createObjectURL(newBlob);
        let link = document.createElement('a');
        link.href = data;
        link.download = filename;
        link.click();
        setTimeout(() => {
          window.URL.revokeObjectURL(data);
        }, 100);
      });
    }, (err) => console.log(err)
    );
  }

  // handleEventClick(e) {
  //   var id = e.event._def.publicId;
  //   const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
  //   modalRef.componentInstance.appointId = id;
  //   modalRef.result.then(() => {
  //     this.loadData();
  //     this.appointmentService.getBasic(id).subscribe(item => {
  //       var date = new Date(item.date);
  //       var key = date.toDateString();
  //       if (this.appointmentByDate[key]) {
  //         var index = _.findIndex(this.appointmentByDate[key], o => o.id == item.id);
  //         if (index != -1) {
  //           this.appointmentByDate[key][index] = item;
  //         }
  //       }
  //     });
  //   }, () => {
  //   });
  // }

  changeViewKanban(event) {
    this.viewKanban = event;
  }

  pageChange(event: PageChangeEvent): void {
    this.offset = event.skip;
  }

  computeNameSerivc(items: any[]) {
    var serviceName = "";
    if (items && items.length > 0) {
      serviceName = items.map(x => x.name).join(', ');
    }
    return serviceName;
  }

  // intersectionTwoDateRange(a: MyDateRange, b: MyDateRange) {
  //   var min: MyDateRange = a.start < b.start ? a : b;
  //   var max: MyDateRange = min == a ? b : a;
  //   if (min.end < max.start) return null;
  //   return new MyDateRange(max.start, min.end < max.end ? min.end : max.end);
  // }

  // New Calendar //
  getElements() {
    this.calendarTableEl = document.getElementById('calendar-table');
    this.calendarTheadEl = document.getElementById('calendar-thead');
    this.calendarTbodyEl = document.getElementById('calendar-tbody');
  }

  groupByDataAppointments() {
    if (this.dataAppointmentsForFilter !== null && this.dataAppointmentsForFilter.length > 0) {
      this.dataAppointmentsGrouped = this.dataAppointmentsForFilter.reduce(function (r, a) {
        r[a['dateFormat']] = r[a['dateFormat']] || [];
        r[a['dateFormat']].push(a);
        return r;
      }, Object.create(null));
    } else {
      this.dataAppointmentsGrouped = null
    }
  }

  next() {
    if (this.timePeriod === 'day') {
      this.differenceDay++;
    } else if (this.timePeriod === 'week') {
      this.differenceWeek++;
    } else {
      this.differenceMonth++;
    }

    this.processDates();
    this.loadDataFromApi();
  }

  previous() {
    if (this.timePeriod === 'day') {
      this.differenceDay--;
    } else if (this.timePeriod === 'week') {
      this.differenceWeek--;
    } else {
      this.differenceMonth--;
    }

    this.processDates();
    this.loadDataFromApi();
  }

  jump_today() {
    if (this.timePeriod === 'day') {
      this.differenceDay = 0;
    } else if (this.timePeriod == 'week') {
      this.differenceWeek = 0;
    } else {
      this.differenceMonth = 0;
    }

    this.processDates();
    this.loadDataFromApi();
  }

  renderCalendar() {
    //Vẽ header
    if (this.timePeriod === 'day') {
      this.showCalendarDay();
    } else if (this.timePeriod === 'week') {
      this.showCalendarWeek();
    } else {
      // month
      this.showCalendarMonth();
    }

    //Vẽ body
    this.loadAppointmentToCalendar();
  }

  changeTimePeriod(event) {
    //thay đổi dateFrom, dateTo
    this.processDates();
    this.loadDataFromApi();
    // this.jump_today();
  }

  setTitleToolbar() {
    if (this.timePeriod === 'day') {
      this.titleToolbar = moment(this.dateFrom).format('DD/MM/YYYY');
    } else if (this.timePeriod === 'week') {
      this.titleToolbar = moment(this.dateFrom).format('DD/MM/YYYY') + ' - ' + moment(this.dateTo).format('DD/MM/YYYY');
    } else {
      const beginMonth = new Date(this.currentYear, this.currentMonth + this.differenceMonth, 1);
      this.titleToolbar = moment(beginMonth).format('MM/YYYY');
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
      date = new Date(this.dateFrom);
      this.daysOfWeek.forEach(el => {
        let cell = document.createElement('th');
        cell.classList.add('th-v2');
        if (
          date.getDate() === this.todayFormat.getDate() &&
          date.getMonth() === this.todayFormat.getMonth() &&
          date.getFullYear() === this.todayFormat.getFullYear()
        ) {
          cell.innerHTML = `
                      <div class="now">${('0' + date.getDate()).slice(-2)}</div>
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

  showCalendarDay() {
    let firstDate = this.dateFrom;

    this.setTitleToolbar();

    this.showCalendarThead(firstDate);

    this.calendarTbodyEl.innerHTML = ''; // Clear calendar-tbody
    this.calendarTableEl.style.tableLayout = 'auto';

    // Create cells in calendar-tbody
    for (let i = this.firstTime; i <= this.lastTime; i++) {
      var dateTmp = new Date(firstDate.getFullYear(), firstDate.getMonth(), firstDate.getDate(), i);

      let row = document.createElement('tr');
      let cellTime = document.createElement('td');
      cellTime.classList.add('td-time');
      cellTime.id = i.toString();
      cellTime.textContent = `${i < 10 ? `0${i}` : i}:00 - ${i + 1 < 10 ? `0${i + 1}` : i + 1}:00`;
      let cell = document.createElement('td');
      cell.classList.add('td-day');
      cell.id = moment(dateTmp).format('DD-MM-YYYY-HH');
      cell.addEventListener('click', (e) => {
        e.preventDefault();
        const dateTime = new Date(firstDate.getFullYear(), firstDate.getMonth(), firstDate.getDate(), i);
        this.createUpdateAppointment(null, dateTime);
      });

      let cell_dateEvent = document.createElement('div');
      cell_dateEvent.classList.add('list-data-event-day');

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

    // Data Processing 
    // this.loadData();
  }

  showCalendarWeek() {
    let firstDate = this.dateFrom;

    this.setTitleToolbar();

    this.showCalendarThead();

    this.calendarTbodyEl.innerHTML = ''; // Clear calendar-tbody
    this.calendarTableEl.style.tableLayout = 'fixed';

    // Create cells in calendar-tbody
    let row = document.createElement('tr');
    for (let i = 0; i < this.daysOfWeek.length; i++) {
      let cell = document.createElement('td');
      cell.classList.add('td-week');
      const tempDate = new Date(firstDate.getFullYear(), firstDate.getMonth(), firstDate.getDate() + i);
      cell.id = moment(tempDate).format('DD-MM-YYYY');
      /// td-week click
      cell.addEventListener('click', () => {
        this.timePeriod = 'day';
        this.differenceDay = moment(tempDate).diff(moment(this.todayFormat), 'days');
        this.processDates();
        this.loadDataFromApi();
      });

      let cell_dateEvent = document.createElement('div');
      cell_dateEvent.classList.add('list-data-event-week');

      /// td-week-overwrite
      let tdWeekOverwrite = document.createElement('div');
      tdWeekOverwrite.classList.add('td-week-overwrite');

      cell_dateEvent.appendChild(tdWeekOverwrite);
      cell.appendChild(cell_dateEvent);
      row.appendChild(cell);
    }
    this.calendarTbodyEl.appendChild(row);
  }

  showCalendarMonth() {
    let firstDay = this.dateFrom;
    const beginMonth = new Date(this.currentYear, this.currentMonth + this.differenceMonth, 1);

    this.setTitleToolbar();

    this.showCalendarThead();

    this.calendarTbodyEl.innerHTML = ''; // Clear calendar-tbody
    this.calendarTableEl.style.tableLayout = 'fixed';

    const weekCount = this.weekCount(beginMonth.getFullYear(), beginMonth.getMonth() + 1);
    // Create cells in calendar-tbody
    for (let i = 0; i < weekCount; i++) {
      let row = document.createElement('tr');

      for (let j = 0; j < 7; j++) {
        const dateTmp = new Date(firstDay.getFullYear(), firstDay.getMonth(), firstDay.getDate() + i * 7 + j);

        let cell = document.createElement('td');
        cell.classList.add('td-month');
        let cell_dateText = document.createElement('div');
        cell_dateText.classList.add('date-text');
        let cell_dateEvent = document.createElement('div');
        cell_dateEvent.classList.add('date-event');

        cell.classList.add('active');
        cell.id = moment(dateTmp).format('DD-MM-YYYY');
        /// td-month click
        cell.addEventListener('click', () => {
          this.timePeriod = 'day';
          this.differenceDay = moment(dateTmp).diff(moment(this.todayFormat), 'days');
          this.processDates();
          this.loadDataFromApi();
        });

        cell_dateText.textContent = moment(dateTmp).format('DD');

        cell.appendChild(cell_dateText);
        cell.appendChild(cell_dateEvent);
        row.appendChild(cell);
      }
      this.calendarTbodyEl.appendChild(row);
    }
  }

  weekCount(year, month_number) {
    // month_number is in the range 1..12
    var firstOfMonth = new Date(year, month_number - 1, 1);
    var lastOfMonth = new Date(year, month_number, 0);

    var used = firstOfMonth.getDay() + (firstOfMonth.getDay() == 0 ? 6 : -1) + lastOfMonth.getDate();

    return Math.ceil(used / 7);
  }

  loadAppointmentToCalendar() {
    this.dataAppointments = this.filterAppointments.map(v => ({
      ...v,
      date: new Date(v.date),
      dateFormat: new Date(v.date).setHours(0, 0, 0, 0),
      dateHour: new Date(v.date).getHours()
    }));

    this.dataAppointmentsGrouped = this.dataAppointments.reduce(function (r, a) {
      r[a['dateFormat']] = r[a['dateFormat']] || [];
      r[a['dateFormat']].push(a);
      return r;
    }, Object.create({}));

    if (this.timePeriod === 'day') {
      let dateFormat = this.dateFrom.setHours(0);
      if (this.dataAppointmentsGrouped[dateFormat]) {
        let dataAppointmentsGroupedHour = this.dataAppointmentsGrouped[dateFormat].reduce(
          function (r, a) {
            r[a.dateHour] = r[a.dateHour] || [];
            r[a.dateHour].push(a);
            return r;
          },
          Object.create({})
        );
        for (let key in dataAppointmentsGroupedHour) {
          const idCell = moment(new Date(this.dateFrom.getFullYear(), this.dateFrom.getMonth(), this.dateFrom.getDate(), parseInt(key))).format('DD-MM-YYYY-HH');
          let cell_dateEventEl = document.querySelector(`[id='${idCell}'] .list-data-event-day`);
          if (cell_dateEventEl) {
            const tdDayOverwriteEl = cell_dateEventEl.lastChild; // td-day-overwrite
            cell_dateEventEl.removeChild(tdDayOverwriteEl); // td-day-overwrite
            dataAppointmentsGroupedHour[key].forEach(appointment => {
              cell_dateEventEl.appendChild(this.getEventDayNWeek(appointment));
            });
            cell_dateEventEl.appendChild(tdDayOverwriteEl); // td-day-overwrite
          }
        }
      }
    } else if (this.timePeriod === 'week') {
      for (let key in this.dataAppointmentsGrouped) {
        if (parseInt(key) >= this.dateFrom.setHours(0) && parseInt(key) <= this.dateTo.setHours(0)) {
          const dateKey = new Date(parseInt(key));
          const idCell = moment(dateKey).format('DD-MM-YYYY');
          let cell_dateEventEl = document.querySelector(`[id='${idCell}'] .list-data-event-week`);
          if (cell_dateEventEl) {
            const tdWeekOverwriteEl = cell_dateEventEl.lastChild; // td-week-overwrite
            cell_dateEventEl.removeChild(tdWeekOverwriteEl); // td-week-overwrite
            this.dataAppointmentsGrouped[key].forEach(appointment => {
              cell_dateEventEl.appendChild(this.getEventDayNWeek(appointment));
            });
            cell_dateEventEl.appendChild(tdWeekOverwriteEl); // td-week-overwrite
          }
        }
      }
    } else {
      // month
      for (let key in this.dataAppointmentsGrouped) {
        if (parseInt(key) >= this.dateFrom.setHours(0) && parseInt(key) <= this.dateTo.setHours(0)) {
          const dateKey = new Date(parseInt(key));
          const year = dateKey.getFullYear();
          const month = dateKey.getMonth();
          const date = dateKey.getDate();
          const idCell = moment(dateKey).format('DD-MM-YYYY');
          let cell_dateEventEl = document.querySelector(`[id='${idCell}'] .date-event`);
          let dataAppointmentsGroupedState = this.dataAppointmentsGrouped[key].reduce(function (r, a) {
            if (a.state == 'confirmed' && a.isLate) {
              r['overdue'] = r['overdue'] || [];
              r['overdue'].push(a);
            }
            else {
              r[a.state] = r[a.state] || [];
              r[a.state].push(a);
            }
            return r;
          }, Object.create({}));

          let eventMonthString = '';
          for (let key in dataAppointmentsGroupedState) {
            eventMonthString += this.getEventMonth(key, dataAppointmentsGroupedState[key].length);
          }
          cell_dateEventEl.innerHTML = eventMonthString;
        }
      }
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
      case 'done':
        statusShow = 'Đã đến';
        classEvent = 'event-arrived';
        break;
      case 'cancel':
        statusShow = 'Hủy hẹn';
        classEvent = 'event-cancel';
        break;
      // case 'overdue':
      //   statusShow = 'Quá hạn';
      //   classEvent = 'event-overdue';
      //   break;
      default:
        statusShow = 'Đang hẹn';
        classEvent = 'event-confirmed';
        break;
    }

    if (appointment.isLate) {
      statusShow = 'Quá hẹn';
      classEvent = 'event-overdue';
    }

    let dateEventV2El = document.createElement('div');
    dateEventV2El.classList.add("date-event-v2");
    dateEventV2El.classList.add(`${classEvent}`);
    dateEventV2El.classList.add(`appointment_border_color_${appointment.color}`);
    let color = appointment.color || 0;
    if (color != '') {
      dateEventV2El.classList.add(`appointment_color_${color}`);
    }
    dateEventV2El.id = `appointment-${appointment.id}`;

    dateEventV2El.addEventListener('click', el => {
      el.stopPropagation();
    });

    let headerEl = document.createElement('div');
    headerEl.classList.add("header");
    let statusEl = document.createElement('div');
    statusEl.classList.add("status");
    statusEl.innerText = statusShow;
    let actionEl = document.createElement('div');
    actionEl.classList.add("t-action");
    let btnEditEl = document.createElement('div');
    btnEditEl.classList.add("edit");
    btnEditEl.title = "Sửa";
    btnEditEl.innerHTML = `<i class="fas fa-pen"></i>`;
    btnEditEl.addEventListener("click", () => {
      this.createUpdateAppointment(appointment.id);
    });
    let btnReceiveEl = document.createElement('div');
    btnReceiveEl.classList.add("receive");
    btnReceiveEl.title = "Tiếp nhận";
    btnReceiveEl.innerHTML = `<i class="fas fa-sign-out-alt"></i>`;
    btnReceiveEl.addEventListener("click", () => {
      this.receiveAppointment(appointment.id);
    });
    actionEl.appendChild(btnEditEl);
    actionEl.appendChild(btnReceiveEl);
    headerEl.appendChild(statusEl);
    headerEl.appendChild(actionEl);
    let customerNameEl = document.createElement('a');
    customerNameEl.classList.add("customer-name");
    if (color != '')
      dateEventV2El.classList.add("text-name-color");

    customerNameEl.title = "Xem hồ sơ khách hàng";
    customerNameEl.innerText = appointment.partnerName;
    customerNameEl.addEventListener("click", () => {
      this.router.navigate(['/partners/customer/' + appointment.partnerId]);
    });
    let contentPhoneEl = document.createElement('div');
    contentPhoneEl.classList.add("content", "phone");
    contentPhoneEl.innerHTML = `
      <i class="fas fa-phone"></i>
      <span>${appointment.partnerPhone || ""}</span>
    `;
    let contentTimeEl = document.createElement('div');
    contentTimeEl.classList.add("content", "time");
    contentTimeEl.innerHTML = `
      <i class="fas fa-info-circle"></i>
      <span>
        ${('0' + appointment.date.getHours()).slice(-2)}:${('0' + appointment.date.getMinutes()).slice(-2)} - 
        ${('0' + (appointment.date.getHours() + Math.floor((appointment.date.getMinutes() + appointment.timeExpected) / 60)))
        .slice(-2)}:${('0' + Math.floor((appointment.date.getMinutes() + appointment.timeExpected) % 60)).slice(-2)}
      </span>
    `;
    let contentReferrerEl = document.createElement('div');
    contentReferrerEl.classList.add("content", "referrer");
    contentReferrerEl.innerHTML = `
      <i class="fas fa-user-plus"></i>
      <span>${appointment.doctorName || ""}</span>
    `;

    dateEventV2El.appendChild(headerEl);
    dateEventV2El.appendChild(customerNameEl);
    dateEventV2El.appendChild(contentPhoneEl);
    dateEventV2El.appendChild(contentTimeEl);
    dateEventV2El.appendChild(contentReferrerEl);

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
      case 'done':
        statusShow = 'Đã đến';
        classBg = 'bg-arrived';
        break;
      case 'cancel':
        statusShow = 'Hủy hẹn';
        classBg = 'bg-cancel';
        break;
      case 'overdue':
        statusShow = 'Quá hẹn';
        classBg = 'bg-overdue';
        break;
      default:
        statusShow = 'Đang hẹn';
        classBg = 'bg-confirmed';
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

  createUpdateAppointment(id = null, dateTime = null) {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.appointId = id;
    modalRef.componentInstance.title = id ? "Cập nhật lịch hẹn" : "Đặt lịch hẹn";
    modalRef.componentInstance.dateTime = dateTime;
    modalRef.result.then(result => {
      if (!this.connectionEstablished) {
        this.loadDataFromApi();
      }
    }, () => { });
  }

  // form tiếp nhận
  receiveAppointment(id = null) {
    if (id) {
      // const appoint = this.dataAppointments.find(value => value.id === id);

      this.receiveAppointmentService.defaultGet(id).subscribe(res => {
        const modalRef = this.modalService.open(ReceiveAppointmentDialogComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.appointId = id;
        modalRef.componentInstance.receiveAppointmentDisplay = res;
        modalRef.result.then(result => {
          this.notificationService.show({
            content: 'Lưu thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });

          if (!this.connectionEstablished) {
            this.loadDataFromApi(); // Render Calendar
          }
        }, () => { });
      })
    }
  }

  onPrint(id) {
    this.appointmentService.print(id).subscribe((res: any) => {
      this.printService.printHtml(res.html);
    });
  }

  computeTimeExpected() {
    const timeExpected = this.dataAppointments.filter(x => x.state === 'confirmed' && Boolean(!x.isLate)).reduce((previousValue, currentValue) => {
      return previousValue += currentValue.timeExpected;
    }, 0) || 0;

    const m = timeExpected % 60;
    const h = (timeExpected - m) / 60;
    const HHMM = h.toString() + 'g' + (m < 10 ? '0' : '') + m.toString() + 'p';
    return HHMM;
  }

  ngOnDestroy(): void {
  }
}
