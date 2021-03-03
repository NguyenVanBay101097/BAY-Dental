import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ProductRequestPaged } from 'src/app/sale-orders/product-request';
import { ProductRequestService } from 'src/app/shared/product-request.service';

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
    val.states = this.states.map(x => x.value);
    val.search = this.search || '';

    this.productRequestService.getPaged(val).pipe(
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

}
