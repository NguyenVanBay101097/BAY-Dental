import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AccountPaymentPaged, AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { PartnerService } from '../partner.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AccountInvoiceRegisterPaymentDialogV2Component } from 'src/app/account-invoices/account-invoice-register-payment-dialog-v2/account-invoice-register-payment-dialog-v2.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-partner-payments',
  templateUrl: './partner-payments.component.html',
  styleUrls: ['./partner-payments.component.css']
})
export class PartnerPaymentsComponent implements OnInit {

  @Input() public partnerId: string;//Id KH
  @Input() public saleOrderId: string;//Id phiếu điều trị
  @Output() paymentChange = new EventEmitter<any>();
  gridLoading = false;
  gridView: GridDataResult;

  limit = 5;
  skip = 0;
  constructor(private service: PartnerService, private modalService: NgbModal,
    private paymentService: AccountPaymentService) { }

  ngOnInit() {
    this.loadPayments();
  }

  loadPayments() {
    this.gridLoading = true;
    var apPaged = new AccountPaymentPaged;
    apPaged.partnerId = this.partnerId ? this.partnerId : '';
    apPaged.saleOrderId = this.saleOrderId ? this.saleOrderId : '';
    apPaged.limit = this.limit;
    apPaged.offset = this.skip;
    apPaged.state = "posted";

    this.service.getPayments(apPaged).pipe(
      map(rs1 => (<GridDataResult>{
        data: rs1.items,
        total: rs1.totalItems
      }))
    ).subscribe(rs2 => {
      this.gridView = rs2;
      this.gridLoading = false;
    }, er => {
      this.gridLoading = true;
      console.log(er);
    }
    );
  }

  draftUnlinkPayment(payment) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa thanh toán';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.paymentService.unlink([payment.id]).subscribe(() => {
        this.loadPayments();
        this.paymentChange.emit(null);
      });
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadPayments();
  }

  registerPayment() {
    this.service.getDefaultRegisterPayment(this.partnerId).subscribe(result => {
      let modalRef = this.modalService.open(AccountInvoiceRegisterPaymentDialogV2Component, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Thanh toán';
      modalRef.componentInstance.defaultVal = result;
      modalRef.result.then(() => {
        this.loadPayments();
        this.paymentChange.emit(null);
      }, () => {
      });
    });
  }

}
