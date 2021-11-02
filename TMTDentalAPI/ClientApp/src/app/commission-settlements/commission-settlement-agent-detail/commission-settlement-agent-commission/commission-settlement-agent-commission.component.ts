import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AgentService, TotalAmountAgentFilter } from 'src/app/agents/agent.service';
import { AuthService } from 'src/app/auth/auth.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { CommissionSettlementAgentPaymentDialogComponent } from '../../commission-settlement-agent-payment-dialog/commission-settlement-agent-payment-dialog.component';
import { CommissionSettlementFilterReport, CommissionSettlementsService } from '../../commission-settlements.service';

@Component({
  selector: 'app-commission-settlement-agent-commission',
  templateUrl: './commission-settlement-agent-commission.component.html',
  styleUrls: ['./commission-settlement-agent-commission.component.css']
})
export class CommissionSettlementAgentCommissionComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  dateFrom: Date;
  dateTo: Date;
  agentId: string;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  items: any;
  baseAmount: number = 0;
  amount: number = 0;
  commissionPaid: number = 0;
  amountDebit: any;
  totalAmountAgent: any;
  agentObj: any;
  constructor(
    private intl: IntlService,
    private commissionSettlementsService: CommissionSettlementsService,
    private route: ActivatedRoute,
    private modalService: NgbModal,
    private authService: AuthService,
    private agentService: AgentService,
    private notifyService: NotifyService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.agentId = this.route.parent.snapshot.paramMap.get('id');
    this.loadDataFromApi();
    this.loadAmountDebitTotalAgent();
    this.loadAgent();
    this.loadSumAmountTotal();
  }

  loadAgent() {
    this.agentService.get(this.agentId).subscribe((res: any) => {
      this.agentObj = res;
    });
  }

  loadDataFromApi() {
    var val = new CommissionSettlementFilterReport();
    // val.offset = this.skip;
    // val.limit = 0;
    val.search = this.search ? this.search : '';
    val.agentId = this.agentId ? this.agentId : '';
    // val.commissionType = this.commissionType ? this.commissionType : '';
    val.dateFrom = this.dateFrom ? this.intl.formatDate(this.dateFrom, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.dateTo = this.dateFrom ? this.intl.formatDate(this.dateTo, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.groupBy = 'agent';
    this.commissionSettlementsService.getReportDetail(val).subscribe((res: any) => {
      this.items = res.items;
      this.loadItems(this.items);
    }, err => {
      console.log(err);
    });
  }

  loadItems(items): void {
    this.gridData = {
      data: items.slice(this.skip, this.skip + this.limit),
      total: items.length
    };
  }

  loadSumAmountTotal() {
    var val = new CommissionSettlementFilterReport();
    val.agentId = this.agentId ? this.agentId : '';
    val.groupBy = 'agent';
    this.commissionSettlementsService.getSumAmountTotalReport(val).subscribe((res: any) => {
      this.totalAmountAgent = res;
    }, err => {
      console.log(err);
    });
  }

  loadAmountDebitTotalAgent() {
    if (this.agentId) {
      var val = new TotalAmountAgentFilter();
      val.agentId = this.agentId;
      val.companyId = this.authService.userInfo.companyId;
      this.agentService.getAmountDebitTotalAgent(val).subscribe((res: any) => {
        this.amountDebit = res;
      },
        (error) => {
          console.log(error);
        }
      );
    }
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadItems(this.items);
  }

  actionPayment() {
    const modalRef = this.modalService.open(CommissionSettlementAgentPaymentDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Chi hoa hồng';
    modalRef.componentInstance.type = 'chi';
    modalRef.componentInstance.accountType = 'commission';
    modalRef.componentInstance.agentId = this.agentId;
    modalRef.componentInstance.amountBalanceTotal = this.totalAmountAgent.totalComissionAmount - this.amountDebit.amountDebitTotal;
    modalRef.componentInstance.partnerId = this.agentObj ? this.agentObj.partnerId : null;
    modalRef.result.then(() => {
      this.notifyService.notify('success', 'Thanh toán thành công');
      this.loadDataFromApi();
      this.loadAmountDebitTotalAgent();
      this.loadSumAmountTotal();
    }, er => { })
  }

  exportExcelFile() {
    var val = new CommissionSettlementFilterReport();
    val.dateFrom = this.dateFrom ? this.intl.formatDate(this.dateFrom, 'yyyy-MM-ddTHH:mm:ss') : '';
    val.dateTo = this.dateTo ? this.intl.formatDate(this.dateTo, 'yyyy-MM-ddTHH:mm:ss') : '';
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.agentId = this.agentId ? this.agentId : '';
    val.groupBy = 'agent';
    this.commissionSettlementsService.exportExcelCommissionItemDetail(val).subscribe((res: any) => {
      let filename = "HoaHongNguoiGioiThieu";
      let newBlob = new Blob([res], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        window.URL.revokeObjectURL(data);
      }, 100);
    })
  }
}
