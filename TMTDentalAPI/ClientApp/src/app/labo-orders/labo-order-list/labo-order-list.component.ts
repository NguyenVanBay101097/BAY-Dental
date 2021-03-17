import { Component, Input, OnInit, SimpleChanges, Output } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { Router } from '@angular/router';
import { NgbDate, NgbDateParserFormatter, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { LaboOrderPaged, LaboOrderService, LaboOrderBasic } from '../labo-order.service';
import { TmtOptionSelect } from 'src/app/core/tmt-option-select';
import { IntlService } from '@progress/kendo-angular-intl';
import { SaleOrderLineService, SaleOrderLinesLaboPaged, SaleOrderLinesPaged } from 'src/app/core/services/sale-order-line.service';

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

  // dateOrderFrom: Date;
  // dateOrderTo: Date;
  // datePlannedFrom: Date;
  // datePlannedTo: Date;
  stateFilter: string = '';

  laboStatusFilter: boolean;
  filterPaged: SaleOrderLinesLaboPaged = new SaleOrderLinesLaboPaged();



  constructor(private laboOrderService: LaboOrderService,
    private router: Router,
    private saleOrderLineService: SaleOrderLineService,
    private modalService: NgbModal, private intlService: IntlService) { }

  ngOnInit() {
    this.filterPaged.limit = this.limit;
    this.filterPaged.offset = this.skip;
    this.loadDataFromApi();

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

  reloaChange(val) {
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
    this.saleOrderLineService.getListLineIsLabo(this.filterPaged).pipe(
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
    this.loadDataFromApi();
  }

  showInfo(val) {
    var list = [];
    if (val.teeth.length) {
      list.push(val.teeth.map(x => x.name).join(','));
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
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu labo';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.laboOrderService.unlink([item.id]).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }
}