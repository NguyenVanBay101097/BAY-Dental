import { Component, OnInit } from '@angular/core';
import { PartnerDisplay } from '../partner-simple';
import { PartnerService } from '../partner.service';
import { ActivatedRoute } from '@angular/router';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { AppointmentDisplay } from 'src/app/appointment/appointment';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PartnerCustomerCuDialogComponent } from 'src/app/shared/partner-customer-cu-dialog/partner-customer-cu-dialog.component';

@Component({
  selector: 'app-partner-customer-profile',
  templateUrl: './partner-customer-profile.component.html',
  styleUrls: ['./partner-customer-profile.component.css']
})
export class PartnerCustomerProfileComponent implements OnInit {
  customerInfo: PartnerDisplay;
  customerAppointment: AppointmentDisplay;
  id: string;

  constructor(
    private partnerService: PartnerService,
    private activeRoute: ActivatedRoute,
    private serviceAppointment: AppointmentService,
    private modalService: NgbModal
  ) {}

  ngOnInit() {
    this.id = this.activeRoute.parent.snapshot.paramMap.get('id');

    if (this.id) {
      this.loadCustomerInfo();
      this.loadCustomerAppointment();
    }
  }

  editCustomer() {
    if (this.id) {
      const modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.id = this.id;
      modalRef.componentInstance.title = 'Sửa khách hàng';

      modalRef.result.then(result => {
        this.loadCustomerInfo();
      }, () => {})
    }
  }

  loadCustomerInfo() {
    this.partnerService.getCustomerInfo(this.id).subscribe((result: any) => {
      this.customerInfo = result;
    })
  }

  loadCustomerAppointment() {
    this.partnerService.getNextAppointment(this.id).subscribe(
      rs => {
        this.customerAppointment = rs;
      })
  }

}
