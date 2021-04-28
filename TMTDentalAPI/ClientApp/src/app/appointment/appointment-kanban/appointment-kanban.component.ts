import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
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

@Component({
  selector: 'app-appointment-kanban',
  templateUrl: './appointment-kanban.component.html',
  styleUrls: ['./appointment-kanban.component.css']
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

  @ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;
  listEmployees: EmployeeBasic[] = [];
  filterEmployee: any;

  // permission
  showAppointmentCreate = this.checkPermissionService.check(["Basic.Appointment.Create"]);
  showAppointmentEdit = this.checkPermissionService.check(["Basic.Appointment.Edit"]);
  showAppointmentDelete = this.checkPermissionService.check(["Basic.Appointment.Delete"]);
  showEmployeeRead = this.checkPermissionService.check(["Catalog.Employee.Read"]);
  showCustomerLink = this.checkPermissionService.check(["Basic.Partner.Read"]);

  public today: Date = new Date(new Date().toDateString());
  public next3days: Date = new Date(new Date(new Date().setDate(new Date().getDate() + 3)).toDateString());

  appointmentByDate: { [id: string]: AppointmentBasic[]; } = {};
  constructor(private appointmentService: AppointmentService,
    private intlService: IntlService, private modalService: NgbModal, private dotkhamService: DotKhamService,
    private notificationService: NotificationService, private router: Router, private employeeService: EmployeeService, 
    private checkPermissionService: CheckPermissionService
  ) { }

  ngOnInit() {
    this.dateFrom = this.today;
    this.dateTo = this.next3days;
    this.dateList = this.getDateList();
    this.loadData();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadData();
      });

    this.loadListEmployees();

    this.employeeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => this.employeeCbx.loading = true),
      switchMap(val => this.searchEmployees(val))
    ).subscribe(
      rs => {
        this.listEmployees = rs.items;
        this.employeeCbx.loading = false;
      }
    )
  }

  searchEmployees(search?: string) {
    var paged = new EmployeePaged();
    paged.search = search || '';
    paged.isDoctor = true;
    return this.employeeService.getEmployeePaged(paged);
  }

  valueChangeUser(employee) {
    this.filterEmployee = employee;
    this.loadData();
  }

  loadListEmployees() {
    this.searchEmployees().subscribe(
      rs => {
        this.listEmployees = rs.items;
      });
  }


  onDateSearchChange(filter) {
    this.dateFrom = filter.dateFrom;
    this.dateTo = filter.dateTo;
    this.dateList = this.getDateList();
    this.loadData();
  }


  onStateSearchChange(state) {
    this.state = state;
    this.loadData();
  }

  createAppointment() {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.result.then(result => {
      this.loadData();
    }, () => { });
  }

  refreshData() {
    this.loadData();
  }


  loadData() {
    this.resetData();
    var val = new AppointmentPaged();
    val.limit = 1000;
    if (this.state) {
      val.state = this.state;
    }
    if (this.search) {
      val.search = this.search;
    }

    val.doctorId = this.filterEmployee ? this.filterEmployee.id : '';

    val.dateTimeFrom = this.intlService.formatDate(this.dateList[0], 'yyyy-MM-dd');
    val.dateTimeTo = this.intlService.formatDate(this.dateList[this.dateList.length - 1], 'yyyy-MM-dd');
    this.appointmentService.getPaged(val).subscribe((result: any) => {
      this.addAppointments(result);
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

  addAppointments(paged: PagedResult2<AppointmentBasic>) {
    for (var i = 0; i < paged.items.length; i++) {
      var item = paged.items[i];
      var date = new Date(item.date);
      var key = date.toDateString();
      if (!this.appointmentByDate[key]) {
        this.appointmentByDate[key] = [];
      }

      this.appointmentByDate[key].push(item);
    }
  }

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
    val.limit = 0;
    if (this.state) {
      val.state = this.state;
    }
    if (this.search) {
      val.search = this.search;
    }

    val.doctorId = this.filterEmployee ? this.filterEmployee.id : '';

    val.dateTimeFrom = this.intlService.formatDate(this.dateList[0], 'yyyy-MM-dd');
    val.dateTimeTo = this.intlService.formatDate(this.dateList[this.dateList.length - 1], 'yyyy-MM-dd');
    this.appointmentService.exportExcel(val).subscribe((result: any) => {
      let filenam = 'DanhSachLichHen';
      let newBlob = new Blob([result], {type:"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
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
}
