import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { map } from 'rxjs/operators';
import { AccountPaymentPaged, AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
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
  pagerSettings: any;
  constructor(
    private service: PartnerService,
    private modalService: NgbModal,
    private paymentService: AccountPaymentService,
    private activeRoute: ActivatedRoute,
    private printService: PrintService,
    private notificationService: NotificationService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettingsPopup }

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
        return 'Ho??n ti???n';
      case 'outbound':
        return 'Tr??? ti???n';
      default:
        return '';
    }
  }

  draftUnlinkPayment(payment) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'H???y thanh to??n';
    modalRef.componentInstance.body = 'B???n c?? ch???c ch???n mu???n h???y thanh to??n n??y?';
    modalRef.result.then(() => {
      this.paymentService.actionCancel([payment.id]).subscribe(() => {
        this.loadPayments();
        this.notificationService.show({
          content: 'H???y th??nh c??ng',
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

  printPayment(payment) {
    // this.accountPaymentOdataService.getPrint(payment.id).subscribe(result => {
    //   if (result) {
    //     var html = result['html']
    //     this.printService.printHtml(html);
    //   }
    // });
    if (payment && payment.id) {
      this.paymentService.getPrint(payment.id).subscribe((result: any) => {
        this.printService.printHtml(result);
      });
    }
  }
}
