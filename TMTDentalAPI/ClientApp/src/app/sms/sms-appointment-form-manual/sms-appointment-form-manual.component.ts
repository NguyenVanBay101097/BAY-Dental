import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { PartnerPaged } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { SmsManualDialogComponent } from '../sms-manual-dialog/sms-manual-dialog.component';

@Component({
  selector: 'app-sms-appointment-form-manual',
  templateUrl: './sms-appointment-form-manual.component.html',
  styleUrls: ['./sms-appointment-form-manual.component.css']
})
export class SmsAppointmentFormManualComponent implements OnInit {
  // @ViewChild("smsTemplateCbx", { static: true }) smsTemplateCbx: ComboBoxComponent

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
    // if (this.formGroup.invalid) { return; }
    // var val = this.formGroup.value;
    // val.partnerIds = this.selectedIds ? this.selectedIds : [];
    // console.log(val);

    var modalRef = this.modalService.open(SmsManualDialogComponent, { size: "sm", windowClass: "o_technical_modal" });
    modalRef.componentInstance.title = "Tạo tin gửi";
    modalRef.componentInstance.id = this.selectedIds ? this.selectedIds : [];
    modalRef.result.then(
      result => {

      }
    )
  }
}
