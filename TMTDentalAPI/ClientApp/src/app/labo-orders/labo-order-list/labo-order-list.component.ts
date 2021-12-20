import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { SaleOrderLineService, SaleOrderLinesLaboPaged } from 'src/app/core/services/sale-order-line.service';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { LaboOrderBasic, LaboOrderService } from '../labo-order.service';

@Component({
  selector: 'app-labo-order-list',
  templateUrl: './labo-order-list.component.html',
  styleUrls: ['./labo-order-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class LaboOrderListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  opened = false;
  search: string;
  searchUpdate = new Subject<string>();
  selectedIds: string[] = [];
  islabo: string = '';
  stateLabo: string;
  filterLabo = [
    { name: 'Đã tạo', value: 'true' },
    { name: 'Chưa tạo', value: 'false' }
  ];
  filterLaboStatus = [
    { name: 'Nháp', value: 'draft' },
    { name: 'Đơn hàng', value: 'confirmed' },
  ];
  toothTypeDict = [
    { name: "Hàm trên", value: "upper_jaw" },
    { name: "Nguyên hàm", value: "whole_jaw" },
    { name: "Hàm dưới", value: "lower_jaw" },
    { name: "Chọn răng", value: "manual" },
  ];
  // dateOrderFrom: Date;
  // dateOrderTo: Date;
  // datePlannedFrom: Date;
  // datePlannedTo: Date;
  stateFilter: string = '';

  laboStatusFilter: boolean;
  filterPaged: SaleOrderLinesLaboPaged = new SaleOrderLinesLaboPaged();

  canUpdateSaleOrder: boolean = false;

  constructor(private laboOrderService: LaboOrderService,
    private router: Router,
    private saleOrderLineService: SaleOrderLineService,
    private modalService: NgbModal,
    private checkPermissionService: CheckPermissionService,
    private authService: AuthService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.filterPaged.limit = this.limit;
    this.filterPaged.offset = this.skip;
    this.loadDataFromApi();
    this.checkRole();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.filterPaged.search = value || '';
        this.skip = 0;
        this.loadDataFromApi();
      });
  }



  // onDateSearchChange(data) {
  //   this.dateOrderFrom = data.dateFrom;
  //   this.dateOrderTo = data.dateTo;
  //   this.loadDataFromApi();
  // }

  // onDatePlannedSearchChange(data) {
  //   this.datePlannedFrom = data.dateFrom;
  //   this.datePlannedTo = data.dateTo;
  //   this.loadDataFromApi();
  // }

  onStateLaboChange(e) {
    var value = e ? e.value : null;
    if (value) {
      // this.filterPaged.laboState = value;
      this.stateLabo = value;
    } else {
      // delete this.filterPaged['laboState'];
      this.stateLabo = null;
    }
    this.skip = 0;
    this.loadDataFromApi();
  }

  reloadChange(val) {
    if (val) {
      this.loadDataFromApi();
    }

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

  unlink() {
    if (this.selectedIds.length == 0) {
      return false;
    }

    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu labo';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.laboOrderService.unlink(this.selectedIds).subscribe(() => {
        this.selectedIds = [];
        this.loadDataFromApi();
      });
    });
  }

  loadDataFromApi() {
    this.loading = true;
    this.filterPaged.limit = this.limit;
    this.filterPaged.offset = this.skip;
    this.filterPaged.hasAnyLabo = this.islabo ? this.islabo : '';
    this.filterPaged.laboState = this.stateLabo ? this.stateLabo : '';
    this.filterPaged.search = this.search ? this.search : '';
    this.filterPaged.companyId = this.authService.userInfo.companyId;
    this.saleOrderLineService.getListLineIsLabo(this.filterPaged).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      console.log(this.gridData);
      
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  onChangeLaboStatus(e) {
    var value = e ? e.value : null;
    if (value) {
      // this.filterPaged.hasAnyLabo = value == 'true';
      this.islabo = value;
    } else {
      // delete this.filterPaged['hasAnyLabo'];
      this.islabo = null;
    }
    this.skip = 0;
    this.loadDataFromApi();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  showInfo(val) {
    var list = [];
    if(val.toothType == 'manual'){
      if (val.teeth.length) {
        list.push(val.teeth.map(x => x.name).join(','));
      }
    } else {
      list.push(this.toothTypeDict.find(x=> x.value == val.toothType)?.name)
    }
   
    if (val.diagnostic) {
      list.push(val.diagnostic);
    }

    return list.join('; ');
  }

  createItem() {
    this.router.navigate(['/labo-orders/form']);
  }

  editItem(item: LaboOrderBasic) {
    this.router.navigate(['/labo-orders/form'], { queryParams: { id: item.id } });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu labo';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.laboOrderService.unlink([item.id]).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }

  checkRole(){
    this.canUpdateSaleOrder = this.checkPermissionService.check(['Basic.SaleOrder.Update']);
  }

  getToothType(key) {
    var type = this.toothTypeDict.find(x=> x.value == key);
    return type;
  }
}