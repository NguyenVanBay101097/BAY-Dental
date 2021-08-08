import { CustomerReceiptBasic, CustomerReceiptDisplay, CustomerReceiptStatePatch } from './../../../customer-receipt/customer-receipt.service';
import { EmployeeService } from 'src/app/employees/employee.service';
import { EmployeeSimple } from 'src/app/employees/employee';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin, of, Subject, pipe } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, subscribeOn, mergeMap } from 'rxjs/operators';
import { AppointmentGetCountVM } from 'src/app/appointment/appointment';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { CustomerReceiptPaged, CustomerReceiptService } from 'src/app/customer-receipt/customer-receipt.service';
import { CustomerReceipCreateUpdateComponent } from '../../customer-receip-create-update/customer-receip-create-update.component';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { EmployeePaged } from 'src/app/employees/employee';
import { groupBy } from 'lodash';

@Component({
  selector: 'app-dashboard-customer-receipt-list-today',
  templateUrl: './dashboard-customer-receipt-list-today.component.html',
  styleUrls: ['./dashboard-customer-receipt-list-today.component.css']
})
export class DashboardCustomerReceiptListTodayComponent implements OnInit {
  @Input() customerReceipts: CustomerReceiptBasic[] = [];
  @Input() initialListEmployees: any = [];

  @Output() onUpdateCREvent = new EventEmitter<any>();
  @Output() onCreateCREvent = new EventEmitter<any>();

  listCustomerReceipt: CustomerReceiptBasic[];
  filteredEmployeesDoctor: any[] = [];
  loading = false;
  hoveredIndex: -1;
  search: string;
  limit = 1000;
  skip = 0;
  total: number;
  listEmployees: EmployeeSimple[] = [];
  stateFilter: string = '';
  doctorFilter: string;
  stateCount: any = {};
  states: any[] = [
    { value: '', text: 'Tất cả' },
    { value: 'waiting', text: 'Chờ khám' },
    { value: 'examination', text: 'Đang khám' },
    { value: 'done', text: 'Hoàn thành' },
  ]

  @ViewChild('employeeCbx', { static: true }) employeeCbx: ComboBoxComponent;

  public today: Date = new Date(new Date().toDateString());

  constructor(private intlService: IntlService, private customerReceiptService: CustomerReceiptService,
    private employeeService: EmployeeService,
    private notifyService: NotifyService,
    private modalService: NgbModal) { }

  ngOnChanges(changes: SimpleChanges): void {
    this.loadData();
    this.loadStateCount();
  }

  ngOnInit() {
    this.loadData();
    this.loadListEmployees();
    this.loadStateCount();
  }

  loadData() {
    let res = this.customerReceipts;
    if (this.stateFilter) {
      res = res.filter(x => x.state.includes(this.stateFilter));
    }

    if (this.doctorFilter) {
      res = res.filter(x => x.doctorId.includes(this.doctorFilter));
    }

    if (this.search) {
      res = res.filter(x => this.RemoveVietnamese(x.partnerName).includes(this.search) || x.partnerPhone.includes(this.search));
    }

    this.listCustomerReceipt = res;
    this.filteredEmployeesDoctor = this.initialListEmployees;
  }

  onChangeSearch(value) {
    this.search = value ? value : '';
    this.loadData();
    this.loadStateCount();
  }

  loadStateCount() {
    forkJoin(this.states.map(x => {
      var val = new AppointmentGetCountVM();
      val.state = x.value;
      val.dateFrom = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
      val.dateTo = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
      return this.customerReceiptService.getCount(val).pipe(
        switchMap(count => of({ state: x.value, count: count }))
      );
    })).subscribe((result) => {
      result.forEach(item => {
        this.stateCount[item.state] = item.count;
      });
    });
  }

  setStateFilter(state) {
    this.stateFilter = state || '';
    this.loadData();
  }

  loadListEmployees() {
    this.searchEmployees().subscribe(
      rs => {
        this.listEmployees = rs.items;
      });
  }

  onChangeDoctor(val) {
    this.doctorFilter = val ? val.id : '';
    this.loadData();
    this.loadStateCount();
  }

  onEmployeeFilter(value) {
    this.filteredEmployeesDoctor = this.initialListEmployees
      .filter((s) => s.isDoctor == true && s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  searchEmployees(search?: string) {
    var paged = new EmployeePaged();
    paged.search = search || '';
    paged.isDoctor = true;
    return this.employeeService.getEmployeePaged(paged);
  }

  createItem() {
    let modalRef = this.modalService.open(CustomerReceipCreateUpdateComponent, { size: "lg", windowClass: "o_technical_modal modal-appointment", keyboard: false, backdrop: "static", });
    modalRef.componentInstance.title = "Tiếp nhận";
    modalRef.result.then(res => {
      this.notifyService.notify('success', 'Lưu thành công');
      this.onCreateCREvent.emit(res);
      this.loadStateCount();
    }, () => { }
    );
  }

  editItem(item) {
    const modalRef = this.modalService.open(CustomerReceipCreateUpdateComponent, { size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Cập nhật tiếp nhận";
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(res => {
      this.notifyService.notify('success', 'Lưu thành công');
      this.onUpdateCREvent.emit(res);
      this.loadStateCount();
    }, () => {
    });
  }

  onChangeState(item, val) {
    var res = new CustomerReceiptStatePatch();
    res.state = val.state;
    res.isNoTreatment = val.isNoTreatment;
    res.reason = val.reason != null ? val.reason : null;
    res.dateExamination = val.state == 'examination' ? this.intlService.formatDate(new Date(), 'yyyy-MM-ddTHH:mm:ss') : null;
    res.dateDone = val.state == 'done' ? this.intlService.formatDate(new Date(), 'yyyy-MM-ddTHH:mm:ss') : null;
    this.customerReceiptService.patchState(item.id, res).pipe(
      mergeMap((rs: any) => {
        return this.customerReceiptService.get(item.id);
      })
    ).subscribe((rs: CustomerReceiptDisplay) => {
      this.notifyService.notify('success', 'Lưu thành công');
      var basic = this.getBasic(rs);
      this.onUpdateCREvent.emit(basic);
      this.loadStateCount();
    });

  }

  getBasic(res: CustomerReceiptDisplay) {
    var basic = new CustomerReceiptBasic();
    basic.id = res.id;
    basic.doctorId = res.doctor ? res.doctorId : null;
    basic.doctorName = res.doctor ? res.doctor.name : '';
    basic.partnerId = res.partnerId;
    basic.partnerName = res.partner.name;
    basic.partnerPhone = res.partner.phone;
    basic.dateWaiting = res.dateWaiting ? res.dateWaiting : null;
    basic.dateExamination = res.dateExamination ? res.dateExamination : null;
    basic.dateDone = res.dateDone ? res.dateDone : null;
    basic.state = res.state;
    return basic;
  }

  getTime(item) {
    var state = item.state;
    switch (state) {
      case 'waiting':
        return item.dateWaiting;
      case 'examination':
        return item.dateExamination;
      case 'done':
        return item.dateDone;
    }
  }

  getState(state) {
    switch (state) {
      case 'waiting':
        return 'Chờ khám';
      case 'examination':
        return 'Đang khám';
      case 'done':
        return 'Hoàn thánh';
    }
  }

  getColorState(state) {
    switch (state) {
      case 'waiting':
        return 'badge-outline-warning';
      case 'examination':
        return 'badge-outline-info';
      case 'done':
        return 'badge-outline-success';
    }
  }

  RemoveVietnamese(text) {
    text = text.toLowerCase().trim();
    text = text.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    text = text.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    text = text.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    text = text.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    text = text.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    text = text.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    text = text.replace(/đ/g, "d");
    // Some system encode vietnamese combining accent as individual utf-8 characters
    text = text.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // Huyền sắc hỏi ngã nặng 
    text = text.replace(/\u02C6|\u0306|\u031B/g, ""); // Â, Ê, Ă, Ơ, Ư
    return text;
  }


}
