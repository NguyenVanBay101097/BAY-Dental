import { Component, Input, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AppointmentDisplay } from 'src/app/appointment/appointment';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { AppointmentCreateUpdateComponent } from 'src/app/shared/appointment-create-update/appointment-create-update.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PartnerService } from '../../partner.service';

@Component({
  selector: 'app-partner-overview-appointment',
  templateUrl: './partner-overview-appointment.component.html',
  styleUrls: ['./partner-overview-appointment.component.css']
})
export class PartnerOverviewAppointmentComponent implements OnInit {

  @Input() customerAppointment: AppointmentDisplay;
  @Input() partnerId: any;

  constructor(
    private modalService: NgbModal,
    private appointmentService: AppointmentService,
    private partnerService: PartnerService,
    private notificationService: NotificationService,
  ) {
  }

  ngOnInit() {
  }

  addAppointment() {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.defaultVal = {
      partnerId: this.partnerId
    };

    modalRef.result.then(result => {
      if (result) {
        this.getNextAppointment();
        this.notificationService.show({
          content: "Tạo mới lịch hẹn thành công!.",
          hideAfter: 3000,
          position: { horizontal: "center", vertical: "top" },
          animation: { type: "fade", duration: 400 },
          type: { style: "success", icon: true },
        });
      }
    }, () => {
    });
  }

  removeAppointment() {
    if (this.customerAppointment) {
      const modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Xóa lịch hẹn';
      modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa lịch hẹn này?';
      modalRef.result.then(() => {
        this.appointmentService.removeAppointment(this.customerAppointment.id).subscribe(() => {
          this.getNextAppointment();
          this.notificationService.show({
            content: "Xóa lịch hẹn thành công!.",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
        })
      }, () => { });
    }
  }

  editAppointment() {
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.appointId = this.customerAppointment.id;
    modalRef.result.then(() => {
      this.getNextAppointment();
      this.notificationService.show({
        content: "Sửa lịch hẹn thành công!.",
        hideAfter: 3000,
        position: { horizontal: "center", vertical: "top" },
        animation: { type: "fade", duration: 400 },
        type: { style: "success", icon: true },
      });
    }, () => { });
  }

  getNextAppointment() {
    this.partnerService.getNextAppointment(this.partnerId).subscribe(rs => {
      this.customerAppointment = rs;
    });
  }

  getState(value) {
    switch (value) {
      case 'waiting':
        return 'Chờ khám';
      case 'examination':
        return 'Đang khám';
      case 'done':
        return 'Hoàn thành';
      case 'cancel':
        return 'Đã hủy';
      default:
        return 'Đang hẹn';
    }
  }
}
