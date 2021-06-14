import { AuthService } from './../../auth/auth.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { AgentCommmissionPaymentDialogComponent } from '../agent-commmission-payment-dialog/agent-commmission-payment-dialog.component';
import { AgentService, CommissionAgentDetailFilter, TotalAmountAgentFilter } from '../agent.service';

@Component({
  selector: 'app-agent-commmission-form-detail',
  templateUrl: './agent-commmission-form-detail.component.html',
  styleUrls: ['./agent-commmission-form-detail.component.css']
})
export class AgentCommmissionFormDetailComponent implements OnInit {

  id: string;
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  dateFrom: Date;
  dateTo: Date;
  commissionAgentStatistics : any;
  updateSubject: Subject<boolean> = new Subject<boolean>();

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  constructor(private route: ActivatedRoute, private modalService: NgbModal,
    private agentService: AgentService, private router: Router,
    private authService: AuthService,
    private intlService: IntlService,
    private notifyService: NotifyService,
    private printService: PrintService) { }

  ngOnInit() {
    this.id = this.route.parent.snapshot.paramMap.get('id');
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
    this.loadAmountCommissionAgentTotal();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new CommissionAgentDetailFilter();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.agentId = this.id;
    val.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
    this.agentService.getCommissionAgentDetail(val).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      this.loading = false;
    })
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  loadAmountCommissionAgentTotal() {
    if (this.id) {
      debugger
      var val = new TotalAmountAgentFilter();
      val.agentId = this.id;
      val.companyId = this.authService.userInfo.companyId;
      this.agentService.getAmountCommissionAgentToTal(val).subscribe((result : any) => {
        this.commissionAgentStatistics = result;
      });
    }
  }


  actionPayment(item) {
    const modalRef = this.modalService.open(AgentCommmissionPaymentDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Chi hoa hồng';
    modalRef.componentInstance.type = 'chi';
    modalRef.componentInstance.accountType = 'commission';
    modalRef.componentInstance.agentId = this.id;
    modalRef.componentInstance.partnerId = item.partner.id;
    modalRef.result.then(() => {
      this.notifyService.notify('success', 'Thanh toán thành công');
      this.loadDataFromApi();
      this.loadAmountCommissionAgentTotal();
    }, er => { })
  }

}
