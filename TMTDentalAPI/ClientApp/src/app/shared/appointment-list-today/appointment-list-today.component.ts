import { AppointmentBasic } from './../../appointment/appointment';
import { DashboardReportService, GetDefaultRequest } from './../../core/services/dashboard-report.service';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { state } from '@angular/animations';
import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { forkJoin, of, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, mergeMap, switchMap } from 'rxjs/operators';
import { AppointmentGetCountVM, AppointmentPaged, AppointmentPatch, AppointmentStatePatch, DateFromTo } from 'src/app/appointment/appointment';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { EmployeeService } from 'src/app/employees/employee.service';
import { AppointmentCreateUpdateComponent } from '../appointment-create-update/appointment-create-update.component';
import { CustomerReceipCreateUpdateComponent } from '../customer-receip-create-update/customer-receip-create-update.component';

@Component({
  selector: 'app-appointment-list-today',
  templateUrl: './appointment-list-today.component.html',
  styleUrls: ['./appointment-list-today.component.css']
})
export class AppointmentListTodayComponent implements OnInit {
  @Input() appointments: AppointmentBasic[] = [];

  @Output() onUpdateAPEvent = new EventEmitter<any>();
  @Output() onCreateAPEvent = new EventEmitter<any>();
  @Output() onDeleteAPEvent = new EventEmitter<any>();
  @Output() onCreateCREvent = new EventEmitter<any>();
  listAppointment: AppointmentBasic[];
  userId: string;
  limit = 1000;
  skip = 0;
  loading = false;
  opened = false;
  total: number;
  search: string;
  searchUpdate = new Subject<string>();
  public today: Date = new Date(new Date().toDateString());
  stateFilter: string = '';
  stateFilterOptions: any[] = [];
  stateCount: any = {};
  states: any[] = [
    { value: '', text: 'Tất cả' },
    { value: 'confirmed', text: 'Đang hẹn' },
    { value: 'arrived', text: 'Đã đến' },
    { value: 'cancel', text: 'Hủy hẹn' },
  ]

  constructor(private appointmentService: AppointmentService,
    private intlService: IntlService, private modalService: NgbModal,
    private notifyService: NotifyService,
    private dashboardReportService : DashboardReportService,
    private notificationService: NotificationService, private router: Router, private employeeService: EmployeeService) { }


  ngOnInit() {
    this.loadData();
    this.loadStateCount();

  }

  ngOnChanges(changes: SimpleChanges): void {
    this.loadData();
    this.loadStateCount();
  }

  loadData() {
    let res = this.appointments.sort();
    if (this.stateFilter) {
      res = res.filter(x => x.state.includes(this.stateFilter));
    }

    if (this.search) {
      res = res.filter(x => this.RemoveVietnamese(x.partnerName).includes(this.search) || x.partnerPhone.includes(this.search));
    }

    this.listAppointment = res;
  }

  onChangeOverState(value) {
    this.stateFilter = value;
    this.loadData();
  }

  setStateFilter(state: any) {
    this.stateFilter = state;
    this.loadData();
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
      return this.appointmentService.getCount(val).pipe(
        switchMap(count => of({ state: x.value, count: count }))
      );
    })).subscribe((result) => {
      result.forEach(item => {
        this.stateCount[item.state] = item.count;
      });
    });
  }

  createItem() {
    let modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { size: "lg", windowClass: "o_technical_modal modal-appointment", keyboard: false, backdrop: "static", });
    modalRef.componentInstance.title = "Đặt lịch hẹn";
    modalRef.result.then(res => {
      this.notifyService.notify('success','Lưu thành công');
      this.onCreateAPEvent.emit(res);
      this.loadStateCount();
    }, () => { }
    );
  }

  editItem(item) {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = "Cập nhật lịch hẹn";
    modalRef.componentInstance.appointId = item.id;
    modalRef.result.then(res => {
      if(res.isDetele){
        this.notifyService.notify('success','Xóa thành công');
        this.onDeleteAPEvent.emit(res);
      }else{
        this.notifyService.notify('success','Lưu thành công');
        this.onUpdateAPEvent.emit(res);
      }     
      this.loadStateCount();
    }, () => {
    });
  }

  createCustomerReceipt(item) {   
    this.dashboardReportService.getDefaultCustomerReceipt({appointmentId : item.id}).subscribe(res => {
      let modalRef = this.modalService.open(CustomerReceipCreateUpdateComponent, { size: "lg", windowClass: "o_technical_modal modal-appointment", keyboard: false, backdrop: "static", });
      modalRef.componentInstance.title = "Tiếp nhận";
      modalRef.componentInstance.appointId = item.id;
      modalRef.componentInstance.defaultData = res;
      modalRef.result.then(rs => {
        this.notifyService.notify('success','Lưu thành công');
        if(rs.appointment){
          this.onUpdateAPEvent.emit(rs.appointment);
        }

        if(rs.customerReceipt){
          this.onCreateCREvent.emit(rs.customerReceipt);
        }
        this.loadStateCount();
      }, () => { }
      );
    });
  }



  stateGet(state) {
    switch (state) {
      case 'confirmed':
        return 'Đang hẹn';
      case 'arrived':
        return 'Đã đến';
      case 'cancel':
        return 'Hủy hẹn';
      default:
        return 'Tất cả';
    }
  }

  getBgBoxContent(state) {
    switch (state) {
      case 'confirmed':
        return 'appointment-today-box_examination';
      case 'arrived':
        return 'appointment-today-box_arrived';
      case 'cancel':
        return 'appointment-today-box_cancel';
    }
  }

  getTextColorState(state) {
    switch (state) {
      case 'confirmed':
        return 'text-primary';
      case 'arrived':
        return 'text-success';
      case 'cancel':
        return 'text-danger';
    }
  }

  onChangeState(item, val) {
    var res = new AppointmentStatePatch();
    res.state = val.state;
    res.reason = val.reason != null ? val.reason : null;
    this.appointmentService.patchState(item.id, res).subscribe(() => {
      this.notificationService.show({
        content: 'Lưu thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });

      item.state = val.state;
      this.loadStateCount();
    });

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
