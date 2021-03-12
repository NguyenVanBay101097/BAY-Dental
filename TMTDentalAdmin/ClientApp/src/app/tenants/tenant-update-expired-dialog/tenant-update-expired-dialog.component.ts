import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { TenantService } from '../tenant.service';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TenantExtendHistoryService } from '../tenant-extend-history.service';
import { ConfirmDialogComponent } from '@shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-tenant-update-expired-dialog',
  templateUrl: './tenant-update-expired-dialog.component.html',
  styleUrls: ['./tenant-update-expired-dialog.component.css']
})
export class TenantUpdateExpiredDialogComponent implements OnInit {
  title = 'Gia hạn';
  formGroup: FormGroup;
  id: string;
  expirationDate: Date;
  endDate: Date;
  startDate: Date;
  limit: number;
  limitOption: any;
  tenant: any;
  today: Date = new Date();

  constructor(private fb: FormBuilder, private intlService: IntlService,
    private tenantExtendHistoryService: TenantExtendHistoryService,
    private tenantService: TenantService,
    private modalService: NgbModal,
    private activeModal: NgbActiveModal) { }

  ngOnInit() {
    if (this.today <= this.expirationDate)
      this.startDate = new Date(this.expirationDate.getFullYear(), this.expirationDate.getMonth(), this.expirationDate.getDate() + 1);
    else {
      this.startDate = this.today;
    }
    this.formGroup = this.fb.group({
      limit: [1],
      checkOption: "time",
      limitOption: ['month'],
      activeCompaniesNbr: [this.tenant.activeCompaniesNbr]
    });
    this.limit = this.formGroup.get('limit') ? this.formGroup.get('limit').value : 0;
    this.limitOption = this.formGroup.get('limitOption') ? this.formGroup.get('limitOption').value : null;
    this.computeEndDate();
  }

  getFormGroup() {
    if (this.formGroup.invalid)
      return;
    var value = this.formGroup.value;
    return value;
  }

  changeRadio() {
    var checkOption = this.formGroup.get('checkOption') ? this.formGroup.get('checkOption').value : null;
    if (checkOption) {
      switch (checkOption) {
        case "company":
          this.startDate = this.today;
          this.endDate = this.expirationDate;
          if (this.startDate > this.expirationDate) {
            let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
            modalRef.componentInstance.body = "Tên miền này đã hết hạn, Vui lòng gia hạn thời gian trước khi thêm chi nhánh!";
            modalRef.componentInstance.title = `Lưu ý`;
            modalRef.componentInstance.isClose = false;
            modalRef.result.then(() => {
              this.formGroup.get('checkOption').setValue("time");
              if (this.today <= this.expirationDate)
                this.startDate = new Date(this.expirationDate.getFullYear(), this.expirationDate.getMonth(), this.expirationDate.getDate() + 1);
              else {
                this.startDate = this.today;
              }
              this.computeEndDate();
            })
          }
          break;
        case "time":
          if (this.today <= this.expirationDate)
            this.startDate = new Date(this.expirationDate.getFullYear(), this.expirationDate.getMonth(), this.expirationDate.getDate() + 1);
          else {
            this.startDate = this.today;
          }
          this.computeEndDate();
          break;
        default:
          break;
      }
    }
  }

  get checkOption() {
    return this.formGroup.get('checkOption') ? this.formGroup.get('checkOption').value : null;
  }

  computeEndDate() {
    this.limit = this.formGroup.get('limit') ? this.formGroup.get('limit').value : 0;
    this.limitOption = this.formGroup.get('limitOption') ? this.formGroup.get('limitOption').value : null;
    if (this.limit && this.limitOption) {
      switch (this.limitOption) {
        case "day":
          this.endDate = new Date(this.startDate.getFullYear(), this.startDate.getMonth(), this.startDate.getDate() + this.limit);
          break;
        case "month":
          this.endDate = new Date(this.startDate.getFullYear(), this.startDate.getMonth() + this.limit, this.startDate.getDate());
          break;
        case "year":
          this.endDate = new Date(this.startDate.getFullYear() + this.limit, this.expirationDate.getMonth(), this.startDate.getDate());
          break;
        default:
          break;
      }
    }
  }

  onSave() {
    var val = this.getFormGroup();
    val.tenantId = this.id;
    this.tenantService.extendExpired(val).subscribe(() => {
      this.activeModal.close(true);
    }, (err) => {
      if (err.message) {
        alert(err.message);
      }
    });
  }
}
