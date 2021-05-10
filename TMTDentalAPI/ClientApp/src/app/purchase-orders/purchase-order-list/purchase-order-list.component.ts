import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbDate, NgbDateParserFormatter, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PurchaseOrderService, PurchaseOrderPaged, PurchaseOrderBasic } from '../purchase-order.service';
import { TmtOptionSelect } from 'src/app/core/tmt-option-select';
import { IntlService } from '@progress/kendo-angular-intl';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';

@Component({
  selector: 'app-purchase-order-list',
  templateUrl: './purchase-order-list.component.html',
  styleUrls: ['./purchase-order-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class PurchaseOrderListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;
  search: string;
  searchUpdate = new Subject<string>();
  selectedIds: string[] = [];
  type: string;

  dateOrderFrom: Date;
  dateOrderTo: Date;
  stateFilter: string;

  stateFilterOptions: TmtOptionSelect[] = [
    { text: 'Tất cả', value: '' },
    { text: 'Đơn hàng', value: 'purchase,done' },
    { text: 'Nháp', value: 'draft,cancel' }
  ];
  canAdd = true;
  canUpdate = true;
  canDelete = true;
  constructor(private purchaseOrderService: PurchaseOrderService,
    private router: Router,
    private modalService: NgbModal, private route: ActivatedRoute, private intlService: IntlService,
    private checkPermissionService: CheckPermissionService
    ) { }

  ngOnInit() {
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    this.route.queryParamMap.subscribe(params => {
      this.type = params.get('type');
      this.skip = 0;
      this.loadDataFromApi();
    });
    this.checkRole();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  onDateSearchChange(data) {
    this.dateOrderFrom = data.dateFrom;
    this.dateOrderTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  onStateSelectChange(data: TmtOptionSelect) {
    this.stateFilter = data.value;
    this.skip = 0;
    this.loadDataFromApi();
  }

  stateGet(state) {
    switch (state) {
      case 'purchase':
        return 'Đơn hàng';
      case 'done':
        return 'Đã khóa';
      case 'cancel':
        return 'Đã hủy';
      default:
        return 'Nháp';
    }
  }

  getTitle() {
    switch (this.type) {
      case 'refund':
        return 'Trả hàng';
      default:
        return 'Mua hàng';
    }
  }

  unlink() {
    if (this.selectedIds.length == 0) {
      return false;
    }

    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa: ' + this.getTitle();
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.purchaseOrderService.unlink(this.selectedIds).subscribe(() => {
        this.selectedIds = [];
        this.loadDataFromApi();
      });
    });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new PurchaseOrderPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.type = this.type;
    if (this.dateOrderFrom) {
      val.dateOrderFrom = this.intlService.formatDate(this.dateOrderFrom, 'd', 'en-US');
    }
    if (this.dateOrderTo) {
      val.dateOrderTo = this.intlService.formatDate(this.dateOrderTo, 'd', 'en-US');
    }
    if (this.stateFilter) {
      val.state = this.stateFilter;
    }

    this.purchaseOrderService.getPaged(val).pipe(
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
    this.loadDataFromApi();
  }

  createItem() {
    this.router.navigate(['/purchase/orders/create'], { queryParams: { type: this.type } });
  }

  editItem(item: PurchaseOrderBasic) {
    this.router.navigate(['/purchase/orders/edit/', item.id]);
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa: ' + this.getTitle();
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.purchaseOrderService.unlink([item.id]).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }

  checkRole() {
    this.canAdd = this.checkPermissionService.check(['Purchase.Order.Create']);
    this.canUpdate = this.checkPermissionService.check(['Purchase.Order.Update']);
    this.canDelete = this.checkPermissionService.check(['Purchase.Order.Delete']);
  }
}



