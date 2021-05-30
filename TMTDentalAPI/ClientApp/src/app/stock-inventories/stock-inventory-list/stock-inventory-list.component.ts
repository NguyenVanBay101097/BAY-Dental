import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PrecscriptionPaymentPaged, StockInventoryService } from '../stock-inventory.service';

@Component({
  selector: 'app-stock-inventory-list',
  templateUrl: './stock-inventory-list.component.html',
  styleUrls: ['./stock-inventory-list.component.css']
})
export class StockInventoryListComponent implements OnInit {
  gridData: GridDataResult;
  searchUpdate = new Subject<string>();
  search: string;
  loading = false;
  limit = 20;
  dateFrom: Date;
  dateTo: Date;
  offset = 0;
  state: string;
  filterInventoryState = [
    { name: 'Nháp', value: 'draft' },
    { name: 'Đang xử lý', value: 'confirmed' },
    { name: 'Hoàn thành', value: 'done' },
  ];

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  canCreate = false;
  constructor(
    private stockInventorySevice: StockInventoryService,
    private notificationService: NotificationService,
    private intlService: IntlService,
    private router: Router,
    private modalService: NgbModal,
    private checkPermissionService: CheckPermissionService
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.search = value || '';
        this.offset = 0;
        this.loadDataFromApi();
      });
    this.checkRole();
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var paged = new PrecscriptionPaymentPaged();
    paged.limit = this.limit;
    paged.offset = this.offset;
    paged.state = this.state ? this.state : '';
    paged.search = this.search ? this.search : '';
    paged.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd")
    paged.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:59")
    this.stockInventorySevice.getPaged(paged).pipe(
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
    this.offset = event.skip;
    this.loadDataFromApi();
  }

  createItem() {
    this.router.navigate(['stock-inventories/form']);
  }

  onSearchDateChange(data) {
    this.offset = 0;
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.loadDataFromApi();
  }

  onStateChange(e) {
    this.offset = 0;
    var value = e ? e.value : null;
    if (value) {
      this.state = value;
    } else {
      this.state = null;
    }

    this.loadDataFromApi();
  }

  editItem(item) {
    var id = item.id;
    this.router.navigate(['/stock-inventories/form'], { queryParams: { id: id } });
    // this.router.navigate(['stock-inventories/form', item.id]);
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu kiểm kho';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa phiếu kiểm kho ?';
    modalRef.result.then(() => {
      this.stockInventorySevice.delete(item.id).subscribe(() => {
        this.notificationService.show({
          content: 'Xóa thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadDataFromApi();
      });
    });
  }

  getState(state) {
    switch (state) {
      case 'draft':
        return 'Nháp';
      case 'confirmed':
        return 'Đang xử lý';
      case 'done':
        return 'Hoàn thành';
    }
  }

  checkRole() {
    this.canCreate = this.checkPermissionService.check(["Stock.Inventory.Create"]);
  }

}
