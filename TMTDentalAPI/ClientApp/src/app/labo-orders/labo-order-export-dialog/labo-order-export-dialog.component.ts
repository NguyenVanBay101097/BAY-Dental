import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { LaboOrderService } from '../labo-order.service';

@Component({
  selector: 'app-labo-order-export-dialog',
  templateUrl: './labo-order-export-dialog.component.html',
  styleUrls: ['./labo-order-export-dialog.component.css']
})
export class LaboOrderExportDialogComponent implements OnInit {
  labo : any;
  formGroup: FormGroup;
  submitted = false;
  private btnConfirm = new Subject<any>();
  get f() { return this.formGroup.controls; }

  
  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private intlService: IntlService,
    private laboOrderService: LaboOrderService,
    private notificationService: NotificationService,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      dateExport: [null, Validators.required]
    });

    if (this.labo) {
      if (this.labo.dateExport) {
        this.formGroup.get('dateExport').setValue(new Date(this.labo.dateExport));
      } else {
        this.formGroup.get('dateExport').setValue(new Date());
      }
    }
  }

  onSave() {
    this.submitted = true;
    if (!this.formGroup.valid) {
      return false;
    }

    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xác nhận xuất Labo cho khách';
    modalRef.componentInstance.body = 'Phiếu Labo đã xuất cho khách không thể xóa và chỉnh sửa.';
    modalRef.componentInstance.body2 = 'Bạn chắc chắn muốn xuất Labo cho khách ?';
    modalRef.result.then(() => {
      var val = this.formGroup.value;
      val.id = this.labo.id;
      val.dateExport = val.dateExport ? this.intlService.formatDate(val.dateExport, 'yyyy-MM-dd HH:mm:ss') : null;

      this.laboOrderService.updateExportLabo(val).subscribe(() => { 
        this.activeModal.close("reload");
        this.notificationService.show({
          content: 'Xuất Labo thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.btnConfirm.next();
      }, err => {
        this.notificationService.show({
          content: 'Xuất Labo không thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'error', icon: true }
        });
      });
    }, () => {
    });
  }

  onCancelReceipt() {
    this.laboOrderService.actionCancelReceipt([this.labo.id]).subscribe(() => {   
      var status = 'remove';
      this.notificationService.show({
        content: 'Hủy nhận thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });   
      this.activeModal.close(status);
    });
  }

  getBtnConfirm() {
    return this.btnConfirm.asObservable();
  }
  
}
