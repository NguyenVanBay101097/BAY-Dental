import { Component, DoCheck, EventEmitter, Input, IterableDiffer, IterableDiffers, OnInit, Output } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { AppointmentStatePatch } from 'src/app/appointment/appointment';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { ReceiveAppointmentService } from 'src/app/customer-receipt/receive-appointment.service';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { AppointmentCreateUpdateComponent } from '../appointment-create-update/appointment-create-update.component';
import { ReceiveAppointmentDialogComponent } from '../receive-appointment-dialog/receive-appointment-dialog.component';
import { AppointmentBasic } from './../../appointment/appointment';

@Component({
  selector: 'app-appointment-list-today',
  templateUrl: './appointment-list-today.component.html',
  styleUrls: ['./appointment-list-today.component.css']
})
export class AppointmentListTodayComponent implements OnInit, DoCheck {
  @Input() appointments: AppointmentBasic[] = [];

  @Output() onUpdateAPEvent = new EventEmitter<any>();
  @Output() onCreateAPEvent = new EventEmitter<any>();
  @Output() onDeleteAPEvent = new EventEmitter<any>();
  @Output() onCreateCREvent = new EventEmitter<any>();
  @Output() successReceiveEvent = new EventEmitter<any>();
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
  stateCount: any[] = [];
  states: any[] = [
    { value: '', text: 'Tất cả' },
    { value: 'confirmed', text: 'Đang hẹn' },
    { value: 'done', text: 'Đã đến' },
    { value: 'cancel', text: 'Hủy hẹn' },
  ]

  iterableDiffer: IterableDiffer<any>;
  colorsSelected: number[] = [];
  constructor(
    private appointmentService: AppointmentService,
    private modalService: NgbModal,
    private notifyService: NotifyService,
    private notificationService: NotificationService,
    private receiveAppointmentService: ReceiveAppointmentService,
    private iterableDiffers: IterableDiffers
    ) { 
      this.iterableDiffer = this.iterableDiffers.find([]).create(null);
    }


  ngOnInit() {
    this.loadData();
  }

  ngDoCheck() {
    var changes = this.iterableDiffer.diff(this.appointments);
    if (changes) {
      this.loadData();
    }
  }

  loadData() {
    let res = this.appointments.sort();
    res.forEach(x => {
      if (x.color == null)
        x.color = "0";
    });
    if (this.stateFilter) {
      res = res.filter(x => x.state.includes(this.stateFilter));
    }

    if (this.search) {
      res = res.filter(x => x.partnerName && this.RemoveVietnamese(x.partnerName).includes(this.RemoveVietnamese(this.search)) || x.partnerPhone && x.partnerPhone.includes(this.search) || x.doctorName && this.RemoveVietnamese(x.doctorName).includes(this.RemoveVietnamese(this.search)));
    }

    if (this.colorsSelected.length > 0) {
      res = res.filter(x => {
        return this.colorsSelected.includes(+x.color);
      })
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
    // this.loadStateCount();
  }

  getStateCount(state) {
    if (state) {
      return this.appointments.filter(s => s.state == state).length;
    } else {
      return this.appointments.length;
    }
  }

  createItem() {
    let modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { size: "lg", windowClass: "o_technical_modal modal-appointment", keyboard: false, backdrop: "static", });
    modalRef.componentInstance.title = "Đặt lịch hẹn";
    modalRef.result.then(res => {
      this.notifyService.notify('success','Lưu thành công');
      this.onCreateAPEvent.emit(res);
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
    }, () => {
    });
  }

  createCustomerReceipt(item) {   
    this.receiveAppointmentService.defaultGet(item.id).subscribe(result => {
      let modalRef = this.modalService.open(ReceiveAppointmentDialogComponent, { size: "lg", windowClass: "o_technical_modal modal-appointment", keyboard: false, backdrop: "static", });
      modalRef.componentInstance.receiveAppointmentDisplay = result;
      modalRef.result.then((res) => {
        this.notifyService.notify('success','Lưu thành công');
        this.successReceiveEvent.emit({
          appointment: item,
          customerReceipt: res
        });
      }, () => { }
      );
    });
   
  }



  stateGet(state) {
    switch (state) {
      case 'confirmed':
        return 'Đang hẹn';
      case 'done':
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
      case 'done':
        return 'appointment-today-box_arrived';
      case 'cancel':
        return 'appointment-today-box_cancel';
    }
  }

  getBackgroundColor(item) {
    return this.getTextColorState(item.state) + ' ' + 'appointment_background_color_'+item.color;
  }

  getTextColorState(state) {
    switch (state) {
      case 'confirmed':
        return 'event-confirmed';
      case 'done':
        return 'event-done';
      case 'cancel':
        return 'event-cancel';
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
    });

  }

  RemoveVietnamese(text : string) {
    text = text.toLowerCase();
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
    text = text.replace(/\s/g, "");;
    return text;
  }

  clickColor(color) {
    let index = this.colorsSelected.findIndex(x => x == color);
    if (index != -1)
      this.colorsSelected.splice(index,1);
    else 
      this.colorsSelected.push(color);

    this.loadData();
  }

  isContainColor(color) {
    return this.colorsSelected.findIndex(x => x == color) != -1;
  }

}
