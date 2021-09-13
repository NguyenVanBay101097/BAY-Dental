import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { TmtOptionSelect } from 'src/app/core/tmt-option-select';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { LaboOrderCuDialogComponent } from 'src/app/shared/labo-order-cu-dialog/labo-order-cu-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { LaboOrderReceiptDialogComponent } from '../labo-order-receipt-dialog/labo-order-receipt-dialog.component';
import { LaboOrderBasic, LaboOrderService, OrderLaboPaged } from '../labo-order.service';

@Component({
  selector: 'app-order-labo-list',
  templateUrl: './order-labo-list.component.html',
  styleUrls: ['./order-labo-list.component.css']
})
export class OrderLaboListComponent implements OnInit {
  skip = 0;
  limit = 20;
  pagerSettings: any;
  gridData: GridDataResult;
  details: LaboOrderBasic[];
  search: string;
  searchUpdate = new Subject<string>();
  loading = false;
  stateFilter: string;

  stateFilterOptions: TmtOptionSelect[] = [
    { text: 'Tất cả', value: '' },
    { text: 'Trễ hạn', value: 'trehan' },
    { text: 'Chờ nhận', value: 'chonhan' },
    { text: 'Tới hạn', value: 'toihan' }
  ];

  filterLaboStatus = [
    { name: 'Trễ hạn', value: 'trehan' },
    { name: 'Chờ nhận', value: 'chonhan' },
    { name: 'Tới hạn', value: 'toihan' },
  ];
  canUpdate = false;
  canUpdateSaleOrder = false;
  constructor(
    private laboOrderService: LaboOrderService,
    private modalService: NgbModal,
    private intlService: IntlService,
    private notificationService: NotificationService, 
    private checkPermissionService: CheckPermissionService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loadDataFromApi();
    this.checkRole();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });

  }

  loadDataFromApi() {
    this.loading = true;
    var val = new OrderLaboPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.state = this.stateFilter || '';
    this.laboOrderService.getOrderLabo(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  getState(val) {
    var today = new Date();
    var now = new Date(today.getFullYear(), today.getMonth(), today.getDate());
    var datePlanned = new Date(val.datePlanned);
    if (val.datePlanned != null && now > datePlanned) {
      return "Trễ hạn";
    } else if (val.datePlanned != null && now.getDate() == datePlanned.getDate() && now.getMonth() == datePlanned.getMonth() && now.getFullYear() == datePlanned.getFullYear()) {
      return "Tới hạn";
    } else {
      return "Chờ nhận";
    }
  }

  getTextColor(val) {
    var today = new Date();
    var now = new Date(today.getFullYear(), today.getMonth(), today.getDate());
    var datePlanned = new Date(val.datePlanned);
    if (val.datePlanned != null && now > datePlanned) {
      return { 'text-danger': true };
    } else if (val.datePlanned != null && now.getDate() == datePlanned.getDate() && now.getMonth() == datePlanned.getMonth() && now.getFullYear() == datePlanned.getFullYear()) {
      return { 'text-success': true };
    } else {
      return;
    }
  }

  onStateSelectChange(data: TmtOptionSelect) {
    this.stateFilter = data.value;
    this.loadDataFromApi();
  }

  onChangeLaboState(e) {
    this.stateFilter = e ? e.value : null;
    this.skip = 0;
    this.loadDataFromApi();
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  editItem(item) {
    const modalRef = this.modalService.open(LaboOrderReceiptDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.labo = item;
    modalRef.result.then(() => {
      this.notificationService.show({
        content: 'Cập nhật thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.loadDataFromApi();
    }, er => { });
  }
  
  editLabo(item){
    console.log(item);
    const modalRef = this.modalService.open(LaboOrderCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Cập nhật phiếu labo';
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.saleOrderLineId = item.saleOrderLine.id;
    modalRef.componentInstance.saleOrderLineLabo = item;

    modalRef.result.then(res => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  cellClick(item) {
    console.log(item);

    const modalRef = this.modalService.open(LaboOrderCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thông tin phiếu Labo';
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.saleOrderLineId = item.saleOrderLineId;

    modalRef.result.then(res => {
      if (res) {
        this.loadDataFromApi();
      }
    }, () => {
    });
  }

  checkRole() {
    this.canUpdate = this.checkPermissionService.check(['Labo.OrderLabo.Update']);
    this.canUpdateSaleOrder = this.checkPermissionService.check(['Basic.SaleOrder.Update']);
  }
}
