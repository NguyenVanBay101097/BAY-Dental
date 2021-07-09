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
import { debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
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

  public today: Date = new Date(new Date().toDateString());
  public next3days: Date = new Date(new Date(new Date().setDate(new Date().getDate() + 3)).toDateString());

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

  events = [];
  calendarApi: Calendar;
  calendarPlugins = [dayGridPlugin, timeGrigPlugin, interactionPlugin];
  @ViewChild('fullcalendar',{static: true}) calendarComponent: FullCalendarComponent;
  titleDateToolbar: string = "";
  viewToolbar: string = "dayGridMonth";
  validRange: DateRangeInput;
  listData = [];
  doctors = [];

  constructor (
    private appointmentService: AppointmentService,
    private intlService: IntlService,
    private modalService: NgbModal,
    private dotkhamService: DotKhamService,
    private notificationService: NotificationService,
    private router: Router,
    private employeeService: EmployeeService,
    private checkPermissionService: CheckPermissionService
  ) { }
  ngAfterViewInit(): void {
    //Called after ngAfterContentInit when the component's view has been initialized. Applies to components only.
    //Add 'implements AfterViewInit' to the class.
    this.calendarApi = this.calendarComponent.getApi();
    this.titleDateToolbar = this.getDateToolbar();
// console.log(this.calendarApi.view.activeStart);
// console.log(this.calendarApi.view.activeEnd);

    //     document.getElementsByClassName("fc-next-button")[0].addEventListener("click",()=> {

    //     });
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
      });

    // this.loadListEmployees();
    this.loadDoctorList();
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
  }

  onChangeDate(e) {
    this.dateFrom = e.dateFrom;
    this.dateTo = e.dateTo;
    this.loadData();
  }

  onChangeState(state) {
    this.state = state;
    this.loadData();
  }

  createAppointment() {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.result.then(result => {
      this.loadData();
    }, () => { });

    modalRef.componentInstance.getBtnDeleteObs.subscribe(() => {
      this.loadData();
    })
  }

  refreshData() {

    this.loadData();
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
      this.listData = result.items;
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
    this.events = [];
    for (var i = 0; i < paged.items.length; i++) {
      var item = paged.items[i];
      var d = new Date();
      d.setHours(0, 0, 0, 0);
      this.events = this.events.concat([
        <EventInput>{
          title: item.time + '\n' + item.partnerName,
          date: this.formatDate(item.date),
          backgroundColor: new Date(item.date) >= d ? (this.states.find(x => x.value == item.state) ? this.states.find(x => x.value == item.state).bgColor : '') : '#FFC107',
          id: item.id,
          textColor: 'white'
        }
      ]);
    }
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
  dateToday() {
    var currentStart = this.calendarApi.view.currentStart;
    var currentEnd = this.calendarApi.view.currentEnd;
    var today = this.calendarApi.getNow();
    if (today < currentStart || today > currentEnd) 
      this.calendarApi.today();
  }
  datePrev() {
    this.calendarApi.prev();
    this.titleDateToolbar = this.getDateToolbar();
  }
  dateNext() {
    this.calendarApi.next();
    this.titleDateToolbar = this.getDateToolbar();
  }
  getDateToolbar() {
    var currentStart = this.calendarApi.view.currentStart;
    currentStart.setDate(currentStart.getDate() + 1);
    var currentStartString = ("0" + currentStart.getUTCDate()).slice(-2) + "/" + 
                            ("0" + (currentStart.getUTCMonth()+1)).slice(-2) + "/" + 
                            currentStart.getUTCFullYear();

    var currentEnd = this.calendarApi.view.currentEnd;
    var currentEndString = ("0" + currentEnd.getUTCDate()).slice(-2) + "/" + 
                          ("0" + (currentEnd.getUTCMonth()+1)).slice(-2) + "/" + 
                          currentEnd.getUTCFullYear();
                                                
    if (currentStartString == currentEndString) {
      return currentStartString;
    } else {
      return `${currentStartString} - ${currentEndString}`;
    }
  }
  changeViewToolbar(event) {
    this.viewToolbar = event;
    this.calendarApi.changeView(event);
    this.titleDateToolbar = this.getDateToolbar();
  }
}
