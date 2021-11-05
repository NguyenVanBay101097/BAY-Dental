import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { StockPickingBasic, StockPickingPaged, StockPickingService } from '../stock-picking.service';

@Component({
  selector: 'app-stock-picking-incoming-list',
  templateUrl: './stock-picking-incoming-list.component.html',
  styleUrls: ['./stock-picking-incoming-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class StockPickingIncomingListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  dateFrom: Date = new Date(new Date().getFullYear(), new Date().getMonth(), 1);;
  dateTo: Date = new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0);
  canCreate = false;
  canUpdate = false;
  canDelete = false;
  constructor(private stockPickingService: StockPickingService, private router: Router,
    private modalService: NgbModal, private intlService: IntlService, 
    private notificationService: NotificationService,
    private checkPermissionService: CheckPermissionService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.skip = 0;
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
    this.checkRole();
  }

  getState(item: StockPickingBasic) {
    switch (item.state) {
      case 'done':
        return 'Hoàn thành';
      default:
        return 'Nháp';
    }
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new StockPickingPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.type = 'incoming';
    val.search = this.search || '';
    if (this.dateFrom) {
      val.dateFrom = this.intlService.formatDate(this.dateFrom, 'd', 'en-US');
    }
    if (this.dateTo) {
      val.dateTo = this.intlService.formatDate(this.dateTo, 'd', 'en-US');
    }

    this.stockPickingService.getPaged(val).pipe(
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

  onDateSearchChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  createItem() {
    this.router.navigate(['/stock/incoming-pickings/create']);
  }

  editItem(item: StockPickingBasic) {
    this.router.navigate(['/stock/incoming-pickings/edit', item.id]);
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa phiếu nhập kho';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa phiếu nhập kho?';
    modalRef.result.then(() => {
      this.stockPickingService.delete(item.id).subscribe(() => {
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

  checkRole(){
    this.canCreate = this.checkPermissionService.check(["Stock.Picking.Create"]);
    this.canUpdate = this.checkPermissionService.check(["Stock.Picking.Update"]);
    this.canDelete = this.checkPermissionService.check(["Stock.Picking.Delete"]);
  }
}


