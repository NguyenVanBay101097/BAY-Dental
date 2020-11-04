import { Component, Input, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AppointmentDisplay } from 'src/app/appointment/appointment';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { AppointmentCreateUpdateComponent } from 'src/app/shared/appointment-create-update/appointment-create-update.component';
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
    const modalRef = this.modalService.open(AppointmentCreateUpdateComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
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

  getNextAppointment() {
    this.partnerService.getNextAppointment(this.partnerId).subscribe(rs => {
        this.customerAppointment = rs;
    });
  }

}
