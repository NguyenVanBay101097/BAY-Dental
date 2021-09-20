import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { SaleOrderLinesLaboPaged } from 'src/app/core/services/sale-order-line.service';
import { LaboOrderPaged, LaboOrderService } from 'src/app/labo-orders/labo-order.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { LaboOrderCuDialogComponent } from 'src/app/shared/labo-order-cu-dialog/labo-order-cu-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-partner-customer-labo-orders-component',
  templateUrl: './partner-customer-labo-orders-component.component.html',
  styleUrls: ['./partner-customer-labo-orders-component.component.css']
})
export class PartnerCustomerLaboOrdersComponentComponent implements OnInit {

  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  state: string;
  filterStatus = [
    { name: 'Nháp', value: 'draft' },
    { name: 'Đơn hàng', value: 'confirmed' },
  ];
  customerId: string;
  dateExportFrom: Date;
  dateExportTo: Date;
  canUpdateSaleOrder: boolean = false;
  canReadLaboWarranty: boolean = false;

  constructor(private laboOrderService: LaboOrderService, 
    private route: ActivatedRoute, private modalService: NgbModal, 
      private printService: PrintService,
      private checkPermissionService: CheckPermissionService,
      private intlService: IntlService, 
      private notifyService: NotifyService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.customerId = this.route.parent.snapshot.paramMap.get('id');

    this.loadDataFromApi();
    this.checkRole();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new LaboOrderPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.state = this.state == undefined ? '' : this.state;
    val.customerId = this.customerId || '';
    val.search = this.search || '';
    if (this.dateExportFrom) {
      val.dateExportFrom = this.intlService.formatDate(this.dateExportFrom, 'd', 'en-US');
    }
    if (this.dateExportTo) {
      val.dateExportTo = this.intlService.formatDate(this.dateExportTo, 'd', 'en-US');
    }
    this.laboOrderService.getLaboForSaleOrderLine(val).pipe(
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

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  onStateChange(e) {
    var value = e ? e.value : null;
    if (value) {
      this.state = value;
    } else {
      this.state = null;
    }
    this.skip = 0;
    this.loadDataFromApi();
  }

  // getTeeth(val) {
  //   return val.teeth.map(x => x.name).join(',');
  // }

  getTeeth(val) {
    var list = [];
    if (val.teeth.length) {
      list.push(val.teeth.map(x => x.name).join(','));
    }
    return list;
  }

  stateGet(state) {
    switch (state) {
      case 'confirmed':
        return 'Đơn hàng';
      default:
        return 'Nháp';
    }
  }

  printLabo(item: any) {
    this.laboOrderService.getPrint(item.id).subscribe((result: any) => {
      this.printService.printHtml(result);
    });
  }

  editItem(item) {
    const modalRef = this.modalService.open(LaboOrderCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Cập nhật phiếu labo';
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.saleOrderLineId = item.saleOrderLineId;
    // modalRef.componentInstance.saleOrderLineLabo = item;

    modalRef.result.then(res => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'md', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu Labo';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa phiếu Labo?';
    modalRef.result.then(() => {
      this.laboOrderService.unlink([item.id]).subscribe(() => {
        this.notifyService.notify("success", "Xóa phiếu Labo thành công");
        this.loadDataFromApi();
      });
    });
  }

  dateExportChange(data) {
    this.dateExportFrom = data.dateFrom;
    this.dateExportTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  checkRole(){
    this.canUpdateSaleOrder = this.checkPermissionService.check(['Basic.SaleOrder.Update']);
    this.canReadLaboWarranty = this.checkPermissionService.check(['Labo.LaboWarranty.Read']);
  }
}
