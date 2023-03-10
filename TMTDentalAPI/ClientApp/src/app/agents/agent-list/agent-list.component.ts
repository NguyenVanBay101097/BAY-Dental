import { PhieuThuChiService } from 'src/app/phieu-thu-chi/phieu-thu-chi.service';
import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AgentCreateUpdateDialogComponent } from 'src/app/shared/agent-create-update-dialog/agent-create-update-dialog.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { AgentPaged, AgentService } from '../agent.service';
import { NotifyService } from './../../shared/services/notify.service';
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
  constructor(private modalService: NgbModal,
    private agentService: AgentService,
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
    modalRef.componentInstance.title = 'Th??m ng?????i gi???i thi???u';
    modalRef.result.then(() => {
      this.notifyService.notify('success', 'L??u th??nh c??ng');
      this.loadDataFromApi();
    }, er => { })
  }

  editItem(item: any) {
    const modalRef = this.modalService.open(AgentCreateUpdateDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'S???a ng?????i gi???i thi???u';
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.notifyService.notify('success', 'L??u th??nh c??ng');
      this.loadDataFromApi();
    }, () => {
    })
  }

  deleteItem(item: any) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'X??a ng?????i gi???i thi???u';
    modalRef.componentInstance.body = 'B???n c?? ch???c ch???n mu???n x??a ng?????i gi???i thi???u ?';
    modalRef.result.then(() => {
      this.agentService.delete(item.id).subscribe(() => {
        this.notifyService.notify('success', 'X??a th??nh c??ng');
        this.loadDataFromApi();
      }, () => {
      });
    }, () => {
    });
  }

  getAgentType(type) {
    switch (type) {
      case 'customer':
        return 'Kh??ch h??ng';
      case 'employee':
        return 'Nh??n vi??n';
      case 'partner':
        return '?????i t??c';
      default:
        return '';
    }
  }

  actionPayment(item: any) {
    var val =  {agentId: item.id , type:'chi'};
    this.agentService.getCommissionPaymentByAgentId(val).subscribe(res => {
      const modalRef = this.modalService.open(CommissionSettlementAgentPaymentDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Chi hoa h???ng';
      modalRef.componentInstance.type = 'chi';
      modalRef.componentInstance.accountType = 'commission';
      modalRef.componentInstance.agentId = item.id;
      modalRef.componentInstance.partnerId = item.partnerId;
      modalRef.componentInstance.amountBalanceTotal = item.amount - item.amountCommission;
      modalRef.componentInstance.resAgent = res;
      modalRef.result.then(() => {
        this.notifyService.notify('success', 'Chi hoa h???ng th??nh c??ng');
        this.loadDataFromApi();
      }, er => { })
    });


  }



}
