import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { SaleOrderPaymentPaged, SaleOrderPaymentService } from 'src/app/core/services/sale-order-payment.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PrintService } from 'src/app/shared/services/print.service';

@Component({
  selector: 'app-sale-order-payment-list',
  templateUrl: './sale-order-payment-list.component.html',
  styleUrls: ['./sale-order-payment-list.component.css']
})
export class SaleOrderPaymentListComponent implements OnInit {
  @Input() saleOrderId: string;
  @Output() hasDeletePayment = new EventEmitter<any>();

  paymentHistories: any = [];
  canCancel = false;
  constructor(
    private notificationService: NotificationService,
    private modalService: NgbModal,
    private printService: PrintService,
    private saleOrderPaymentService: SaleOrderPaymentService,
  ) { }

  ngOnInit() {
    this.loadPayments();
  }

  loadPayments() {
    var val = new SaleOrderPaymentPaged();
    val.limit = 0;
    if (!this.saleOrderId) {
      return;
    }
    if (this.saleOrderId) {
      val.saleOrderId = this.saleOrderId;
      this.saleOrderPaymentService.getPaged(val).subscribe(result => {
        this.paymentHistories = result.items;
      });
    }
  }

  deletePayment(payment) {
    if (payment.state == "cancel") {
      this.notificationService.show({
        content: 'Không thể hủy phiếu ở trạng thái hủy',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return;
    }

    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = payment.payments[0].journalType === 'insurance' ? 'Hủy bảo lãnh' : 'Hủy thanh toán điều trị';
    modalRef.componentInstance.body = payment.payments[0].journalType === 'insurance' ? 'Bạn chắc chắn muốn hủy bảo lãnh ?' :'Bạn có chắc chắn muốn hủy thanh toán điều trị?';
    modalRef.result.then(() => {
      this.saleOrderPaymentService.actionCancel([payment.id])
        .subscribe(() => {
          this.notificationService.show({
            content: 'Hủy thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });

          this.hasDeletePayment.emit(true);
          this.loadPayments();
        }, err => {
          console.log(err);
        })
    }, () => {
    });
  }

  printPayment(payment) {
    this.saleOrderPaymentService.getPrint(payment.id).subscribe((result: any) => {
      this.printService.printHtml(result.html);
    });
  }

  getPaymentState(state) {
    if (state == 'draft') {
      return 'Nháp'
    }
    else if (state == 'posted') {
      return 'Đã thanh toán'
    }
    else {
      return 'Hủy'
    }
  }

  showServicesName(saleOrderLines) {
    if (!saleOrderLines || saleOrderLines.length == 0)
      return "";
    return saleOrderLines.map(x => x.name).join(", ");
  }
}
