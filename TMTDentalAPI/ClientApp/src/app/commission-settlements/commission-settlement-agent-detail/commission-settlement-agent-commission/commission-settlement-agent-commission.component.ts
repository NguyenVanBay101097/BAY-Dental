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
  limit = 2;
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
  amountTotalDebit = 0;
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
    this.loadIncomAmountTotalAgent();
  }

  loadDataFromApi() {
    this.loading = true;
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
      console.log(this.items);
      this.loadItems(this.items);
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    });
  }

  loadItems(items): void {
    this.gridData = {
      data: items.slice(this.skip, this.skip + this.limit),
      total: items.length
    };

    const result = aggregateBy(this.items, [
      { aggregate: "sum", field: "baseAmount" },
      { aggregate: "sum", field: "amount" },
    ]);

    this.baseAmount = result.baseAmount ? result.baseAmount.sum : 0;
    this.amount = result.amount ? result.amount.sum : 0;
  }

  loadIncomAmountTotalAgent() {
    if (this.agentId) {
      var val = new TotalAmountAgentFilter();
      val.agentId = this.agentId;
      val.companyId = this.authService.userInfo.companyId;
      this.agentService.getAmountDebitTotalAgent(val).subscribe((res: any) => {
        this.amountTotalDebit = res;
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
    // this.loadDataFromApi();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadItems(this.items);
  }

  actionPayment(){
    const modalRef = this.modalService.open(CommissionSettlementAgentPaymentDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Chi hoa hồng';
    modalRef.componentInstance.type = 'chi';
    modalRef.componentInstance.accountType = 'commission';
    modalRef.componentInstance.agentId = this.agentId;
    modalRef.componentInstance.amountBalanceTotal = this.amount - this.amountTotalDebit;
    modalRef.result.then(() => {
      this.notifyService.notify('success', 'Thanh toán thành công');
      this.loadDataFromApi();
      this.loadIncomAmountTotalAgent();
    }, er => { })   
  }

  exportExcelFile(){
    var val = new CommissionSettlementFilterReport();
    val.dateFrom = this.dateFrom ? this.intl.formatDate(this.dateFrom, 'yyyy-MM-ddTHH:mm:ss') : '';
    val.dateTo = this.dateTo ? this.intl.formatDate(this.dateTo, 'yyyy-MM-ddTHH:mm:ss') : '';
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.agentId = this.agentId ? this.agentId : '';
    // val.commissionType = this.commissionType ? this.commissionType : '';
    val.groupBy = 'agent';
    this.commissionSettlementsService.excelCommissionDetailExport(val).subscribe((res: any) => {
      let filename = "Hoa hồng người giới thiệu";
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
