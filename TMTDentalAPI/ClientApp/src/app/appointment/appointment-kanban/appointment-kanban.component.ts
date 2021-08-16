import { Component, OnInit, Input, ViewChild, ElementRef, ViewChildren, AfterViewInit, ViewEncapsulation } from '@angular/core';
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
import { GridDataResult, PageChangeEvent, RowClassArgs } from '@progress/kendo-angular-grid';
import { MyDateRange } from '../my-date-range';
import { CustomerReceipCreateUpdateComponent } from 'src/app/shared/customer-receip-create-update/customer-receip-create-update.component';
import { AppointmentFilterExportExcelDialogComponent } from '../appointment-filter-export-excel-dialog/appointment-filter-export-excel-dialog.component';
@Component({
  encapsulation: ViewEncapsulation.None, //<<<<< this one! 
  // To css active with innerHTML
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

  states: { text: string, value: string, color?: string }[] = [
    { text: 'Tất cả', value: '', color: '' },
    { text: 'Đang hẹn', value: 'confirmed', color: '#2395FF' },
    { text: 'Đã đến', value: 'done', color: '#28A745' },
    { text: 'Hủy hẹn', value: 'cancel', color: '#EB3B5B' },
    { text: 'Quá hẹn', value: 'overdue', color: '#FFC107' }
  ];

  stateSelected: string = this.states[0].value;
  listEmployees: EmployeeBasic[] = [];
  employeeSelected: string = '';

  viewKanban: string = "calendar"; // "calendar", "list"
  gridData: GridDataResult;
  limit: number = 1000;
  offset: number = 0
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
  currentDate = this.todayFormat.getDate();
  currentMonth = this.todayFormat.getMonth();
  currentYear = this.todayFormat.getFullYear();

  daysOfWeek = ["Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật"];
  months = ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"];

  timePeriod = 'week';
  firstTime = 6; // format 24h : 0h - 23h
  lastTime = 23; // format 24h : 0h - 23h
  dataAppointmentsGrouped = null;
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

  ngOnInit() {
    // New Calendar //
    this.getElements();

    this.renderCalendar(); // Render Calendar

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.renderCalendar(); // Render Calendar
      });

    this.loadListEmployees();
    // this.loadDoctorList();
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

  onChangeEmployee(employeeId) {
    this.employeeSelected = employeeId;
    this.renderCalendar(); // Render Calendar
  }

  // onChangeDate(e) {
  //   this.dateFrom = e.dateFrom;
  //   this.dateTo = e.dateTo;
  //   this.loadData();
  //   this.loadGridData();
  // }

  onChangeState(state) {
    if (state == 'overdue') {
      this.state = 'confirmed';
      this.isLateFilter = true;
    } else {
      this.state = state;
      this.isLateFilter = undefined;
    }
   
    this.renderCalendar(); // Render Calendar
  }

  // createAppointment() {
  //   const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
  //   modalRef.componentInstance.title = "Đặt lịch hẹn";
  //   modalRef.result.then(result => {
  //     this.loadData();
  //   }, () => { });
  // }

  refreshData() {
    this.renderCalendar(); // Render Calendar
  }

  loadData() {
    // this.resetData();
    // this.loading = true;
    var val = new AppointmentPaged();
    val.limit = this.limit;
    val.offset = this.offset;
    val.state = this.state || '';
    if (this.isLateFilter) {
      val.isLate = this.isLateFilter;
    }
    val.search = this.search || '';
    val.doctorId = this.employeeSelected || '';
    val.dateTimeFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
    val.dateTimeTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';

    this.appointmentService.getPaged(val).pipe(
      map((response: any) =>
      (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe((result: any) => {
      this.gridData = result;
      this.loading = false;
      this.dataAppointments = result.data.map(v => ({
        ...v,
        date: new Date(v.date),
        dateFormat: new Date(v.date).setHours(0, 0, 0, 0),
        dateHour: new Date(v.date).getHours()
      }));

      this.loadAppointmentToCalendar();
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
  //       console.log(rs.items);
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
    val.search = this.search || '';
    val.doctorId = this.employeeSelected || '';
    val.dateTimeFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
    val.dateTimeTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';

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
    if (this.dataAppointments !== null && this.dataAppointments.length > 0) {
      this.dataAppointmentsGrouped = this.dataAppointments.reduce(function (r, a) {
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
      this.showCalendarDay(this.currentYear, this.currentMonth, this.currentDate, this.differenceDay);
    } else if (this.timePeriod === 'week') {
      this.differenceWeek++;
      this.showCalendarWeek(this.currentYear, this.currentMonth, this.differenceWeek);
    } else {
      // month
      this.currentMonth = (this.currentMonth + 1) % 12;
      this.currentYear = this.currentMonth === 11 ? this.currentYear + 1 : this.currentYear;
      this.showCalendarMonth(this.currentYear, this.currentMonth);
    }
  }

  previous() {
    if (this.timePeriod === 'day') {
      this.differenceDay--;
      this.showCalendarDay(this.currentYear, this.currentMonth, this.currentDate, this.differenceDay);
    } else if (this.timePeriod === 'week') {
      this.differenceWeek--;
      this.showCalendarWeek(this.currentYear, this.currentMonth, this.differenceWeek);
    } else {
      // month
      this.currentMonth = this.currentMonth === 0 ? 11 : this.currentMonth - 1;
      this.currentYear = this.currentMonth === 0 ? this.currentYear - 1 : this.currentYear;
      this.showCalendarMonth(this.currentYear, this.currentMonth);
    }
  }

  jump_today() {
    this.differenceDay = 0;
    this.differenceWeek = 0;
    this.currentDate = this.todayFormat.getDate();
    this.currentMonth = this.todayFormat.getMonth();
    this.currentYear = this.todayFormat.getFullYear();
    if (this.timePeriod === 'day') {
      this.showCalendarDay(this.currentYear, this.currentMonth, this.currentDate, this.differenceDay);
    } else if (this.timePeriod === 'week') {
      this.showCalendarWeek(this.currentYear, this.currentMonth, this.differenceWeek);
    } else {
      // month
      this.showCalendarMonth(this.currentYear, this.currentMonth);
    }
  }

  renderCalendar() {
    if (this.timePeriod === 'day') {
      this.showCalendarDay(this.currentYear, this.currentMonth, this.currentDate, this.differenceDay);
    } else if (this.timePeriod === 'week') {
      this.showCalendarWeek(this.currentYear, this.currentMonth, this.differenceWeek);
    } else {
      // month
      this.showCalendarMonth(this.currentYear, this.currentMonth);
    }
  }

  changeTimePeriod(event) {
    this.jump_today();
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
        this.dateFrom = new Date(firstDate);
        this.dateTo = new Date(firstDate);
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
        this.dateFrom = new Date(firstDate);
        this.dateTo = new Date(lastDate);
      }
    } else {
      // month
      if (month !== null && year !== null) {
        this.titleToolbar = this.months[month] + ', ' + year;
        const monthStart = new Date(year, month, 1);
        const monthEnd = new Date(year, month, new Date(year, month + 1, 0).getDate());
        this.dateFrom = monthStart;
        this.dateTo = monthEnd;
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

    // Create cells in calendar-tbody
    for (let i = this.firstTime; i <= this.lastTime; i++) {
      let row = document.createElement('tr');
      let cellTime = document.createElement('td');
      cellTime.classList.add('td-time');
      cellTime.id = i.toString();
      cellTime.textContent = `${i < 10 ? `0${i}` : i}:00 - ${i + 1 < 10 ? `0${i + 1}` : i + 1}:00`;
      let cell = document.createElement('td');
      cell.classList.add('td-day');
      cell.id = `${year}-${('0' + month).slice(-2)}-${('0' + firstDate.getDate()).slice(-2)}-${('0' + i).slice(-2)}`;

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
    this.loadData();
  }

  showCalendarWeek(year, month, week) {
    let firstDay = this.todayFormat.getDate() - this.todayFormat.getDay() + 1 + 7 * week; // First day is the day of the month - the day of the week
    let lastDay = firstDay + 6; // last day is the first day + 6
    let firstDate = new Date(year, month, firstDay);
    let lastDate = new Date(year, month, lastDay);

    this.setTitleToolbar(firstDate, lastDate);

    this.showCalendarThead(new Date(firstDate));

    this.calendarTbodyEl.innerHTML = ''; // Clear calendar-tbody
    this.calendarTableEl.style.tableLayout = 'fixed';

    // Create cells in calendar-tbody
    let row = document.createElement('tr');
    for (let i = 0; i < this.daysOfWeek.length; i++) {
      let cell = document.createElement('td');
      cell.classList.add('td-week');
      cell.id = `${year}-${('0' + month).slice(-2)}-${('0' + (firstDay + i)).slice(-2)}`;
      /// td-week click
      cell.addEventListener('click', () => {
        this.timePeriod = 'day';
        this.differenceDay = 0;
        this.currentDate = firstDay + i;
        this.showCalendarDay(year, month, this.currentDate, this.differenceDay);
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

    // Data Processing 
    this.loadData();
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
          cell.id = `${year}-${('0' + month).slice(-2)}-${('0' + date).slice(-2)}`;
          /// td-month click
          cell.addEventListener('click', () => {
            this.timePeriod = 'day';
            this.differenceDay = 0;
            this.currentDate = parseInt(cell.id.slice(-2));
            this.showCalendarDay(year, month, this.currentDate, this.differenceDay);
          });

          cell_dateText.textContent = ('0' + date).slice(-2);

          date++;
        }
        cell.appendChild(cell_dateText);
        cell.appendChild(cell_dateEvent);
        row.appendChild(cell);
      }
      this.calendarTbodyEl.appendChild(row);
    }

    // Data Processing 
    this.loadData();
  }

  loadAppointmentToCalendar() {
    this.dataAppointments = this.dataAppointments.map(v => ({
      ...v,
      dateFormat: new Date(v.date).setHours(0, 0, 0, 0),
      dateHour: v.date.getHours()
    }));

    this.groupByDataAppointments(); // group by 

    if (this.timePeriod === 'day') {
      let dataAppointmentsGroupedHour = null;
      if (this.dataAppointmentsGrouped !== null) {
        let dateFormat = this.dateFrom.setHours(0);
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

      if (dataAppointmentsGroupedHour !== null) {
        const year = this.dateFrom.getFullYear();
        const month = this.dateFrom.getMonth();
        const date = this.dateFrom.getDate();

        for (let key in dataAppointmentsGroupedHour) {
          const idCell = `${year}-${('0' + month).slice(-2)}-${('0' + date).slice(-2)}-${('0' + key).slice(-2)}`;
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
      if (this.dataAppointmentsGrouped !== null) {
        for (let key in this.dataAppointmentsGrouped) {
          if (parseInt(key) >= this.dateFrom.setHours(0) && parseInt(key) <= this.dateTo.setHours(0)) {
            const dateKey = new Date(parseInt(key));
            const year = dateKey.getFullYear();
            const month = dateKey.getMonth();
            const date = dateKey.getDate();
            const idCell = `${year}-${('0' + month).slice(-2)}-${('0' + date).slice(-2)}`;
            let cell_dateEventEl = document.querySelector(`[id='${idCell}'] .list-data-event-week`);
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
      if (this.dataAppointmentsGrouped !== null) {
        for (let key in this.dataAppointmentsGrouped) {
          if (parseInt(key) >= this.dateFrom.setHours(0) && parseInt(key) <= this.dateTo.setHours(0)) {
            const dateKey = new Date(parseInt(key));
            const year = dateKey.getFullYear();
            const month = dateKey.getMonth();
            const date = dateKey.getDate();
            const idCell = `${year}-${('0' + month).slice(-2)}-${('0' + date).slice(-2)}`;
            let cell_dateEventEl = document.querySelector(`[id='${idCell}'] .date-event`);
            let dataAppointmentsGroupedState = this.dataAppointmentsGrouped[key].reduce(function (r, a) {
              r[a.state] = r[a.state] || [];
              r[a.state].push(a);
              return r;
            }, Object.create(null));
            let eventMonthString = '';
            for (let key in dataAppointmentsGroupedState) {
              eventMonthString += this.getEventMonth(key, dataAppointmentsGroupedState[key].length);
            }
            cell_dateEventEl.innerHTML = eventMonthString;
          }
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
    customerNameEl.classList.add("customer-name", "text-primary");
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
      // case 'overdue':
      //   statusShow = 'Quá hạn';
      //   classBg = 'bg-overdue';
      //   break;
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

  createUpdateAppointment(id = null) {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.appointId = id;
    modalRef.componentInstance.title = id ? "Cập nhật lịch hẹn" : "Đặt lịch hẹn";
    modalRef.result.then(result => {
      this.renderCalendar(); // Render Calendar
    }, () => { });
  }

  // form tiếp nhận
  receiveAppointment(id = null) {
    if (id) {
      const appoint = this.dataAppointments.find(value => value.id === id)
      if (appoint && (appoint.state === 'cancel' || appoint.isLate)) {
        this.notificationService.show({
          content: 'Không thể tiếp nhận lịch hẹn ở trạng thái hủy hẹn và quá hạn',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'error', icon: true }
        });
        return false;
      }
      const modalRef = this.modalService.open(CustomerReceipCreateUpdateComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.appointId = id;
      modalRef.componentInstance.title = "Tiếp nhận";
      modalRef.result.then(result => {
        this.renderCalendar(); // Render Calendar
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      }, () => { });
    }
  }
}
