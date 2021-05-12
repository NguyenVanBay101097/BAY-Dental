import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AccountPaymentBasic, AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { SaleOrderPaymentPaged, SaleOrderPaymentService } from 'src/app/core/services/sale-order-payment.service';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { AccountPaymentPrintComponent } from 'src/app/shared/account-payment-print/account-payment-print.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { AccountPaymentsOdataService } from 'src/app/shared/services/account-payments-odata.service';
import { PrintService } from 'src/app/shared/services/print.service';

@Component({
  selector: 'app-sale-order-payment-list',
  templateUrl: './sale-order-payment-list.component.html',
  styleUrls: ['./sale-order-payment-list.component.css']
})
export class SaleOrderPaymentListComponent implements OnInit {
  @Input() saleOrderId: string;
  @Output() paymentOutput = new EventEmitter<any>();

  paymentHistories: any = [];

  constructor(
    private saleOrderService: SaleOrderService,
    private paymentService: AccountPaymentService,
    private notificationService: NotificationService,
    private modalService: NgbModal,
    private printService: PrintService,
    private accountPaymentOdataService: AccountPaymentsOdataService,
    private saleOrderPaymentService: SaleOrderPaymentService
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
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Hủy thanh toán';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn hủy thanh toán?';
    modalRef.result.then(() => {
      this.saleOrderPaymentService.actionCancel([payment.id]).subscribe(() => {
        this.notificationService.show({
          content: 'Hủy thanh toán thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });

        this.paymentOutput.emit('');
        this.loadPayments();
      });
    });
  }

  printPayment(payment) {
    this.saleOrderPaymentService.getPrint(payment.id).subscribe(result => {
      if (result) {
        var html = result['html']
        this.printService.printHtml(html);
      }
    });
  }

  getPaymentState(state){
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
}
