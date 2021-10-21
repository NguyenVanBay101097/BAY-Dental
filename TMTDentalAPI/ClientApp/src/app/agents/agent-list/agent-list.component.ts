import { NotifyService } from './../../shared/services/notify.service';
import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { AgentPaged, AgentService } from '../agent.service';
import { AgentCreateUpdateDialogComponent } from 'src/app/shared/agent-create-update-dialog/agent-create-update-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { AgentCommmissionPaymentDialogComponent } from '../agent-commmission-payment-dialog/agent-commmission-payment-dialog.component';
import { CommissionSettlementAgentPaymentDialogComponent } from 'src/app/commission-settlements/commission-settlement-agent-payment-dialog/commission-settlement-agent-payment-dialog.component';

@Component({
  selector: 'app-agent-list',
  templateUrl: './agent-list.component.html',
  styleUrls: ['./agent-list.component.css']
})
export class AgentListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;

  search: string;
  searchUpdate = new Subject<string>();
  constructor(private route: ActivatedRoute, private modalService: NgbModal,
    private agentService: AgentService, private router: Router,
    private notifyService: NotifyService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new AgentPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    this.agentService.getPaged(val).pipe(
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
    this.limit = event.take;
    this.loadDataFromApi();
  }

  createItem() {
    const modalRef = this.modalService.open(AgentCreateUpdateDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm người giới thiệu';
    modalRef.result.then(() => {
      this.notifyService.notify('success','Lưu thành công');
      this.loadDataFromApi();
    }, er => { })
  }

  editItem(item: any) {
    const modalRef = this.modalService.open(AgentCreateUpdateDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa người giới thiệu';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.notifyService.notify('success','Lưu thành công');
      this.loadDataFromApi();
    }, () => {
    })
  }

  deleteItem(item: any) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa người giới thiệu';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa người giới thiệu ?';
    modalRef.result.then(() => {
      this.agentService.delete(item.id).subscribe(() => {
        this.notifyService.notify('success','Xóa thành công');
        this.loadDataFromApi();
      }, () => {
      });
    }, () => {
    });
  }

  getAgentType(type){
    switch(type) {
      case 'customer':
        return 'Khách hàng';
      case 'employee':
        return 'Nhân viên';
      case 'partner':
        return 'Đối tác';
      default:
        return '';
    }
  }

  actionPayment(item: any){
    const modalRef = this.modalService.open(CommissionSettlementAgentPaymentDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Chi hoa hồng';
    modalRef.componentInstance.type = 'chi';
    modalRef.componentInstance.accountType = 'commission';
    modalRef.componentInstance.agentId = item.id;
    modalRef.componentInstance.partnerId = item.partnerId;
    modalRef.componentInstance.amountBalanceTotal = item.amount - item.amountCommission;
    modalRef.result.then(() => {
      this.notifyService.notify('success', 'Chi hoa hồng thành công');
      this.loadDataFromApi();
    }, er => { })
  }



}
