import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import * as moment from 'moment';
import { map } from 'rxjs/operators';
import { AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { ResInsuranceHistoriesDetailComponent } from '../res-insurance-histories-detail/res-insurance-histories-detail.component';
import { InsuranceHistoryInComeFilter } from '../res-insurance-report.model';
import { ResInsuranceReportService } from '../res-insurance-report.service';

@Component({
  selector: 'app-res-insurance-histories',
  templateUrl: './res-insurance-histories.component.html',
  styleUrls: ['./res-insurance-histories.component.css']
})
export class ResInsuranceHistoriesComponent implements OnInit {
  gridData: GridDataResult;
  limit: number = 20;
  skip: number = 0;
  pagerSettings: any;
  loading = false;
  dateFrom: Date;
  dateTo: Date;
  monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  insuranceId: string;
  constructor(
    private modalService: NgbModal,
    private notifyService: NotifyService,
    private resInsuranceReportService: ResInsuranceReportService,
    private accountPaymentService: AccountPaymentService,
    private activeRoute: ActivatedRoute,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit(): void {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.insuranceId = this.activeRoute.parent.snapshot.paramMap.get('id');
    if (this.insuranceId) {
      this.loadInsuranceHistories();
    }
  }

  loadInsuranceHistories(): void {
    let val = new InsuranceHistoryInComeFilter();
    val.limit = this.limit;
    val.offset = this.skip;
    val.dateFrom = this.dateFrom ? moment(this.dateFrom).format('YYYY-MM-DD') : '';
    val.dateTo = this.dateTo ? moment(this.dateTo).format('YYYY-MM-DD') : '';
    val.insuranceId = this.insuranceId ? this.insuranceId : '';
    this.resInsuranceReportService.getHistoryInComePaged(val).pipe(
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

  clickItem(item) {
    if (item && item.dataItem) {
      var id = item.dataItem.id
      const modalRef = this.modalService.open(ResInsuranceHistoriesDetailComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Chi tiết phiếu thu';
      modalRef.componentInstance.paymentId = id;
      modalRef.result.then(res => {
        this.loadInsuranceHistories();
      }, () => {
      });
    }
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
    event.stopPropagation();
    if (item.state === 'cancel') {
      this.notifyService.notify('error', 'Không thể hủy phiếu ở trạng thái Đã hủy');
      return;
    }

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
