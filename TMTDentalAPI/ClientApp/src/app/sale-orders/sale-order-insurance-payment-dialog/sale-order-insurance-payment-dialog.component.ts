import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ResInsuranceCuDialogComponent } from 'src/app/shared/res-insurance-cu-dialog/res-insurance-cu-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-sale-order-insurance-payment-dialog',
  templateUrl: './sale-order-insurance-payment-dialog.component.html',
  styleUrls: ['./sale-order-insurance-payment-dialog.component.css']
})
export class SaleOrderInsurancePaymentDialogComponent implements OnInit {
  @Input() title: string;
  constructor(
    private activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private notifyService: NotifyService,
  ) { }

  ngOnInit(): void {
  }

  onSave(): void {

  }

  quickCreateInsurance(): void {
    let modalRef = this.modalService.open(ResInsuranceCuDialogComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal modal-appointment', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm công ty bảo hiểm';
    modalRef.result.then((result) => {
      this.notifyService.notify("success", "Lưu thành công")
    }, () => { });
  }

  changeDiscountType(): void {

  }
  onCancel(): void {
    this.activeModal.dismiss();
  }
}
