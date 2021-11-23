import { Component, Inject, Input, OnInit, SimpleChanges } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import * as moment from 'moment';
import { map } from 'rxjs/operators';
import { AccountPaymentPaged, AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { PartnerService } from 'src/app/partners/partner.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-res-insurance-histories',
  templateUrl: './res-insurance-histories.component.html',
  styleUrls: ['./res-insurance-histories.component.css']
})
export class ResInsuranceHistoriesComponent implements OnInit {
  @Input() partnerId: string;
  gridData: GridDataResult;
  limit: number = 20;
  skip: number = 0;
  pagerSettings: any;
  loading = false;
  dateFrom: Date;
  dateTo: Date;
  monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(
    private modalService: NgbModal,
    private notifyService: NotifyService,
    private partnerService: PartnerService,
    private accountPaymentService: AccountPaymentService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit(): void {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.partnerId.currentValue) {
      this.loadInsuranceHistories();
    }
  }

  loadInsuranceHistories(): void {
    let val = new AccountPaymentPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.partnerId = this.partnerId ? this.partnerId : '';
    val.state = "posted";
    val.paymentDateFrom = this.dateFrom ? moment(this.dateFrom).format('YYYY-MM-DD') : '';
    val.paymentDateTo = this.dateTo ? moment(this.dateTo).format('YYYY-MM-DD') : '';
    this.partnerService.getPayments(val).pipe(
      map(rs1 => (<GridDataResult>{
        data: rs1.items,
        total: rs1.totalItems
      }))
    ).subscribe(rs2 => {
      this.gridData = rs2;
    }, er => {
      console.log(er);
    }
    );
  }

  onSearchDateChange(e): void {
    this.dateFrom = e.dateFrom;
    this.dateTo = e.dateTo;
    this.skip = 0;
    this.loadInsuranceHistories();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadInsuranceHistories();
  }

  onCancelPaymnet(item): void {
    let modalRef = this.modalService.open(ConfirmDialogComponent, {
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Hủy phiếu thu bảo hiểm";
    modalRef.componentInstance.body = "Bạn chắc chắn muốn hủy phiếu thu bảo hiểm?";
    modalRef.result.then(() => {
      this.accountPaymentService.actionCancel([item.id]).subscribe(() => {
        this.loadInsuranceHistories();
        this.notifyService.notify('success', 'Hủy thành công');
      });
    }, () => { })
  }
}
