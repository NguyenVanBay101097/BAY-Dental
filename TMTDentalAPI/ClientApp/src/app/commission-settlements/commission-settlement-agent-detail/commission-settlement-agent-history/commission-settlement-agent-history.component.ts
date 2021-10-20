import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { TotalAmountAgentFilter } from 'src/app/agents/agent.service';
import { PhieuThuChiPaged, PhieuThuChiSave, PhieuThuChiService } from 'src/app/phieu-thu-chi/phieu-thu-chi.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-commission-settlement-agent-history',
  templateUrl: './commission-settlement-agent-history.component.html',
  styleUrls: ['./commission-settlement-agent-history.component.css']
})
export class CommissionSettlementAgentHistoryComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  dateFrom: Date;
  dateTo: Date;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  agentId: any;

  constructor(
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig,
    private route: ActivatedRoute,
    private intlService: IntlService,
    private phieuThuChiService: PhieuThuChiService,
    private modalService: NgbModal,
    private notifyService: NotifyService,
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.agentId = this.route.parent.snapshot.paramMap.get('id');
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var val = new PhieuThuChiPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.agentId = this.agentId;
    val.accountType = 'commission';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
    this.phieuThuChiService.getPaged(val).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      console.log(res);
      this.gridData = res;
    }, err => {
    })
  }



  deleteItem(item: any) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa phiếu chi hoa hồng';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa phiếu chi hoa hồng ?';
    modalRef.result.then(() => {
      this.phieuThuChiService.actionCancel([item.id]).subscribe(() => {
        this.notifyService.notify('success', 'Xóa thành công');
        this.loadDataFromApi();

      })
    }, () => {
    });
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    // this.loadDataFromApi();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    // this.loadDataFromApi();
  }
  exportExcelFile() { }

  getState(val){
    switch (val) {
      case "cancel":
        return 'Hủy';         
      default:
        return 'Đã thanh toán';
    }
  }
}
