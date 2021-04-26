import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { SmsManualDialogComponent } from '../sms-manual-dialog/sms-manual-dialog.component';

@Component({
  selector: 'app-sms-appointment-form-manual',
  templateUrl: './sms-appointment-form-manual.component.html',
  styleUrls: ['./sms-appointment-form-manual.component.css']
})
export class SmsAppointmentFormManualComponent implements OnInit {
  formGroup: FormGroup;
  filteredTemplate: any[];
  gridData: any;
  skip: number = 0;
  limit: number = 20;
  isRowSelected: any[];
  search: string = '';
  selectedIds: string[] = [];

  constructor(
    private modalService: NgbModal,
    private partnerService: PartnerService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var val = new PartnerPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.customer = true;
    val.supplier = false;
    this.partnerService.getCustomerAppointments(val)
      .subscribe((res: any[]) => {
        this.gridData = res;
      }, err => {
        console.log(err);
      }
      )
  }

  pageChange(event) {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  onSend() {
    if (this.selectedIds.length == 0) {
      this.notify("chưa chọn khách hàng", false);
    }
    else {
      var modalRef = this.modalService.open(SmsManualDialogComponent, { size: "lg", windowClass: "o_technical_modal" });
      modalRef.componentInstance.title = "Tạo tin gửi";
      modalRef.componentInstance.ids = this.selectedIds ? this.selectedIds : [];
      modalRef.componentInstance.provider = "res.appointment"
      modalRef.result.then(
        result => {

        }
      )
    }
  }

  notify(title, isSuccess = true) {
    this.notificationService.show({
      content: title,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: isSuccess ? 'success' : 'error', icon: true },
    });
  }
}
