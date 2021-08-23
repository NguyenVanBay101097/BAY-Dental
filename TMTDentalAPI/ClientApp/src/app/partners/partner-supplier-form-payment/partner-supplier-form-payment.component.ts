import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { map } from 'rxjs/operators';
import { AccountPaymentPaged, AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { AccountPaymentsOdataService } from 'src/app/shared/services/account-payments-odata.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { PartnerService } from '../partner.service';

@Component({
  selector: 'app-partner-supplier-form-payment',
  templateUrl: './partner-supplier-form-payment.component.html',
  styleUrls: ['./partner-supplier-form-payment.component.css']
})
export class PartnerSupplierFormPaymentComponent implements OnInit {
  id: string;
  gridLoading = false;
  gridView: GridDataResult;
  limit = 10;
  skip = 0;
  pageSizes = [20, 50, 100, 200];
  constructor(
    private service: PartnerService,
    private modalService: NgbModal,
    private paymentService: AccountPaymentService,
    private activeRoute: ActivatedRoute,
    private accountPaymentOdataService: AccountPaymentsOdataService,
    private printService: PrintService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.id = this.activeRoute.parent.snapshot.paramMap.get('id');
    this.loadPayments();
  }

  loadPayments() {
    this.gridLoading = true;
    var apPaged = new AccountPaymentPaged;
    apPaged.partnerId = this.id ? this.id : '';
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

  showPaymentType(type: string) {
    switch (type) {
      case 'inbound':
        return 'Hoàn tiền';
      case 'outbound':
        return 'Trả tiền';
      default:
        return '';
    }
  }

  draftUnlinkPayment(payment) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Hủy thanh toán';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn hủy thanh toán này?';
    modalRef.result.then(() => {
      this.paymentService.actionCancel([payment.id]).subscribe(() => {
        this.loadPayments();
        this.notificationService.show({
          content: 'Hủy thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      });
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadPayments();
  }

  onPageSizeChange(value: number): void {
    this.skip = 0;
    this.limit = value;
    this.loadPayments();
  }

  printPayment(payment) {
    // this.accountPaymentOdataService.getPrint(payment.id).subscribe(result => {
    //   if (result) {
    //     var html = result['html']
    //     this.printService.printHtml(html);
    //   }
    // });
    if (payment && payment.id) {
      this.paymentService.getPrint(payment.id).subscribe(result => {
        this.printService.printHtml(result);
      });
    }
  }
}
