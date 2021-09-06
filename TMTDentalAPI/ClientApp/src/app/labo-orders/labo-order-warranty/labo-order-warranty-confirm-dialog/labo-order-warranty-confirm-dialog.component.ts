import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { Observable } from 'rxjs';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { LaboWarrantyService } from '../../labo-warranty.service';

@Component({
  selector: 'app-labo-order-warranty-confirm-dialog',
  templateUrl: './labo-order-warranty-confirm-dialog.component.html',
  styleUrls: ['./labo-order-warranty-confirm-dialog.component.css']
})
export class LaboOrderWarrantyConfirmDialogComponent implements OnInit {
  @Input() state: string;
  @Input() dateSendWarranty: string;
  @Input() dateReceiptInspection: string;
  @Input() dateAssemblyWarranty: string;
  @Input() laboWarrantyId: string;
  formGroup: FormGroup;

  title: string;
  label: string;
  constructor(
    public activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private laboWarrantyService: LaboWarrantyService,
    private intlService: IntlService,
    private notifyService: NotifyService,

  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      date: [new Date(), Validators.required]
    });
    this.loadDefault();
  }

  get f() {
    return this.formGroup.controls;
  }

  loadDefault() {
    let date = new Date();
    if (this.state == 'new') {
      this.title = 'Gửi bảo hành';
      this.label = 'Ngày gửi';
    }
    else if (this.state == 'sent') {
      this.title = 'Nhận nghiệm thu';
      this.label = 'Ngày nhận';
      date = new Date(this.dateSendWarranty);
    }
    else if (this.state == 'received') {
      this.title = 'Lắp bảo hành';
      this.label = 'Ngày lắp';
      date = new Date(this.dateReceiptInspection);
    }
    else if (this.state == 'assembled') {
      this.title = 'Lắp bảo hành';
      this.label = 'Ngày lắp';
      date = new Date(this.dateAssemblyWarranty);
    }
    this.formGroup.controls['date'].setValue(date);
  }

  onSave$(): Observable<any> {
    let dateObj = this.formGroup.value.date;
    let date = this.intlService.formatDate(dateObj, 'yyyy-MM-ddTHH:mm:ss');
    let val = {
      id: this.laboWarrantyId,
      date: date
    }
    if (this.state == 'new') {
      return this.laboWarrantyService.confirmSendWarranty(val)
    }
    else if (this.state == 'sent') {
      return this.laboWarrantyService.confirmReceiptInspection(val)
    }
    else if (this.state == 'received') {
      return this.laboWarrantyService.confirmAssemblyWarranty(val)
    }
  }

  onSave() {
    if (this.formGroup.invalid) {
      return;
    }

    this.onSave$().subscribe((res: any) => {
      this.notifyService.notify("success", "Lưu thành công");
      this.activeModal.close();
    });

  }

  onCancel$(): Observable<any> {
    // let val = {
    //   id: this.laboWarrantyId,
    // }
    if (this.state == 'sent') {
      return this.laboWarrantyService.cancelSendWarranty([this.laboWarrantyId])
    }
    else if (this.state == 'received') {
      return this.laboWarrantyService.cancelReceiptInspection([this.laboWarrantyId])
    }
    else if (this.state == 'assembled') {
      return this.laboWarrantyService.cancelAssemblyWarranty([this.laboWarrantyId])
    }
  }

  onCancel() {
    this.onCancel$().subscribe((res: any) => {
      this.notifyService.notify("success", "Hủy thành công");
      this.activeModal.close();
    });
  }
}
