import { Component, OnInit } from '@angular/core';
import { AppointmentBasic } from '../appointment';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-appointment-detail-dialog',
  templateUrl: './appointment-detail-dialog.component.html',
  styleUrls: ['./appointment-detail-dialog.component.css']
})
export class AppointmentDetailDialogComponent implements OnInit {
  appointment: AppointmentBasic;
  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit() {
  }

  getState() {
    switch (this.appointment.state) {
      case 'done':
        return 'Đã tới';
      case 'cancel':
        return 'Đã hủy';
      default:
        return 'Đang hẹn';
    }
  }
}
