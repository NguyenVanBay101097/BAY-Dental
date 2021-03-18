import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { StockPickingPaged, StockPickingService, StockPickingBasic } from '../stock-picking.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { StockPickingTypeBasic, StockPickingTypeService } from 'src/app/stock-picking-types/stock-picking-type.service';
import { Subject } from 'rxjs';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';

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
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  dateFrom: Date = new Date(new Date().getFullYear(), new Date().getMonth(), 1);;
  dateTo: Date = new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0);

  constructor(private route: ActivatedRoute, private stockPickingService: StockPickingService,
    private pickingTypeService: StockPickingTypeService, private router: Router,
    private modalService: NgbModal, private intlService: IntlService, 
    private notificationService: NotificationService 
  ) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
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
    this.loadDataFromApi();
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
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
    modalRef.componentInstance.body = 'Bạn có chắc chắn xóa phiếu nhập kho?';
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
}


