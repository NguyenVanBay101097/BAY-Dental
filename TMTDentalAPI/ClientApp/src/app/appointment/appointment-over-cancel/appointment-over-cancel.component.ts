import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { NgbDropdownToggle, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { forkJoin, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { EmployeeBasic, EmployeePaged } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { AppointmentCreateUpdateComponent } from 'src/app/shared/appointment-create-update/appointment-create-update.component';
import { AppointmentPaged } from '../appointment';
import { AppointmentService } from '../appointment.service';

@Component({
  selector: 'app-appointment-over-cancel',
  templateUrl: './appointment-over-cancel.component.html',
  styleUrls: ['./appointment-over-cancel.component.css']
})
export class AppointmentOverCancelComponent implements OnInit {

  @ViewChild('dropdownMenuBtn', { static: false }) dropdownMenuBtn: NgbDropdownToggle;
  @ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;

  gridData: GridDataResult;
  title: string = "Danh sách lịch hẹn quá hạn/hủy hẹn"
  dateFrom: Date;
  dateTo: Date;
  limit: number = 20;
  offset: number = 0
  state: string = "cancel,confirmed";
  search: string;
  searchUpdate = new Subject<string>();
  listEmployees: EmployeeBasic[] = [];
  stateCount: any = {};
  stateConfirmed: boolean = false;
  stateCancel: boolean = false;
  filterEmployee: any;
  loading: boolean = false;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() - 1, 0).getDate())).toDateString());



  constructor(
    private appointmentService: AppointmentService,
    private intlService: IntlService,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private employeeService: EmployeeService
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.loadDataFromApi();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.offset = 0;
        this.loadDataFromApi();
        this.getCountState();
      });

    this.loadListEmployees();
    this.getCountState();
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

  getCountState() {
    var dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    var dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
    let confirmed = this.appointmentService.getCount({ state: "confirmed", search: this.search || "", doctorId: this.filterEmployee ? this.filterEmployee.id : null, dateFrom, dateTo: dateTo, cancel: true });
    let cancel = this.appointmentService.getCount({ state: "cancel", search: this.search || "", doctorId: this.filterEmployee ? this.filterEmployee.id : null, dateFrom, dateTo: dateTo, cancel: true });

    forkJoin([confirmed, cancel]).subscribe(results => {
      this.stateCount['confirmed'] = results[0] || 0;
      this.stateCount['cancel'] = results[1] || 0;
      this.stateCount['all'] = this.stateCount['confirmed'] + this.stateCount['cancel'];
    });
  }

  loadDataFromApi() {
    var val = new AppointmentPaged();
    val.limit = this.limit;
    val.offset = this.offset;
    if (this.state) {
      val.state = this.state;
    }
    if (this.search) {
      val.search = this.search;
    }
    val.doctorId = this.filterEmployee ? this.filterEmployee.id : '';
    val.dateTimeFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
    val.dateTimeTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
    val.cancel = true;
    this.appointmentService.getPaged(val).pipe(
      map((response: any) =>
      (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
      console.log(res);

    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  pageChange(event: PageChangeEvent): void {
    this.offset = event.skip;
    this.loadDataFromApi();
  }

  valueChangeUser(employee) {
    this.filterEmployee = employee;
    this.offset = 0;
    this.getCountState();
    this.loadDataFromApi();
  }

  loadListEmployees() {
    this.searchEmployees().subscribe(
      rs => {
        this.listEmployees = rs.items;
      });
  }

  editAppointment(appointment: any) {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { size: 'xl', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.appointId = appointment.id;
    modalRef.result.then(() => {
      this.loadDataFromApi();
      this.getCountState();
    }, () => {
    });
  }

  searchEmployees(search?: string) {
    var paged = new EmployeePaged();
    paged.search = search || '';
    paged.isDoctor = true;
    return this.employeeService.getEmployeePaged(paged);
  }

  onDateSearchChange(filter) {
    this.dateFrom = filter.dateFrom;
    this.dateTo = filter.dateTo;
    this.offset = 0;
    this.loadDataFromApi();
    this.getCountState();
  }

  setStateFilter(value) {
    if (value != 'all') {
      this.state = value;
      this.getCountState();
      this.loadDataFromApi();
    } else if (value == 'all') {
      this.state = "confirmed,cancel";
      this.loadDataFromApi();
      this.getCountState();
    }
  }

  computeNameSerivc(items) {

  }

  onExportExcelFile() {
    var val = new AppointmentPaged();
    val.limit = this.limit;
    val.offset = this.offset;
    if (this.state) {
      val.state = this.state;
    }
    if (this.search) {
      val.search = this.search;
    }
    val.doctorId = this.filterEmployee ? this.filterEmployee.id : '';
    val.dateTimeFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
    val.dateTimeTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
    val.cancel = true;
    this.appointmentService.exportExcel(val).subscribe((result: any) => {
      let filenam = 'Danh_sach_lich_hen_quan_han_va_huy_hen';
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
}
