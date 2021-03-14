import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ProductRequestPaged } from 'src/app/sale-orders/product-request';
import { ProductRequestService } from 'src/app/shared/product-request.service';
import { StockPickingRequestProductDialogComponent } from '../stock-picking-request-product-dialog/stock-picking-request-product-dialog.component';

@Component({
  selector: 'app-stock-picking-request-product',
  templateUrl: './stock-picking-request-product.component.html',
  styleUrls: ['./stock-picking-request-product.component.css']
})
export class StockPickingRequestProductComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  states: any[] = [{ value: 'confirmed', name: 'Đang yêu cầu' }, { value: 'done', name: 'Đã xuất' }];
  searchUpdate = new Subject<string>();
  state: string = "confirmed,done";
  dateFrom: Date;
  dateTo: Date;

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());


  constructor(
    private modalService: NgbModal,
    private intlService: IntlService,
    private productRequestService: ProductRequestService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.skip = 0;
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ProductRequestPaged();
    val.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:59");
    val.limit = this.limit;
    val.offset = this.skip;
    val.state = this.state;
    val.search = this.search || '';

    this.productRequestService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      console.log(res);
      
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  onSateChange(item) {
    this.skip = 0;
    if (item) {
      this.state = item.value;
      this.loadDataFromApi();
    } else {
      this.state = "confirmed,done";
      this.loadDataFromApi();
    }
  }

  onDateSearchChange(data) {
    this.skip = 0;
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.loadDataFromApi();
  }

  onUpdate(event) {
    let modalRef = this.modalService.open(StockPickingRequestProductDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.id = event.dataItem ? event.dataItem.id : null;
    modalRef.componentInstance.title = "Yêu cầu vật tư"
    modalRef.result.then(() => {
      this.notificationService.show({
        content: 'Xác nhận thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.loadDataFromApi();
    }, () => {
    });
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

}
